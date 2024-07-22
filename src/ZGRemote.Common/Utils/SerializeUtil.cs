using System.IO;
using System.Security.Cryptography;
using ProtoBuf;

namespace ZGRemote.Common.Utils
{
    public static class SerializeUtil
    {
        public static byte[] Serialize<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<T>(stream, obj);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public static bool TrySerialize<T>(in T obj, out byte[] serializeData)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Serializer.Serialize<T>(stream, obj);
                    serializeData = stream.ToArray();
                    return true;
                }
            }
            catch
            {
                serializeData = null;
                return false;
            }
        }

        public static bool TryDeserialize<T>(in byte[] serializeData, out T obj)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(serializeData))
                {
                    obj = Serializer.Deserialize<T>(stream);
                    return true;
                }
            }
            catch
            {
                obj = default(T);
                return false;
            }
        }
    }
}