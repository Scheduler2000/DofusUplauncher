using System;
using System.Net;
using System.Net.Sockets;

namespace Uplauncher.Sound.Network
{
    public class Server
    {

        #region Variables

        private readonly Socket _socketListener;
        private bool _runing = false;

        public bool Connected => _runing;
        public Server() => _socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        #endregion

        #region Methods

        public void Start(short listenPort)
        {
            if (!_runing)
            {
                _runing = true;
                _socketListener.Bind(new IPEndPoint(IPAddress.Any, listenPort));
                _socketListener.Listen(5);
                _socketListener.BeginAccept(BeiginAcceptCallBack, _socketListener);
            }
        }

        public void Stop()
        {
            _runing = false;
            _socketListener.Shutdown(SocketShutdown.Both);
        }

        private void BeiginAcceptCallBack(IAsyncResult result)
        {
            if (_runing)
            {
                var listener = (Socket)result.AsyncState;
                Socket acceptedSocket = listener.EndAccept(result);
                OnConnectionAccepted(acceptedSocket);
                _socketListener.BeginAccept(BeiginAcceptCallBack, _socketListener);
            }
        }

        #endregion

        #region Events

        public delegate void ConnectionAcceptedDelegate(Socket acceptedSocket);
        public event ConnectionAcceptedDelegate ConnectionAccepted;
        private void OnConnectionAccepted(Socket client)
        {
            if (ConnectionAccepted != null)
                ConnectionAccepted(client);
        }

        #endregion

    }
}
