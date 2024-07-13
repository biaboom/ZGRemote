using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProtoBuf.Meta;
using Serilog;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;
using ZGRemote.Common.Util;

namespace ZGRemote.Common.Processor
{
    public static class MessageProcessor
    {
        /*
        初始化后传入IMessage获得能够处理IMessage的委托
        */
        private static Dictionary<Type, Excute> Message2DelegateHandlerTable;

        static MessageProcessor()
        {
            Init();
        }

        /// <summary>
        /// 从socket接收到IMessage调用此方法，可以根据Message2DelegateHandlerTable把消息传递给能够处理这条消息Handle
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public static void Process(UserContext user, MessageBase message)
        {
            Excute action;
            if (!Message2DelegateHandlerTable.TryGetValue(message.GetType(), out action))
            {
                action = HandlerBase.Excute;
            }
            action(user, message);
        }

        private static void Init()
        {
            var allType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes());
            
            // 扫描所有实现了接口IMessage的类 
            var IMessageType = typeof(MessageBase);
            var types = (from type in allType
                        where type.IsClass && !type.IsAbstract && IMessageType.IsAssignableFrom(type)
                        select type).ToArray();
            /*
            protobuf-net正常序列化时需要指定具体类型如
            Serialize<SubObject1>(SubObject1)
            Serialize<SubObject2>(SubObject2)
            添加下面语句之后可以 
            Serialize<IMessage>(SubObject1) 
            Serialize<IMessage>(SubObject2)
            直接序列化和 Deserialize<IMessage>(byte[])反序列对象
            而不用每次都指定具体的IMessage子类型。

            除了添加下面语句还可以手动在接口上添加
            [ProtoContract]
            [ProtoInclude(1, typeof(SubType))]
            public class IMessage{}
            手动添加每种子类型都得添加一次
            */
            for(int i = 0; i < types.Length; i++)
            {
                RuntimeTypeModel.Default[IMessageType].AddSubType(i + 1, types[i]);
            }

            Message2DelegateHandlerTable = new Dictionary<Type, Excute>();
            /*  
            扫描所有HandleBase的子类，添加到Message2DelegateHandlerTable
            key是CanProcessMessageAttribute.CanProcessMessage的值的成员
            vaule是Handlebase子类的静态方法Excute
            */
            if(DelegateHandlerProcessor.DelegateHandlerList == null) DelegateHandlerProcessor.Init();
            types = DelegateHandlerProcessor.DelegateHandlerList;
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<CanProcessMessageAttribute>();
                if (attr == null) continue;
                var method = type.GetMethod("Excute", BindingFlags.Public | BindingFlags.Static);
                if (method == null) continue;
                var action = Delegate.CreateDelegate(typeof(Excute), method) as Excute;
                foreach (var item in attr.CanProcessMessage)
                {
                    Message2DelegateHandlerTable.Add(item, action);
                }
            }
        }

        /// <summary>
        /// MessageBase序列化为二进制数据
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] Pack(MessageBase message)
        {
            return SerializeUtil.Serialize<MessageBase>(message);
        }

        /// <summary>
        /// 二进制数据反序列化为MessageBase
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MessageBase UnPack(byte[] data)
        {
            return SerializeUtil.Deserialize<MessageBase>(data);
        }


    }
}