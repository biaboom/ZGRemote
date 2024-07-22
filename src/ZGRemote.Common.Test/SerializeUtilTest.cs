using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using ProtoBuf;
using ZGRemote.Common.Utils;
namespace ZGRemote.Common.Test;

public class SerializeUtilTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test()
    {
        Student s = new Student();
        s.ID = 4396;
        s.name = "Timi";
        var data = SerializeUtil.Serialize(s);
        Student ns = SerializeUtil.Deserialize<Student>(data);
        Assert.That(ns.ID, Is.EqualTo(s.ID));
        Assert.That(ns.name, Is.EqualTo(s.name));
        Assert.IsTrue(SerializeUtil.TrySerialize(s, out byte[] serializeData));
        Assert.IsTrue(SerializeUtil.TryDeserialize(serializeData, out Student news));
        Assert.That(news.ID, Is.EqualTo(s.ID));
        Assert.That(news.name, Is.EqualTo(s.name));
    }

    [ProtoContract]
    class Student
    {
        [ProtoMember(1)]
        public int ID;
        [ProtoMember(2)]
        public string? name;
    }
}