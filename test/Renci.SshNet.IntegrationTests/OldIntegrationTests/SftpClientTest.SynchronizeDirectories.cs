﻿namespace Renci.SshNet.IntegrationTests.OldIntegrationTests
{
    /// <summary>
    /// Implementation of the SSH File Transfer Protocol (SFTP) over SSH.
    /// </summary>
    public sealed partial class SftpClientTest : IntegrationTestBase
    {
        [TestMethod]
        [TestCategory("Sftp")]
        public void Test_Sftp_SynchronizeDirectories()
        {
            RemoveAllFiles();

            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                var uploadedFileName = Path.GetTempFileName();

                var sourceDir = Path.GetDirectoryName(uploadedFileName);
                var destDir = "/home/sshnet/";
                var searchPattern = Path.GetFileName(uploadedFileName);
                var upLoadedFiles = sftp.SynchronizeDirectories(sourceDir, destDir, searchPattern);

                Assert.IsTrue(upLoadedFiles.Any());
            }
        }

        [TestMethod]
        [TestCategory("Sftp")]
        public void Test_Sftp_BeginSynchronizeDirectories()
        {
            RemoveAllFiles();

            using (var sftp = new SftpClient(SshServerHostName, SshServerPort, User.UserName, User.Password))
            {
                sftp.Connect();

                string uploadedFileName = Path.GetTempFileName();

                string sourceDir = Path.GetDirectoryName(uploadedFileName);
                string destDir = "/home/sshnet/";
                string searchPattern = Path.GetFileName(uploadedFileName);

                var asyncResult = sftp.BeginSynchronizeDirectories(sourceDir,
                                                                   destDir,
                                                                   searchPattern,
                                                                   asyncCallback: null,
                                                                   state: null);

                // Wait for the WaitHandle to become signaled.
                asyncResult.AsyncWaitHandle.WaitOne(1000);

                var upLoadedFiles = sftp.EndSynchronizeDirectories(asyncResult);

                Assert.IsTrue(upLoadedFiles.Any());

                // Close the wait handle.
                asyncResult.AsyncWaitHandle.Close();

                sftp.Disconnect();
            }
        }
    }
}
