using System;
using Uplauncher.Managers;
using Uplauncher.Sound.Network;

namespace Shadow.Sound.Reg
{
    public class RegClient
    {
        #region Déclaration
        private readonly Client _client;

        public event EventHandler<DisconnectedArgs> Disconnected;
        #endregion

        public RegClient(Client client)
        {
            _client = client;

            if (client != null)
            {
                _client.DataReceived += this.ClientDataReceive;
                _client.Disconnected += this.ClientDisconnected;
            }
        }

        /// <summary>
        /// Permet de déconnecter le client
        /// </summary>
        public void Dipose()
        {
            _client.DataReceived -= ClientDataReceive;
            _client.Disconnected -= this.ClientDisconnected;

            _client.Stop();
        }

        #region Events
        private void ClientDataReceive(object sender, Client.DataReceivedEventArgs e)
        {
            SoundManager.GameServer.Client.Send(e.Data);
        }

        private void ClientDisconnected(object sender, Client.DisconnectedEventArgs e)
        {
            OnDisconnected(new DisconnectedArgs(this));
        }
        private void OnDisconnected(DisconnectedArgs e)
        {
            if (Disconnected != null)
                Disconnected(this, e);
        }
        #endregion
        public class DisconnectedArgs : EventArgs
        {
            public RegClient Host { get; private set; }

            public DisconnectedArgs(RegClient host) => Host = host;
        }

        public void Send(byte[] data)
        {
            if (_client.Runing)
                _client.Send(data);
        }
    }
}
