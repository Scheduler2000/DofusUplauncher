using System;
using System.Net;
using System.Net.Sockets;

namespace Uplauncher.Sound.Network
{
    public class Client
    {

        #region Variables

        public Socket Socket;
        public bool Runing { get; private set; }
        private byte[] _sendBuffer, _receiveBuffer;
        private byte[] _buffer;
        private const int _bufferLength = 8192;
        public bool Normaly = false;
        #endregion

        #region Builder

        public Client(string ip, short port)
        {
            Init();
            Start(ip, port);
        }

        public Client(Socket socket)
        {
            Init();
            Start(socket);
        }

        public Client() => Init();

        #endregion

        #region Methods

        public void Start(Socket socket)
        {
            try
            {
                Runing = true;
                Socket = socket;
                Socket.BeginReceive(_receiveBuffer, 0, _bufferLength, SocketFlags.None, new AsyncCallback(ReceiveCallBack), Socket);
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        public void Start(string ip, short port)
        {
            try
            {
                Socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ConnectionCallBack), Socket);
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        public void Start(IPEndPoint endPoint)
        {
            try
            {
                Socket.BeginConnect(endPoint, new AsyncCallback(ConnectionCallBack), Socket);
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        public void Stop()
        {
            try
            {
                if (Socket.Connected == true)
                    Socket.BeginDisconnect(false, DisconectedCallBack, Socket);
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                if (Socket.Connected == false)
                    Runing = false;
                if (Runing)
                {
                    if (data.Length == 0)
                        return;
                    _sendBuffer = data;
                    Socket.BeginSend(_sendBuffer, 0, _sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), Socket);
                }
                else
                    Console.WriteLine("Send " + data.Length + " bytes but not runing");
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        public void Dispose()
        {
            // Dispose

            if (Socket != null)
                Socket.Dispose();

            if (_buffer != null)
                _buffer = null;

            // Clean

            Socket = null;
            _sendBuffer = null;
            _receiveBuffer = null;
            _buffer = null;
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            try
            {
                _buffer = new byte[8192];
                _receiveBuffer = new byte[_bufferLength];
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        #endregion

        #region CallBack

        private void ConnectionCallBack(IAsyncResult asyncResult)
        {
            try
            {
                Runing = true;
                var client = (Socket)asyncResult.AsyncState;
                client.EndConnect(asyncResult);
                client.BeginReceive(_receiveBuffer, 0, _bufferLength, SocketFlags.None, new AsyncCallback(ReceiveCallBack), client);
                OnConnected(new ConnectedEventArgs());
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        private void DisconectedCallBack(IAsyncResult asyncResult)
        {
            try
            {
                Runing = false;
                var client = (Socket)asyncResult.AsyncState;
                client.EndDisconnect(asyncResult);
                OnDisconnected(new DisconnectedEventArgs(this));
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        private void ReceiveCallBack(IAsyncResult asyncResult)
        {
            var client = (Socket)asyncResult.AsyncState;
            if (client.Connected == false)
            {
                Runing = false;
                return;
            }
            if (Runing)
            {
                int bytesRead = 0;

                try
                {
                    bytesRead = client.EndReceive(asyncResult);
                }
                catch (Exception ex)
                {
                    OnError(new ErrorEventArgs(ex));
                }

                if (bytesRead == 0)
                {
                    Runing = false;
                    OnDisconnected(new DisconnectedEventArgs(this));
                    return;
                }
                byte[] data = new byte[bytesRead];
                Array.Copy(_receiveBuffer, data, bytesRead);
                _buffer = data;
                OnDataReceived(new DataReceivedEventArgs(_buffer));
                try
                {
                    client.BeginReceive(_receiveBuffer, 0, _bufferLength, SocketFlags.None, new AsyncCallback(ReceiveCallBack), client);
                }
                catch (Exception ex)
                {
                    OnError(new ErrorEventArgs(ex));
                }
            }
            else
                Console.WriteLine("Receive data but not running");
        }

        private void SendCallBack(IAsyncResult asyncResult)
        {
            try
            {
                if (Runing == true)
                {
                    var client = (Socket)asyncResult.AsyncState;
                    client.EndSend(asyncResult);
                    OnDataSended(new DataSendedEventArgs());
                }
                else
                    Console.WriteLine("Send data but not runing !");
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        #endregion

        #region Events

        public event EventHandler<ConnectedEventArgs> Connected;
        public event EventHandler<DisconnectedEventArgs> Disconnected;
        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<DataSendedEventArgs> DataSended;
        public event EventHandler<ErrorEventArgs> Error;

        private void OnConnected(ConnectedEventArgs e)
        {
            if (Connected != null)
                Connected(this, e);
        }

        private void OnDisconnected(DisconnectedEventArgs e)
        {
            if (Disconnected != null)
                Disconnected(this, e);
        }

        private void OnDataReceived(DataReceivedEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(this, e);
        }

        private void OnDataSended(DataSendedEventArgs e)
        {
            if (DataSended != null)
                DataSended(this, e);
        }

        private void OnError(ErrorEventArgs e)
        {
            if (Error != null)
                Error(this, e);
        }

        #endregion

        #region EventArgs

        public class ConnectedEventArgs : EventArgs
        {
        }

        public class DisconnectedEventArgs : EventArgs
        {
            public Client Socket { get; private set; }

            public DisconnectedEventArgs(Client socket) => Socket = socket;
        }

        public class DataSendedEventArgs : EventArgs
        {
        }

        public class DataReceivedEventArgs : EventArgs
        {
            public byte[] Data { get; private set; }

            public DataReceivedEventArgs(byte[] data) => Data = data;
        }

        public class ErrorEventArgs : EventArgs
        {
            public Exception Ex { get; private set; }

            public ErrorEventArgs(Exception ex) => Ex = ex;
        }

        #endregion

    }
}
