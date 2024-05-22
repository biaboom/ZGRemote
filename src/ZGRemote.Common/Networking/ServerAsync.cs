using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using ZGRemote.Common.Extensions;
using ZGRemote.Common.Util;
using ZGRemote.Common.Logging;
using Serilog;

namespace ZGRemote.Common.Networking
{
    public class ServerAsync
    {
        private Socket _listenSocket;
        private bool _running;
        private RSACryptoServiceProvider _rsa;
        private List<UserContext> _clientList;
        private int _maxClient;
        private int _clientCount;
        private int _bufferSize;
        public List<UserContext> ClientList { get { return _clientList; } }
        public int ClientCount { get { return _clientCount; } }
        public bool IsRunning { get { return _running; } }
        public event Action<UserContext, byte[]> OnReceive;
        public event Action<UserContext> OnConnect;
        public event Action<UserContext> OnDisConnect;

        public ServerAsync(byte[] rsaBlobKey, int bufferSize, int maxClient)
        {
            _bufferSize = bufferSize;
            _maxClient = maxClient;
            _clientCount = 0;
            _clientList = new List<UserContext>(maxClient);
            _running = false;
            _rsa = new RSACryptoServiceProvider(2048);
            _rsa.ImportCspBlob(rsaBlobKey);
        }

