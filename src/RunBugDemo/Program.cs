using System;

using Renci.SshNet;
using Renci.SshNet.Common;

namespace RunBugDemo
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                var host = "localhost";
                var port = 22;
                var username = "MyUsername";
                var password = "MyPassword";
                var passPhrase = "MyPassphrase";
                var fileName = @"C:\Users\MyUsername\.ssh\id_rsa";
                var keyFile = new PrivateKeyFile(fileName, passPhrase);
                var method1 = new PrivateKeyAuthenticationMethod(username, keyFile);
                var method2 = new PasswordAuthenticationMethod(username, password);
                var connectionInfo = new ConnectionInfo(host, port, username, method1, method2);

                var client = new SftpClient(connectionInfo)
                {
                    OperationTimeout = TimeSpan.FromSeconds(5)
                };

                try
                {
                    BugDemo.Write("Program.Main - Connecting...");
                    client.Connect();
                    BugDemo.Write("Program.Main - Connected.");

                    BugDemo.Flag = true;

                    BugDemo.Write("Program.Main - Directory listing...");
                    var items = client.ListDirectory("/");
                    BugDemo.Write("Program.Main - Directory listed.");

                    foreach (var item in items)
                    {
                        BugDemo.Write("Program.Main - Item: " + item.FullName);
                    }
                }
                finally
                {
                    BugDemo.Write("Program.Main - Disposing...");
                    client.Dispose();
                    BugDemo.Write("Program.Main - Disposed.");
                }
            }
            catch (SshOperationTimeoutException)
            {
                BugDemo.Write("Program.Main - Caught timeout exception.");
                throw;
            }
        }
    }
}
