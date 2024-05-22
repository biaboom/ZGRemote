using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ZGRemote.Common.Util
{
    public class RsaCspBlobUtil
    {
        public byte[] GenerateRsaBlob(int keySize, bool includePrivateParameters)
        {
            var rsa = new RSACryptoServiceProvider(keySize);
            return rsa.ExportCspBlob(includePrivateParameters);
        }
    }
}