        public void Start(string IP = "127.0.0.1", int PORT = 9527)
        {
            if (!_running)
            {
                _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listenSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), PORT));
                _listenSocket.Listen(_maxClient);
                _listenSocket.KeepAlive();
                StartAccept();
                _running = true;
            }
        }

        public void Stop()
        {
            if (_running)
            {
                foreach (var item in _clientList)
                {
                    CloseSocket(item.Socket);
                    item.Dispose();
                }
                _clientList.Clear();
                CloseSocket(_listenSocket);
                _running = false;
            }
        }

        private async void StartAccept()
        {
            try
            {
                while (true)
                {
                    Socket client = await _listenSocket.AcceptAsync();
                    _ = ProcessAccept(client);
                }
            }
            catch (Exception ex)
            {
                Log.Warning("listen error", ex);
            }
        }

        private bool Authentication(Socket socket, out byte[] key)
        {
            var sha256 = SHA256.Create();
            try
            {
                byte[] helloBytes = new byte[] { 72, 101, 108, 108, 111, 32, 83, 101, 114, 118, 101, 114 };
                socket.SendPack(_rsa.SignData(helloBytes, sha256));
                byte[] key_buffer = socket.ReceivePack();
                key = _rsa.Decrypt(key_buffer, false);
                if (key.Length != 32) return false;
                return true;
            }
            catch (Exception ex)
            {
                key = null;
                Log.Warning(ex.Message);
                return false;
            }
            finally
            {
                sha256.Dispose();
            }
        }

        private async Task ProcessAccept(Socket socket)
        {
            // 超过最大连接数或验证失败，断开连接
            if (_clientCount >= _maxClient || !Authentication(socket, out byte[] key))
            {
                IPEndPoint ip_ = socket.RemoteEndPoint as IPEndPoint;
                if (_clientCount >= _maxClient)
                {
                    Log.Warning($"The connection limit has been reached, {ip_.Address.ToString()}:{ip_.Port}");
                }
                else
                {
                    Log.Warning($"Authentication fail, {ip_.Address.ToString()}:{ip_.Port}");
                }
                CloseSocket(socket);
                return;
            }

            UserContext userContext = new UserContext();
            // 创建aes加密器和解密器
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                userContext.AesEncryptor = aes.CreateEncryptor();
                userContext.AesDecryptor = aes.CreateDecryptor();
            }
            userContext.ConnectTime = DateTime.Now;
            userContext.Socket = socket;
            userContext.Server = this;

            IPEndPoint ip = socket.RemoteEndPoint as IPEndPoint;
            userContext.IP = ip.Address.ToString();
            userContext.Port = ip.Port;

            Interlocked.Increment(ref _clientCount);
            lock (_clientList) _clientList.Add(userContext);

            OnConnect?.Invoke(userContext);

            var pipe = new Pipe();
            Task writing = FillPipeAsync(socket, pipe.Writer);
            Task reading = ReadPipeAsync(pipe.Reader, userContext);
            await Task.WhenAll(reading, writing);
            CloseClient(userContext);
        }

        private async Task FillPipeAsync(Socket socket, PipeWriter writer)
        {
            while (true)
            {
                // Allocate at least _bufferSize bytes from the PipeWriter.
                Memory<byte> memory = writer.GetMemory(_bufferSize);
                try
                {
                    int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None);
                    if (bytesRead == 0)
                    {
                        // socket close
                        break;
                    }
                    // Tell the PipeWriter how much was read from the Socket.
                    writer.Advance(bytesRead);
                }
                catch (Exception ex)
                {
                    Log.Information(ex.Message);
                    break;
                }

                // Make the data available to the PipeReader.
                FlushResult result = await writer.FlushAsync();

                if (result.IsCompleted)
                {
                    break;
                }
            }

            // By completing PipeWriter, tell the PipeReader that there's no more data coming.
            await writer.CompleteAsync();
        }

        private async Task ReadPipeAsync(PipeReader reader, UserContext userContext)
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (TryParseHeaderLength(ref buffer, out int len))
                {
                    // Process the pack.
                    if (buffer.Length < len) break;
                    var pack = buffer.Slice(4, len);
                    ProcessPack(pack, userContext);
                    buffer = buffer.Slice(pack.End);
                }

                // Tell the PipeReader how much of the buffer has been consumed.
                reader.AdvanceTo(buffer.Start, buffer.End);

                // Stop reading if there's no more data coming.
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Mark the PipeReader as complete.
            await reader.CompleteAsync();
        }

        private bool TryParseHeaderLength(ref ReadOnlySequence<byte> buffer, out int length)
        {
            // If there's not enough space, the length can't be obtained.
            if (buffer.Length < 4)
            {
                length = 0;
                return false;
            }

            // Grab the first 4 bytes of the buffer.
            var lengthSlice = buffer.Slice(buffer.Start, 4);
            if (lengthSlice.IsSingleSegment)
            {
                // Fast path since it's a single segment.
                length = BinaryPrimitives.ReadInt32LittleEndian(lengthSlice.First.Span);
            }
            else
            {
                // There are 4 bytes split across multiple segments. Since it's so small, it
                // can be copied to a stack allocated buffer. This avoids a heap allocation.
                Span<byte> stackBuffer = stackalloc byte[4];
                lengthSlice.CopyTo(stackBuffer);
                length = BinaryPrimitives.ReadInt32BigEndian(stackBuffer);
            }
            return true;
        }

        private void ProcessPack(ReadOnlySequence<byte> pack, UserContext userContext)
        {
            try
            {
                OnReceive?.Invoke(userContext, AesUtil.Decrypt(pack.ToArray(), userContext.AesDecryptor));
            }
            catch(Exception ex)
            {
                Log.Error("ProcessPack Error", ex);
            }
            
        }

        private void CloseSocket(Socket socket)
        {
            try
            {
                socket?.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                socket?.Close();
            }

        }

        public void CloseClient(UserContext userContext)
        {
            OnDisConnect?.Invoke(userContext);
            CloseSocket(userContext.Socket);
            lock (_clientList)
            {
                if (_clientList.Remove(userContext))
                {
                    Interlocked.Decrement(ref _clientCount);
                }
            }
            // Log.Information($"{userContext.IP}:{userContext.Port} is Close");
            userContext.Dispose();
        }

        // public void SendPack(UserContext userContext, byte[] buffer)
        // {

        //     // socket.SendAllBytes(BitConverter.GetBytes(buffer.Length), 0, 4);
        //     // socket.SendAllBytes(buffer, 0, buffer.Length);

        //     try
        //     {
        //         var encryptData = AesUtil.Encrypt(buffer, userContext.AesEncryptor);
        //         userContext.Socket.SendPack(encryptData);
        //     }
        //     catch (SocketException socketException)
        //     {
        //         Log.Information(socketException.Message);
        //         CloseClient(userContext);
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex, "send fail");
        //     }
        // }
        // public async Task SendPackAsync(UserContext userContext, byte[] buffer)
        // {

        //     try
        //     {
        //         using (NetworkStream stream = new NetworkStream(userContext.Socket))
        //         {
        //             await stream.WriteAsync(BitConverter.GetBytes((buffer.Length / 16) * 16 + 16), 0, 4);
        //             using (CryptoStream cryptoStream = new CryptoStream(stream, userContext.AesEncryptor, CryptoStreamMode.Write))
        //             {
        //                 await cryptoStream.WriteAsync(buffer, 0, buffer.Length);
        //             }
        //         }
        //     }
        //     catch (SocketException socketException)
        //     {
        //         Log.Information(socketException.Message);
        //         CloseClient(userContext);
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex, "send fail");
        //     }
        // }

    }

}

