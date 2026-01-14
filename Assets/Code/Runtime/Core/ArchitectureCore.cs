using System;
using System.Collections.Generic;

namespace OriginRuntime
{
    /// <summary>
    /// 架构核心模块管理类
    /// </summary>
    public static class ArchitectureCore
    {
        /// <summary>
        /// 设计系统的数量
        /// </summary>
        /// <remarks>减少内存分配与GCAlloc</remarks>
        internal const int DESIGN_SYSTEM_COUNT = 16;

        /// <summary>
        /// 系统类的映射表
        /// </summary>
        private static readonly Dictionary<Type , ISystemCore> s_SystemMaps = new Dictionary<Type , ISystemCore>(DESIGN_SYSTEM_COUNT);
        private static readonly LinkedList<ISystemCore> s_Systems = new LinkedList<ISystemCore>( );
        private static readonly LinkedList<ISystemCore> s_UpdateModules = new LinkedList<ISystemCore>( );
        private static readonly List<IUpdateSystem> s_UpdateSystems = new List<IUpdateSystem>(DESIGN_SYSTEM_COUNT);

        /// <summary>
        /// 是否需要更新执行列表
        /// </summary>
        private static bool s_IsExecuteListDirty;

        /// <summary>
        /// 系统轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public static void UpdateArchitecture(float elapseSeconds , float realElapseSeconds)
        {
            if(s_IsExecuteListDirty)
            {
                s_IsExecuteListDirty = false;
                s_UpdateSystems.Clear( );
                //构造执行列表
                foreach(var system in s_UpdateModules)
                {
                    s_UpdateSystems.Add(system as IUpdateSystem);
                }
            }
            for(int i = 0; i < s_UpdateSystems.Count; i++)
            {
                s_UpdateSystems[i].UpdateSystem(elapseSeconds , realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理所有系统模块
        /// </summary>
        public static void ShutdownArchitecture( )
        {
            for(LinkedListNode<ISystemCore> current = s_Systems.Last; current != null; current = current.Previous)
            {
                current.Value.ShutdownSystem( );
            }
            s_Systems.Clear( );
            s_SystemMaps.Clear( );
            s_UpdateModules.Clear( );
            s_UpdateSystems.Clear( );
            Utility.Marshal.FreeCachedHGlobal( );
        }

        /// <summary>
        /// 获取系统
        /// </summary>
        /// <typeparam name="T">要获取系统的类型</typeparam>
        /// <returns></returns>
        public static T GetSystem<T>( ) where T : class
        {
            Type interfaceType = typeof(T);
            if(!interfaceType.IsInterface)
            {
                throw new GameFrameworkException(Utility.Text.Format("You must get system by interface, but '{0}' is not" , interfaceType.FullName));
            }
            if(s_SystemMaps.TryGetValue(interfaceType , out ISystemCore systems))
            {
                return systems as T;
            }
            string systemName = Utility.Text.Format("{0}.{1}" , interfaceType.Namespace , interfaceType.Name[1..]);
            Type systemType = Type.GetType(systemName);
            return systemType == null
                ? throw new GameFrameworkException(Utility.Text.Format("Can not find system type '{0}'" , systemName))
                : GetSystem(interfaceType) as T;
        }

        /// <summary>
        /// 注册系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="system"></param>
        /// <returns></returns>
        public static T RegisterSystem<T>(ISystemCore system) where T : class
        {
            Type interfaceType = typeof(T);
            if(!interfaceType.IsInterface)
            {
                throw new GameFrameworkException(Utility.Text.Format("System '{0}' is not interface." , interfaceType.FullName));
            }
            s_SystemMaps[interfaceType] = system;
            RegisterUpdateSystem(system);
            return system as T;
        }

        /// <summary>
        /// 获取系统
        /// </summary>
        /// <param name="systemType">类型</param>
        /// <returns>系统</returns>
        private static ISystemCore GetSystem(Type systemType)
        {
            return s_SystemMaps.TryGetValue(systemType , out ISystemCore systems) ? systems : CreateSystemModule(systemType);
        }

        /// <summary>
        /// 创建系统模块
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        private static ISystemCore CreateSystemModule(Type systemType)
        {
            ISystemCore systemInstance = Activator.CreateInstance(systemType) as ISystemCore ?? throw new GameFrameworkException(Utility.Text.Format("Can not create module '{0}'." , systemType.FullName));
            s_SystemMaps[systemType] = systemInstance;
            RegisterUpdateSystem(systemInstance);
            return systemInstance;
        }

        /// <summary>
        /// 注册需要更新的系统模块
        /// </summary>
        /// <param name="system"></param>
        private static void RegisterUpdateSystem(ISystemCore system)
        {
            LinkedListNode<ISystemCore> current = s_Systems.First;
            while(current != null)
            {
                if(system.Priority > current.Value.Priority)
                {
                    break;
                }
                current = current.Next;
            }
            if(current != null)
            {
                s_Systems.AddBefore(current , system);
            }
            else
            {
                s_Systems.AddLast(system);
            }

            if(typeof(IUpdateSystem).IsInstanceOfType(system))
            {
                LinkedListNode<ISystemCore> currentUpdate = s_UpdateModules.First;
                while(currentUpdate != null)
                {
                    if(system.Priority > currentUpdate.Value.Priority)
                    {
                        break;
                    }
                    currentUpdate = currentUpdate.Next;
                }
                if(currentUpdate != null)
                {
                    s_UpdateModules.AddBefore(currentUpdate , system);
                }
                else
                {
                    s_UpdateModules.AddLast(system);
                }
                s_IsExecuteListDirty = true;
            }

            system.InitSystem( );
        }
    }
}
