﻿using System.Diagnostics;
using System.Globalization;

using Renci.SshNet.Common;

namespace Renci.SshNet.IntegrationTests.OldIntegrationTests
{
    /// <summary>
    /// Implementation of the SSH File Transfer Protocol (SFTP) over SSH.
    /// </summary>
    public sealed partial class SftpClientTest : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory("Sftp")]
        [ExpectedException(typeof(SftpPermissionDeniedException))]
        public void Test_Sftp_ListDirectory_Permission_Denied()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                var files = sftp.ListDirectory("/root");
                foreach (var file in files)
                {
                    Debug.WriteLine(file.FullName);
                }

                sftp.Disconnect();
            }
        }

        [TestMethod]
        [TestCategory("Sftp")]
        [ExpectedException(typeof(SftpPathNotFoundException))]
        public void Test_Sftp_ListDirectory_Not_Exists()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                var files = sftp.ListDirectory("/asdfgh");
                foreach (var file in files)
                {
                    Debug.WriteLine(file.FullName);
                }

                sftp.Disconnect();
            }
        }

        [TestMethod]
        [TestCategory("Sftp")]
        public void Test_Sftp_ListDirectory_Current()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                var files = sftp.ListDirectory(".");

                Assert.IsTrue(files.Any());
            }
        }

#if NET6_0_OR_GREATER
        [TestMethod]
        [TestCategory("Sftp")]
        public async Task Test_Sftp_ListDirectoryAsync_Current()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromMinutes(1));

                await sftp.ConnectAsync(cts.Token).ConfigureAwait(false);

                var count = 0;
                await foreach (var file in sftp.ListDirectoryAsync(".", cts.Token).ConfigureAwait(false))
                {
                    count++;
                }

                Assert.IsTrue(count > 0);

                sftp.Disconnect();
                cts.Dispose();
            }
        }
#endif
        [TestMethod]
        [TestCategory("Sftp")]
        public void Test_Sftp_ListDirectory_Empty()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                var files = sftp.ListDirectory(string.Empty);

                Assert.IsTrue(files.Any());
            }
        }

        [TestMethod]
        [TestCategory("Sftp")]
        [Description("Test passing null to ListDirectory.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Sftp_ListDirectory_Null()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                var files = sftp.ListDirectory(path: null);

                Assert.IsTrue(files.Any());
            }
        }

        [TestMethod]
        [TestCategory("Sftp")]
        public void Test_Sftp_ListDirectory_HugeDirectory()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                // Create 10000 directory items
                for (var i = 0; i < 10000; i++)
                {
                    sftp.CreateDirectory(string.Format(CultureInfo.InvariantCulture, "test_{0}", i));
                }

                var files = sftp.ListDirectory(".");

                // Ensure that directory has at least 10000 items
                Assert.IsTrue(files.Count() > 10000);

                sftp.Disconnect();
            }

            RemoveAllFiles();
        }

        [TestMethod]
        [TestCategory("Sftp")]
        public void Test_Sftp_Change_Directory()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                Assert.AreEqual("/home/sshnet", sftp.WorkingDirectory);

                sftp.CreateDirectory("test1");

                sftp.ChangeDirectory("test1");

                Assert.AreEqual("/home/sshnet/test1", sftp.WorkingDirectory);

                sftp.CreateDirectory("test1_1");
                sftp.CreateDirectory("test1_2");
                sftp.CreateDirectory("test1_3");

                var files = sftp.ListDirectory(".");

                Assert.IsTrue(files.First().FullName.StartsWith(string.Format("{0}", sftp.WorkingDirectory), StringComparison.Ordinal));

                sftp.ChangeDirectory("test1_1");

                Assert.AreEqual("/home/sshnet/test1/test1_1", sftp.WorkingDirectory);

                sftp.ChangeDirectory("../test1_2");

                Assert.AreEqual("/home/sshnet/test1/test1_2", sftp.WorkingDirectory);

                sftp.ChangeDirectory("..");

                Assert.AreEqual("/home/sshnet/test1", sftp.WorkingDirectory);

                sftp.ChangeDirectory("..");

                Assert.AreEqual("/home/sshnet", sftp.WorkingDirectory);

                files = sftp.ListDirectory("test1/test1_1");

                Assert.IsTrue(files.First().FullName.StartsWith(string.Format("{0}/test1/test1_1", sftp.WorkingDirectory), StringComparison.Ordinal));

                sftp.ChangeDirectory("test1/test1_1");

                Assert.AreEqual("/home/sshnet/test1/test1_1", sftp.WorkingDirectory);

                sftp.ChangeDirectory("/home/sshnet/test1/test1_1");

                Assert.AreEqual("/home/sshnet/test1/test1_1", sftp.WorkingDirectory);

                sftp.ChangeDirectory("/home/sshnet/test1/test1_1/../test1_2");

                Assert.AreEqual("/home/sshnet/test1/test1_2", sftp.WorkingDirectory);

                sftp.ChangeDirectory("../../");

                sftp.DeleteDirectory("test1/test1_1");
                sftp.DeleteDirectory("test1/test1_2");
                sftp.DeleteDirectory("test1/test1_3");
                sftp.DeleteDirectory("test1");

                sftp.Disconnect();
            }

            RemoveAllFiles();
        }

        [TestMethod]
        [TestCategory("Sftp")]
        [Description("Test passing null to ChangeDirectory.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Sftp_ChangeDirectory_Null()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                sftp.ChangeDirectory(path: null);

                sftp.Disconnect();
            }
        }

        [TestMethod]
        [TestCategory("Sftp")]
        [Description("Test calling EndListDirectory method more then once.")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Sftp_Call_EndListDirectory_Twice()
        {
            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();
                var ar = sftp.BeginListDirectory("/", asyncCallback: null, state: null);
                var result = sftp.EndListDirectory(ar);
                var result1 = sftp.EndListDirectory(ar);
            }
        }
    }
}
