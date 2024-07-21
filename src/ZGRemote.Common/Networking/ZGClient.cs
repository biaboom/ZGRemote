using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Serilog;
using ZGRemote.Common.Extensions;
using ZGRemote.Common.Util;

namespace ZGRemote.Common.Networking
{
    public class ZGClient
    {
        private Socket _socket;
        private RSACryptoServiceProvider _rsa;
        private Aes _aes;
        public event Action<UserContext> Connect;
        public event Action<UserContext> DisConnect;
        public event Action<UserContext, byte[]> Receive;

        public bool Connected { get { return _socket.Connected; } }

        public ZGClient(byte[] rsaBlobKey)
        {
            _rsa = new RSACryptoServiceProvider(2048);
            _rsa.ImportCspBlob(rsaBlobKey);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _aes = Aes.Create();
            _aes.Mode = CipherMode.ECB;
            _aes.Padding = PaddingMode.PKCS7;
        }

        public bool ConnectServer(string IP = "127.0.0.1", int PORT = 9527)
        {
            if (_socket.Connected) return false;
            try
            {
                _socket.Connect(new IPEndPoint(IPAddress.Parse(IP), PORT));
                if (!Authentication(_socket))
                {
                    if (_socket.Connected) _socket.Disconnect(true);
                    return false;
                }
                UserContext userContext = new UserContext(_socket, _aes.CreateEncryptor(), _aes.CreateDecryptor());
                userContext.Client = this;
                try
                {
                    Connect?.Invoke(userContext);
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "OnConnect Error");
                }
                
                StartReceive(userContext);
                return true;
            }
            catch
            {
                if (_socket.Connected) _socket.Disconnect(true);
                return false;
            }
        }
        public void Disconnect()
        {
            if (_socket.Connected) _socket.Disconnect(true);
        }
        private bool Authentication(Socket socket)
        {
            var sha256 = SHA256.Create();
            try
            {

                byte[] helloBytes = new byte[] { 72, 101, 108, 108, 111, 32, 83, 101, 114, 118, 101, 114 };
                var SignBytes = socket.ReceivePack();
                if (!_rsa.VerifyData(helloBytes, sha256, SignBytes)) return false;
                _socket.SendPack(_rsa.Encrypt(_aes.Key, false));
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Authentication fail");
                return false;
            }
            finally
            {
                sha256.Dispose();
            }
        }

        private async void StartReceive(UserContext userContext)
        {
            try
            {
                var pipe = new Pipe();
                Task writing = FillPipeAsync(_socket, pipe.Writer);
                Task reading = ReadPipeAsync(pipe.Reader, userContext);
                await Task.WhenAll(reading, writing);

                if (_socket.Connected) _socket.Disconnect(true);
                DisConnect?.Invoke(userContext);
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
            
        }

        private async Task FillPipeAsync(Socket socket, PipeWriter writer)
        {
            while (true)
            {
                // Allocate at least 512 bytes from the PipeWriter.
                Memory<byte> memory = writer.GetMemory(512);
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
                catch (Exception)
                {
                    // Log.Information(ex.Message);
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
                Receive?.Invoke(userContext, AesUtil.Decrypt(pack.ToArray(), userContext.AesDecryptor));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        // public void SendPack(byte[] buffer)
        // {
        //     try
        //     {
        //         var encryptData = AesUtil.Encrypt(buffer, _aesEncryptor);
        //         _socket.SendPack(encryptData);

        //     }
        //     catch (SocketException socketException)
        //     {
        //         Log.Information(socketException.Message);
        //         Disconnect();
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex, "send fail");
        //     }
        // }

        // public async Task SendPackAsync(byte[] buffer)
        // {
        //     try
        //     {
        //         using (NetworkStream stream = new NetworkStream(_socket))
        //         {
        //             await stream.WriteAsync(BitConverter.GetBytes((buffer.Length / 16) * 16 + 16), 0, 4);
        //             using (CryptoStream cryptoStream = new CryptoStream(stream, _aesEncryptor, CryptoStreamMode.Write))
        //             {
        //                 await cryptoStream.WriteAsync(buffer, 0, buffer.Length);
        //             }
        //         }
        //     }
        //     catch (SocketException socketException)
        //     {
        //         Log.Information(socketException.Message);
        //         Disconnect();
        //     }
        //     catch (Exception ex)
        //     {
        //         Log.Error(ex, "send fail");
        //     }
        // }

    }
}