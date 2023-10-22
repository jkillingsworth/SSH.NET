﻿using System.Globalization;

namespace Renci.SshNet.IntegrationTests
{
    internal sealed class SshConnectionDisruptor
    {
        private readonly IConnectionInfoFactory _connectionInfoFactory;

        public SshConnectionDisruptor(IConnectionInfoFactory connectionInfoFactory)
        {
            _connectionInfoFactory = connectionInfoFactory;
        }

        public SshConnectionRestorer BreakConnections()
        {
            var client = new SshClient(_connectionInfoFactory.Create());

            client.Connect();

            PauseSshd(client);

            return new SshConnectionRestorer(client);
        }

        private static void PauseSshd(SshClient client)
        {
            var command = client.CreateCommand("sudo echo 'DenyUsers sshnet' >> /etc/ssh/sshd_config");
            var output = command.Execute();
            if (command.ExitStatus != 0)
            {
                throw new InvalidOperationException($"Blocking user sshnet failed with exit code {command.ExitStatus.ToString(CultureInfo.InvariantCulture)}.{Environment.NewLine}{output}{Environment.NewLine}{command.Error}");
            }

            command = client.CreateCommand("sudo pkill -9 -U sshnet -f sshd.pam");
            output = command.Execute();
            if (command.ExitStatus != 0)
            {
                throw new InvalidOperationException($"Killing sshd.pam service failed with exit code {command.ExitStatus.ToString(CultureInfo.InvariantCulture)}.{Environment.NewLine}{output}{Environment.NewLine}{command.Error}");
            }
        }
    }
}
