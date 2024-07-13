using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;

namespace ZGRemote.Common.Processor
{
    
    public delegate void Excute(UserContext user, MessageBase message);
    public abstract class DelegateHandlerBase<T> where T : DelegateHandlerBase<T>, new()
    {
        private static ConcurrentDictionary<UserContext, T> userInstanceTable = new ConcurrentDictionary<UserContext, T>();

        public UserContext UserContext { get; set; }

        public static T GetOrCreateInstance(UserContext user)
        {
            if(userInstanceTable.TryGetValue(user, out T instance))
            {
                return instance;
            }
            return CreateInstance(user);
        }

        public static bool TryGetInstance(UserContext user, out T instance)
        {
            return userInstanceTable.TryGetValue(user, out instance);
        }

        public static T CreateInstance(UserContext user)
        {
            T userInstance = new T();
            userInstance.UserContext = user;
            if(!userInstanceTable.TryAdd(user, userInstance))
            {
                throw new Exception("CreateInstance failed");
            }
            return userInstance;
        }

        public static void ReleaseInstance(UserContext user)
        {
            if(userInstanceTable.TryRemove(user, out T instance))
            {
                instance.UserContext = null;
            }
        }

        public virtual void ReleaseInstance()
        {
            if(UserContext != null)
            {
                ReleaseInstance(UserContext);
            }
        }
 
        
        public static void Excute(UserContext user, MessageBase message)
        {
            throw new Exception("the Excute mothod must override");
        }

    }

    //标记DelegateHandler能处理哪些Message
    public class CanProcessMessageAttribute : Attribute
    {
        public Type[] CanProcessMessage { get; set; }
        public CanProcessMessageAttribute(params Type[] types)
        {
            CanProcessMessage = types;
        }
    }
}