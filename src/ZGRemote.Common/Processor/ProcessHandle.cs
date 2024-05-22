using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ZGRemote.Common.Networking;

namespace ZGRemote.Common.Processor
{
    public static class ProcessHandle
    {
        // 所有HandleBase子类集合
        public static Type[] HandleList;

        // 提前扫描所有handle的CreateInstance方法，方便CreateAllHandleInstanceByUserContext使用。
        private static List<MethodInfo> HandleCreateInstanceMethodList;

        // 提前扫描所有handle的ReleaseInstance方法，方便ReleaseAllHandleInstanceByUserContext使用。
        private static List<MethodInfo> HandleReleaseInstanceMethodList;


        static ProcessHandle()
        {
            Init();
        }

        public static void Init()
        {
            if (HandleList == null)
            {
                var HandlbaseType = typeof(HandleBase<>);
                var test = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass);
                // types 为所有继承了HandleBase<>的子类，由于HandleBase是泛型类所以不能直接使用IsAssignableFrom和IsSubclassOf
                HandleList = (from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
                              where type.IsClass && !type.IsAbstract
                              let baseType = type.BaseType
                              where baseType != null
                                  && baseType.IsGenericType
                                  && baseType.GetGenericTypeDefinition() == HandlbaseType
                              select type).ToArray();
            }

            Type handleBaseType = typeof(HandleBase<>);

            if (HandleCreateInstanceMethodList == null)
            {
                HandleCreateInstanceMethodList = new List<MethodInfo>();
                // 提前扫描所有handle的CreateInstance方法，方便CreateAllHandleInstanceByUserContext使用。
                foreach (var handle in HandleList)
                {
                    Type handle_ = handleBaseType.MakeGenericType(handle); // 泛型基类需要MakeGenericType子类
                    var method = handle_.GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static);
                    if (method == null) continue;
                    HandleCreateInstanceMethodList.Add(method);
                }
            }

            if (HandleReleaseInstanceMethodList == null)
            {
                HandleReleaseInstanceMethodList = new List<MethodInfo>();
                // 提前扫描所有handle的ReleaseInstance方法，方便ReleaseAllHandleInstanceByUserContext使用。
                foreach (var handle in HandleList)
                {
                    Type handle_ = handleBaseType.MakeGenericType(handle);
                    var method = handle_.GetMethod("ReleaseInstance", BindingFlags.Public | BindingFlags.Static);
                    if (method == null) continue;
                    HandleReleaseInstanceMethodList.Add(method);
                }
            }
        }

        // 创建某个UserContext所有的HandleInstance
        public static void ReleaseAllHandleInstanceByUserContext(UserContext user)
        {
            if (user != null)
            {
                foreach (var action in HandleReleaseInstanceMethodList)
                {
                    action?.Invoke(null, new object[] { user });
                }
            }
        }

        // 释放某个UserContext所有的HandleInstance，一般在断开连接时使用。
        public static void CreateAllHandleInstanceByUserContext(UserContext user)
        {
            if (user != null)
            {
                foreach (var action in HandleCreateInstanceMethodList)
                {
                    action?.Invoke(null, new object[] { user });
                }
            }
        }
    }
}
