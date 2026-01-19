using System;
using System.Collections.Generic;

namespace OriginRuntime
{
    /// <summary>
    /// 架构核心：系统生命周期 + 更新调度
    /// 系统实例来源：DiContainer（依赖注入）
    /// 生命周期归属：ArchitectureCore（Init/Update/Shutdown + 排序）
    /// </summary>
    public static class ArchitectureCore
    {
        internal const int DESIGN_SYSTEM_COUNT = 32;

        private static DiContainer Container;

        private static readonly Dictionary<Type , ISystemCore> s_SystemMaps = new Dictionary<Type , ISystemCore>(DESIGN_SYSTEM_COUNT);

        private static readonly LinkedList<ISystemCore> s_Systems = new LinkedList<ISystemCore>( );
        private static readonly LinkedList<ISystemCore> s_UpdateModules = new LinkedList<ISystemCore>( );
        private static readonly List<IUpdateSystem> s_UpdateSystems = new List<IUpdateSystem>(DESIGN_SYSTEM_COUNT);

        private static bool s_IsExecuteListDirty;

        public static int SystemCount => s_SystemMaps.Count;

        static ArchitectureCore( )
        {
            // 绑定 DI 容器 hook：任何服务一旦被“注册实例 / 首次创建 / 首次 resolve”，都会通知到这里
            Container = new DiContainer(OnContainerServiceCreated);
        }
        private static void OnContainerServiceCreated(Type serviceType , object instance)
        {
            // ArchitectureCore 只关心：用“接口”注册的 ISystemCore
            if(serviceType == null || instance == null) return;
            if(!serviceType.IsInterface) return;

            if(instance is not ISystemCore system) return;

            // 已经注册过就跳过（避免重复 Init/重复入链表）
            if(s_SystemMaps.ContainsKey(serviceType)) return;

            RegisterSystemInternal(serviceType , system);
        }

        public static void UpdateArchitecture(float elapseSeconds , float realElapseSeconds)
        {
            if(s_IsExecuteListDirty)
            {
                s_IsExecuteListDirty = false;
                s_UpdateSystems.Clear( );

                foreach(var system in s_UpdateModules)
                {
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

        /// <summary>
        /// 关闭并清理架构
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

            // Container 也清掉（不再接受 Resolve/注册的生命周期自动纳管）
            Container = null;

            Utility.Marshal.FreeCachedHGlobal( );
        }

        /// <summary>
        /// 获取系统（接口）
        /// 系统由 IoC 容器创建/提供，创建后会自动纳入生命周期（hook 或这里都会兜底）
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

            // Resolve 时 hook 会自动 Register；这里再兜底一次（防止 hook 被覆盖/未设置）
            var resolved = Container.Resolve<T>( );
            if(resolved is not ISystemCore sys)
                throw new GameFrameworkException($"Resolved '{interfaceType.FullName}', but it does not implement ISystemCore.");

            if(!s_SystemMaps.ContainsKey(interfaceType))
                RegisterSystemInternal(interfaceType , sys);

            return resolved;
        }

        /// <summary>
        /// 通过 ArchitectureCore 绑定系统到容器（DI）+ 生命周期（Init/Update/Shutdown）
        /// </summary>
        public static void BindSystemSingleton<TSystem>(Func<DiContainer , TSystem> factory , bool instantiateNow = true) where TSystem : class
        {
            if(Container == null)
                throw new GameFrameworkException("ArchitectureCore not initialized. Call InitializeArchitecture(container) first.");

            Container.RegisterSingleton(factory);


            if(instantiateNow)
                GetSystem<TSystem>( );
        }

        /// <summary>
        /// 内部注册系统
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="system">系统</param>
        private static void RegisterSystemInternal(Type interfaceType , ISystemCore system)
        {
            s_SystemMaps[interfaceType] = system;

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
