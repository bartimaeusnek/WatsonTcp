﻿namespace WatsonTcp
{
    using System;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Threading;

    public class ClientMetadata : IDisposable
    {
        #region Public-Members

        public TcpClient TcpClient => _TcpClient;

        public NetworkStream NetworkStream => _NetworkStream;

        public SslStream SslStream
        {
            get => _SslStream;
            set => _SslStream = value;
        }

        public string IpPort => _IpPort;

        public SemaphoreSlim ReadLock { get; set; }

        public SemaphoreSlim WriteLock { get; set; }

        #endregion

        #region Private-Members

        private bool _Disposed = false;

        private readonly TcpClient _TcpClient;
        private readonly NetworkStream _NetworkStream;
        private SslStream _SslStream;
        private readonly string _IpPort;

        #endregion

        #region Constructors-and-Factories

        public ClientMetadata(TcpClient tcp)
        {
            _TcpClient = tcp ?? throw new ArgumentNullException(nameof(tcp));
            _NetworkStream = tcp.GetStream();
            _IpPort = tcp.Client.RemoteEndPoint.ToString();

            ReadLock = new SemaphoreSlim(1);
            WriteLock = new SemaphoreSlim(1);
        }

        #endregion

        #region Public-Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private-Methods

        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_SslStream != null)
                {
                    _SslStream.Close();
                }

                if (_NetworkStream != null)
                {
                    _NetworkStream.Close();
                }

                if (_TcpClient != null)
                {
                    _TcpClient.Close();
                }
            }

            ReadLock.Dispose();
            WriteLock.Dispose();

            _Disposed = true;
        }

        #endregion
    }
}
