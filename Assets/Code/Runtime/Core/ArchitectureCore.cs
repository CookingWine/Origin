using System;
using System.Collections.Generic;

namespace OriginRuntime
{
    /// <summary>
    /// 架构核心：系统生命周期 + 更新调度
    /// IoC 版本：系统实例由容器提供（显式注册），不再靠命名推断 + 反射创建
    /// </summary>
    public static class ArchitectureCore
    {
        internal const int DESIGN_SYSTEM_COUNT = 32;

        /// <summary>
        /// IoC 容器（框架根容器）
        /// </summary>
        private static DiContainer Container;

        /// <summary>
        /// 系统映射
        /// </summary>
        private static readonly Dictionary<Type , ISystemCore> s_SystemMaps = new Dictionary<Type , ISystemCore>(DESIGN_SYSTEM_COUNT);

        private static readonly LinkedList<ISystemCore> s_Systems = new LinkedList<ISystemCore>( );
        private static readonly LinkedList<ISystemCore> s_UpdateModules = new LinkedList<ISystemCore>( );
        private static readonly List<IUpdateSystem> s_UpdateSystems = new List<IUpdateSystem>(DESIGN_SYSTEM_COUNT);

        private static bool s_IsExecuteListDirty;

        public static int SystemCount => s_SystemMaps.Count;

        /// <summary>
        /// 初始化架构（必须先调用）
        /// </summary>
        public static void InitializeArchitecture(DiContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public static void UpdateArchitecture(float elapseSeconds , float realElapseSeconds)
        {
            if(s_IsExecuteListDirty)
            {
                s_IsExecuteListDirty = false;
                s_UpdateSystems.Clear( );

                foreach(var system in s_UpdateModules)
                {
                    // 这里如果不是 IUpdateSystem，属于开发期错误，直接抛更好（避免把 null 塞进去）
                    if(system is not IUpdateSystem up)
                        throw new GameFrameworkException($"System '{system.GetType( ).FullName}' in update list but not IUpdateSystem.");

                    s_UpdateSystems.Add(up);
                }
            }

            for(int i = 0; i < s_UpdateSystems.Count; i++)
            {
                s_UpdateSystems[i].UpdateSystem(elapseSeconds , realElapseSeconds);
            }
        }

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
            Container = null;

            Utility.Marshal.FreeCachedHGlobal( );
        }

        /// <summary>
        /// 获取系统（接口）
        /// 系统由 IoC 容器创建/提供，不再做 “Ixxx -> xxx” 推断
        /// </summary>
        public static T GetSystem<T>( ) where T : class
        {
            Type interfaceType = typeof(T);
            if(!interfaceType.IsInterface)
                throw new GameFrameworkException($"You must get system by interface, but '{interfaceType.FullName}' is not.");

            if(s_SystemMaps.TryGetValue(interfaceType , out var existing))
                return existing as T;

            if(Container == null)
                throw new GameFrameworkException("ArchitectureCore not initialized. Call InitializeArchitecture(container) first.");

            // 由容器创建：依赖注入发生在这里
            var created = Container.Resolve<T>( ) as ISystemCore ?? throw new GameFrameworkException($"Resolved '{interfaceType.FullName}', but it does not implement ISystemCore.");

            // 统一走 Register：保证排序、更新列表、Init 都一致
            RegisterSystemInternal(interfaceType , created);
            return created as T;
        }

        /// <summary>
        /// 显式注册系统实例（手动 new 的系统也允许注册进来）
        /// </summary>
        public static T RegisterSystem<T>(ISystemCore system) where T : class
        {
            Type interfaceType = typeof(T);
            if(!interfaceType.IsInterface)
                throw new GameFrameworkException($"System '{interfaceType.FullName}' is not interface.");

            RegisterSystemInternal(interfaceType , system);
            return system as T;
        }

        private static void RegisterSystemInternal(Type interfaceType , ISystemCore system)
        {
            s_SystemMaps[interfaceType] = system ?? throw new ArgumentNullException(nameof(system));
            RegisterUpdateSystem(system);
            system.InitSystem( );
        }

        private static void RegisterUpdateSystem(ISystemCore system)
        {
            // s_Systems 按 Priority 降序插入
            var current = s_Systems.First;
            while(current != null)
            {
                if(system.Priority > current.Value.Priority)
                    break;
                current = current.Next;
            }

            if(current != null) s_Systems.AddBefore(current , system);
            else s_Systems.AddLast(system);

            if(system is IUpdateSystem)
            {
                var currentUpdate = s_UpdateModules.First;
                while(currentUpdate != null)
                {
                    if(system.Priority > currentUpdate.Value.Priority)
                        break;
                    currentUpdate = currentUpdate.Next;
                }

                if(currentUpdate != null) s_UpdateModules.AddBefore(currentUpdate , system);
                else s_UpdateModules.AddLast(system);

                s_IsExecuteListDirty = true;
            }
        }
    }
}
