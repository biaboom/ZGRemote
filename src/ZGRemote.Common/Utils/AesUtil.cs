using System.IO;
using System.Security.Cryptography;

namespace ZGRemote.Common.Utils
{
    public static class AesUtil
    {
        /// <summary>
        /// aes ecb pkcs7 32bitKey
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, byte[] Key)
        {
            try
            {
                byte[] result;
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
                    {
                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                csEncrypt.Write(data, 0, data.Length);
                            }
                            result = msEncrypt.ToArray();
                        }
                    }

                }
                return result;
            }
            catch
            {
                return null;
            }

        }
        public static bool Encrypt(byte[] data, Stream desStream, byte[] Key)
        {
            try
            {

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    using (var encryptor = aesAlg.CreateEncryptor())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(desStream, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(data, 0, data.Length);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static byte[] Encrypt(byte[] data, ICryptoTransform encryptor)
        {
            try
            {
                byte[] result;
                using (Aes aesAlg = Aes.Create())
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(data, 0, data.Length);
                        }
                        result = msEncrypt.ToArray();
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }

        }
        public static bool Encrypt(byte[] data, Stream desStream, ICryptoTransform encryptor)
        {
            try
            {

                using (Aes aesAlg = Aes.Create())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(desStream, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        public static byte[] Decrypt(byte[] data, byte[] Key)
        {
            try
            {
                byte[] result;
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                    {
                        using (MemoryStream msDecrypt = new MemoryStream())
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                            {
                                csDecrypt.Write(data, 0, data.Length);
                            }
                            result = msDecrypt.ToArray();
                        }
                    }

                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        public static bool Decrypt(byte[] data, Stream desStream, byte[] Key)
        {
            try
            {

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    using (var decryptor = aesAlg.CreateDecryptor())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(desStream, decryptor, CryptoStreamMode.Write))
                        {
                            csDecrypt.Write(data, 0, data.Length);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static byte[] Decrypt(byte[] data, ICryptoTransform decryptor)
        {
            try
            {
                byte[] result;
                using (Aes aesAlg = Aes.Create())
                {
                    using (MemoryStream msDecrypt = new MemoryStream())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                        {
                            csDecrypt.Write(data, 0, data.Length);
                        }
                        result = msDecrypt.ToArray();
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        public static bool Decrypt(byte[] data, Stream desStream, ICryptoTransform decryptor)
        {
            try
            {

                using (Aes aesAlg = Aes.Create())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(desStream, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(data, 0, data.Length);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}