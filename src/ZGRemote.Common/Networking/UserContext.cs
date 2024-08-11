using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Serilog;
using ZGRemote.Common.Extensions;
using ZGRemote.Common.Utils;

namespace ZGRemote.Common.Networking
{
    public class UserContext : IDisposable
    {
        private bool disposedValue;

        public Socket Socket { get; set; }
        public ZGServer Server { get; set; }
        public ZGClient Client { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public DateTime ConnectTime { get; set; }
        public ICryptoTransform AesEncryptor { get; set; }
        public ICryptoTransform AesDecryptor { get; set; }

        public UserContext()
        {
        }

        public UserContext(Socket socket, ICryptoTransform aesEncrypt, ICryptoTransform aesDecryptor)
        {
            Socket = socket;
            AesEncryptor = aesEncrypt;
            AesDecryptor = aesDecryptor;
            IPEndPoint ip = socket.RemoteEndPoint as IPEndPoint;
            IP = ip.Address.ToString();
            Port = ip.Port;
            ConnectTime = DateTime.Now;
        }

        public void SendPack(byte[] buffer)
        {
            try
            {
                var encryptData = AesUtil.Encrypt(buffer, AesEncryptor);
                Socket.SendPack(encryptData);

            }
            catch (SocketException socketException)
            {
                Log.Information(socketException.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "send fail");
            }
        }

        public async Task SendPackAsync(byte[] buffer)
        {
            try
            {
                using (NetworkStream stream = new NetworkStream(Socket))
                {
                    await stream.WriteAsync(BitConverter.GetBytes((buffer.Length / 16) * 16 + 16), 0, 4);
                    using (CryptoStream cryptoStream = new CryptoStream(stream, AesEncryptor, CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (SocketException socketException)
            {
                Log.Information(socketException.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "send fail");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
            {
                Server = null;
                AesEncryptor.Dispose();
                AesDecryptor.Dispose();
            }
            Socket.Dispose();
            Socket = null;
            disposedValue = true;
        }

        ~UserContext() { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}


