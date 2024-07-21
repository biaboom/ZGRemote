using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ZGRemote.Common.Networking;

namespace ZGRemote.Common.Processor
{
    public static class HandlerProcessor
    {
        // 所有DelegateHandlerBase子类集合
        public static Type[] DelegateHandlerList;

        // 所有HandlerBase子类集合
        public static Type[] HandlerList;

        // 提前扫描所有handler的CreateInstance方法，方便CreateAllDelegateHandlerInstanceByUserContext使用。
        private static List<MethodInfo> DelegateHandlerCreateInstanceMethodList;

        // 提前扫描所有handler的ReleaseInstance方法，方便ReleaseAllDelegateHandlerInstanceByUserContext使用。
        private static List<MethodInfo> DelegateHandlerReleaseInstanceMethodList;


        static HandlerProcessor()
        {
            Init();
        }

        public static void Init()
        {
            var handlerBaseType = typeof(HandlerBase);
            if (HandlerList == null)
            {
                // types 为所有继承了DelegateHandlerBase<>的子类，由于DelegateHandlerBase是泛型类所以不能直接使用IsAssignableFrom和IsSubclassOf
                HandlerList = (from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
                                       where type.IsClass && !type.IsAbstract && type.IsSubclassOf(handlerBaseType)
                                       select type).ToArray();
            }

            var delegateHandlerBaseType = typeof(DelegateHandlerBase<>);
            if (DelegateHandlerList == null)
            {
                // 由于DelegateHandlerBase是泛型类所以不能直接使用IsAssignableFrom和IsSubclassOf
                DelegateHandlerList = (from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
                              where type.IsClass && !type.IsAbstract
                              let baseType = type.BaseType
                              where baseType != null
                                  && baseType.IsGenericType
                                  && baseType.GetGenericTypeDefinition() == delegateHandlerBaseType
                              select type).ToArray();
            }


            if (DelegateHandlerCreateInstanceMethodList == null)
            {
                DelegateHandlerCreateInstanceMethodList = new List<MethodInfo>();
                // 提前扫描所有DelegateHandler的CreateInstance方法，方便CreateAllDelegateHandlerInstanceByUserContext使用。
                foreach (var handle in DelegateHandlerList)
                {
                    Type handle_ = delegateHandlerBaseType.MakeGenericType(handle); // 泛型基类需要MakeGenericType子类
                    var method = handle_.GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static);
                    if (method == null) continue;
                    DelegateHandlerCreateInstanceMethodList.Add(method);
                }
            }

            if (DelegateHandlerReleaseInstanceMethodList == null)
            {
                DelegateHandlerReleaseInstanceMethodList = new List<MethodInfo>();
                // 提前扫描所有DelegateHandler的ReleaseInstance方法，方便ReleaseAllDelegateHandlerInstanceByUserContext使用。
                foreach (var handle in DelegateHandlerList)
                {
                    Type handle_ = delegateHandlerBaseType.MakeGenericType(handle);
                    var method = handle_.GetMethod("ReleaseInstance", BindingFlags.Public | BindingFlags.Static);
                    if (method == null) continue;
                    DelegateHandlerReleaseInstanceMethodList.Add(method);
                }
            }
        }

        // 释放某个UserContext所有的HandleInstance，一般在断开连接时使用。
        public static void ReleaseAllDelegateHandlerInstanceByUserContext(UserContext user)
        {
            if (user != null)
            {
                foreach (var action in DelegateHandlerReleaseInstanceMethodList)
                {
                    action?.Invoke(null, new object[] { user });
                }
            }
        }

        // 创建某个UserContext所有的HandleInstance
        public static void CreateAllDelegateHandlerInstanceByUserContext(UserContext user)
        {
            if (user != null)
            {
                foreach (var action in DelegateHandlerCreateInstanceMethodList)
                {
                    action?.Invoke(null, new object[] { user });
                }
            }
        }
    }
}
