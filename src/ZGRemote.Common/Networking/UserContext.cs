using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Serilog;
using ZGRemote.Common.Extensions;
using ZGRemote.Common.Util;

namespace ZGRemote.Common.Networking
{
    public class UserContext : IDisposable
    {
        private bool disposedValue;

        public Socket Socket { get; set; }
        public Server Server { get; set; }
        public Client Client { get; set; }
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


        public virtual void Clear()
        {
            Socket = null;
            Server = null;
            ConnectTime = DateTime.MinValue;
            AesEncryptor.Dispose();
            AesDecryptor.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear();
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~UserContext()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}


