using System.Net.Sockets;
using Uplauncher.Sound.Network;

namespace Shadow.Sound.Reg
{
    public class RegServer
    {
        #region Déclaration
        private readonly Server _server;
        private RegClient _client;
        #endregion


        public RegServer() => _server = new Server();

        public void StartAuthentificate()
        {
            _server.Start(8080);
            _server.ConnectionAccepted += AccepteClient;
        }

        #region Socket auth
        private void AccepteClient(Socket client)
        {
            var newClient = new Client(client);
            _client = new RegClient(newClient);
        }

        private void ClientDisconnected(object sender, RegClient.DisconnectedArgs e)
        {
            _client = null;
        }
        #endregion

        public RegClient Client => _client;
    }
}
