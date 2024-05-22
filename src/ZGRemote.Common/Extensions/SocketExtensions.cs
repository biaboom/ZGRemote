using System.Text;
using System.Net.Sockets;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ZGRemote.Common.Extensions
{

    internal static class SocketExtensions
    {
        /// <summary>
        /// 发送 data 会自动加上4字节的包头为data的大小，配合ReceivePack使用
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public static void SendPack(this Socket socket, byte[] data)
        {
            // send header
            byte[] header = BitConverter.GetBytes(data.Length);
            // if (!BitConverter.IsLittleEndian) Array.Reverse(header); // 转换为小端法
            socket.SendAllBytes(header);

            // send body
            socket.SendAllBytes(data);
        }
        /// <summary>
        /// 发送 data 会自动加上4字节的包头为data的大小，配合ReceivePack使用。
        /// 本方法没有参数越界检测，offset和size请不要超过data的数据范围，否则可能造成接收端接收不到正确的数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <param name="offset">开始发送数据的缓冲区中的位置。</param>
        /// <param name="size">发送的数据大小</param>
        public static void SendPack(this Socket socket, byte[] data, int offset, int size)
        {
            // send header
            byte[] header = BitConverter.GetBytes(size);
            // if (!BitConverter.IsLittleEndian) Array.Reverse(header); // 转换为小端法
            socket.SendAllBytes(header);

            // send body
            socket.SendAllBytes(data, offset, size);
        }

        public static byte[] ReceivePack(this Socket socket)
        {
            byte[] header = new byte[sizeof(int)];
            int bodySize = 0;

            // receive header
            int bytesReceive = 0;
            while (bytesReceive < sizeof(int))
            {
                int receCount = socket.Receive(header, bytesReceive, sizeof(int) - bytesReceive, SocketFlags.None);
                if (receCount == 0) throw new SocketException((int)SocketError.HostDown);
                bytesReceive += receCount;
            }
            // if (!BitConverter.IsLittleEndian) Array.Reverse(header); // 转换为小端法
            bodySize = BitConverter.ToInt32(header, 0);

            // receive body
            bytesReceive = 0;
            byte[] body = new byte[bodySize];
            while (bytesReceive < bodySize)
            {
                int receCount = socket.Receive(body, bytesReceive, bodySize - bytesReceive, SocketFlags.None);
                if (receCount == 0) throw new SocketException((int)SocketError.HostDown);
                bytesReceive += receCount;
            }
            return body;
        }

        /// <summary>
        /// 发送所有buffer字节数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer">类型 Byte 的数组，其中包含要发送的数据。</param>
        public static void SendAllBytes(this Socket socket, byte[] buffer)
        {
            socket.SendAllBytes(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送所有指定字节数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer">类型 Byte 的数组，其中包含要发送的数据。</param>
        /// <param name="offset">开始发送数据的缓冲区中的位置。</param>
        /// <param name="size">要发送的字节数。</param>
        public static void SendAllBytes(this Socket socket, byte[] buffer, int offset, int size)
        {
            int sendCount = 0;
            while (sendCount < size)
            {
                sendCount += socket.Send(buffer, offset + sendCount, size - sendCount, SocketFlags.None);
            }
        }


        public static Task<int> ReceiveAsync(this Socket socket, Memory<byte> memory, SocketFlags socketFlags)
        {
            var arraySegment = GetArray(memory);
            return SocketTaskExtensions.ReceiveAsync(socket, arraySegment, socketFlags);
        }

        public static string GetString(this Encoding encoding, ReadOnlyMemory<byte> memory)
        {
            var arraySegment = GetArray(memory);
            return encoding.GetString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
        }

        private static ArraySegment<byte> GetArray(Memory<byte> memory)
        {
            return GetArray((ReadOnlyMemory<byte>)memory);
        }

        private static ArraySegment<byte> GetArray(ReadOnlyMemory<byte> memory)
        {
            if (!MemoryMarshal.TryGetArray(memory, out var result))
            {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }

            return result;
        }

        public static void KeepAlive(this Socket socket, int firstKeepAlivePacketTime = 40 * 1000, int keepAliveInterval = 40 * 1000)
        {
            // struct {
            // uint isActive;
            // uint firstKeepAlivePacketTime;
            // uint keepAliveInterval;
            // }
            
            byte[] keepAliveArr = new byte[]
            {
                1, 0, 0, 0, 
                (byte)(firstKeepAlivePacketTime & 255),
                (byte)(firstKeepAlivePacketTime >> 8 & 255),
                (byte)(firstKeepAlivePacketTime >> 16 & 255),
                (byte)(firstKeepAlivePacketTime >> 24 & 255),
                (byte)(keepAliveInterval & 255),
                (byte)(keepAliveInterval >> 8 & 255),
                (byte)(keepAliveInterval >> 16 & 255),
                (byte)(keepAliveInterval >> 24 & 255)
            };
            socket.IOControl(IOControlCode.KeepAliveValues, keepAliveArr, null);

            // int size = 4;
            // byte[] keepAliveArr = new byte[size * 3];
            // Buffer.BlockCopy(BitConverter.GetBytes((uint)1), 0, keepAliveArr, 0, size);
            // Buffer.BlockCopy(BitConverter.GetBytes((uint)firstKeepAlivePacketTime), 0, keepAliveArr, size, size);
            // Buffer.BlockCopy(BitConverter.GetBytes((uint)keepAliveInterval), 0, keepAliveArr, size * 2, size);

            // socket.IOControl(IOControlCode.KeepAliveValues, keepAliveArr, null);
        }
    }

}
