using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using ZGRemote.Common.Utils;
namespace ZGRemote.Common.Test;

public class AesUtilTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test()
    {
        string source = "hello world";
        Aes aes = Aes.Create();
        var encryptData = AesUtil.Encrypt(Encoding.UTF8.GetBytes(source), aes.Key);
        var decryptData = AesUtil.Decrypt(encryptData, aes.Key);
        Assert.That(Encoding.UTF8.GetString(decryptData), Is.EqualTo(source));

        encryptData = AesUtil.Encrypt(Encoding.UTF8.GetBytes(source), aes.CreateEncryptor());
        decryptData = AesUtil.Decrypt(encryptData, aes.CreateDecryptor());
        Assert.That(Encoding.UTF8.GetString(decryptData), Is.EqualTo(source));

        MemoryStream enstream = new MemoryStream();
        AesUtil.Encrypt(Encoding.UTF8.GetBytes(source), enstream, aes.Key);
        MemoryStream deStream = new MemoryStream();
        AesUtil.Decrypt(enstream.ToArray(), deStream, aes.Key);
        Assert.That(Encoding.UTF8.GetString(deStream.ToArray()), Is.EqualTo(source));
        enstream.Dispose();
        deStream.Dispose();

        enstream = new MemoryStream();
        AesUtil.Encrypt(Encoding.UTF8.GetBytes(source), enstream, aes.CreateEncryptor());
        deStream = new MemoryStream();
        AesUtil.Decrypt(enstream.ToArray(), deStream, aes.CreateDecryptor());
        Assert.That(Encoding.UTF8.GetString(deStream.ToArray()), Is.EqualTo(source));
        enstream.Dispose();
        deStream.Dispose();
        aes.Dispose();
    }
}