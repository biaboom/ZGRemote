using System;
using System.Collections.Generic;
using Serilog;
using ZGRemote.Common.Message;
using ZGRemote.Common.Networking;

namespace ZGRemote.Common.Processor
{
    
    public delegate void Excute(UserContext user, IMessage message);
    public abstract class HandleBase<T> : IDisposable where T : HandleBase<T>, new()
    {
        protected static Dictionary<UserContext, T> _userInstanceTable = new Dictionary<UserContext, T>();
        protected bool disposedValue;

        public UserContext UserContext { get; set; }

        public static T CreateInstance(UserContext user)
        {
            T userInstance = new T();
            userInstance.UserContext = user;
            _userInstanceTable.Add(user, userInstance);
            return userInstance;
        }

        public static void ReleaseInstance(UserContext user)
        {
            if(_userInstanceTable.TryGetValue(user, out T instance))
            {
                _userInstanceTable.Remove(user);
                instance.Dispose();
            }
        }

        public static bool TryGetInstance(UserContext user, out T instance)
        {
            return _userInstanceTable.TryGetValue(user, out instance);
        }
        
        
        public static void Excute(UserContext user, IMessage message)
        {
            throw new Exception("the Excute mothod must override");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    UserContext = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    //标记Handle能处理哪些IMessage
    public class CanProcessMessageAttribute : Attribute
    {
        public Type[] CanProcessMessage { get; set; }
        public CanProcessMessageAttribute(params Type[] types)
        {
            CanProcessMessage = types;
        }
    }
}