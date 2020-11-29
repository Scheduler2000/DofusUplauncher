using System.Net.Sockets;
using Uplauncher.Sound.Network;

namespace Uplauncher.Sound.Game
{
    public class GameServer
    {
        #region Déclaration
        private readonly Server _server;
        private GameClient _client;
        #endregion


        public GameServer() => _server = new Server();

        public void StartAuthentificate()
        {
            _server.Start(8081);
            _server.ConnectionAccepted += AccepteClient;
        }

        #region Socket auth
        private void AccepteClient(Socket client)
        {
            var newClient = new Client(client);
            _client = new GameClient(newClient);
        }
        private void ClientDisconnected(object sender, GameClient.DisconnectedArgs e)
        {
            _client = null;
        }
        #endregion

        public GameClient Client => _client;
    }
}
