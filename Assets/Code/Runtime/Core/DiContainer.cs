using System;
using System.Collections.Generic;

namespace OriginRuntime
{
    /// <summary>
    /// 轻量级 IoC（控制反转）容器实现
    /// 只做依赖注入：注册/解析/生命周期（Singleton/Transient）
    /// 通过可选 Hook 把“服务创建/首次解析”事件通知给外部
    /// </summary>
    public sealed class DiContainer
    {
        private readonly Dictionary<Type , Func<DiContainer , object>> m_Factories;
        private readonly Dictionary<Type , object> m_Singletons;

        /// <summary>
        /// 可选：服务创建/首次可用通知（由 ArchitectureCore 注入）
        /// </summary>
        private readonly Action<Type , object> m_PostCreateHook;

        /// <summary>
        /// 避免同一 serviceType 重复通知（尤其是单例缓存/重复 Resolve）
        /// </summary>
        private readonly HashSet<Type> m_Notified;

        /// <summary>
        /// 创建Ioc容器
        /// </summary>
        /// <param name="hook">设置“服务已创建/首次解析完成”的回调</param>
        public DiContainer(Action<Type , object> hook)
        {
            m_Factories = new Dictionary<Type , Func<DiContainer , object>>( );
            m_Singletons = new Dictionary<Type , object>( );
            m_Notified = new HashSet<Type>( );
            // 不主动遍历现有单例通知：因为可能尚未初始化 ArchitectureCore 或者顺序不确定
            // 让 Resolve 或 RegisterSingleton(instance) 触发即可（更可控、更低成本）
            m_PostCreateHook = hook;
        }
        /// <summary>
        /// 创建通知
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="instance">实例</param>
        private void NotifyCreated(Type serviceType , object instance)
        {
            if(instance == null) return;
            if(m_PostCreateHook == null) return;
            if(m_Notified.Contains(serviceType)) return;

            m_Notified.Add(serviceType);
            m_PostCreateHook(serviceType , instance);
        }
        public void RegisterSingleton<TService>(Func<DiContainer , TService> factory) where TService : class
        {
            if(factory == null) throw new ArgumentNullException(nameof(factory));

            var serviceType = typeof(TService);
            m_Factories[serviceType] = container =>
            {
                if(m_Singletons.TryGetValue(serviceType , out var cachedInstance))
                {
                    // 可能是先注册实例、后设置 hook 的场景：这里也做一次通知兜底
                    NotifyCreated(serviceType , cachedInstance);
                    return cachedInstance;
                }

                var createdInstance = factory(container);
                m_Singletons[serviceType] = createdInstance ?? throw new InvalidOperationException($"Factory for '{serviceType.FullName}' returned null.");

                // 创建完成即通知
                NotifyCreated(serviceType , createdInstance);
                return createdInstance;
            };
        }

        public void RegisterTransient<TService>(Func<DiContainer , TService> factory) where TService : class
        {
            if(factory == null) throw new ArgumentNullException(nameof(factory));

            var serviceType = typeof(TService);
            m_Factories[serviceType] = container =>
            {
                var createdInstance = factory(container) ?? throw new InvalidOperationException($"Factory for '{serviceType.FullName}' returned null.");

                // Transient 每次都可能不同实例：
                // 但生命周期管理一般只关心 System（通常是单例）。这里仍然通知一次（按 serviceType 维度去重）
                NotifyCreated(serviceType , createdInstance);
                return createdInstance;
            };
        }

        public T Resolve<T>( ) where T : class
        {
            var serviceType = typeof(T);

            if(m_Singletons.TryGetValue(serviceType , out var cachedSingleton))
            {
                // 兜底：先注册/先创建、后设置 hook 或后第一次 resolve 时，确保会通知一次
                NotifyCreated(serviceType , cachedSingleton);
                return (T)cachedSingleton;
            }

            if(m_Factories.TryGetValue(serviceType , out var factoryMethod))
            {
                var created = factoryMethod(this);
                NotifyCreated(serviceType , created);
                return (T)created;
            }

            throw new InvalidOperationException($"Service type not registered：{serviceType.FullName}");
        }
    }
}
