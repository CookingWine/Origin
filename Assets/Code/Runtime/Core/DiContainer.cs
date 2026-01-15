using System;
using System.Collections.Generic;

namespace OriginRuntime
{
    /// <summary>
    /// 轻量级 IoC（控制反转）容器实现
    /// 核心能力：支持单例（Singleton）和瞬时（Transient）两种生命周期的服务注册与解析
    /// 设计目标：高性能、逻辑可控、无隐式的“魔法命名推断”，仅依赖显式注册的工厂方法/实例
    /// </summary>
    public sealed class DiContainer
    {
        /// <summary>
        /// 存储服务类型对应的工厂方法
        /// Key：服务的 Type 类型
        /// Value：接收 DiContainer 实例，返回服务实例的工厂方法
        /// </summary>
        private readonly Dictionary<Type , Func<DiContainer , object>> m_Factories = new( );

        /// <summary>
        /// 存储单例（Singleton）生命周期的服务实例
        /// Key：服务的 Type 类型
        /// Value：已创建的单例服务实例
        /// </summary>
        private readonly Dictionary<Type , object> m_Singletons = new( );

        /// <summary>
        /// 注册单例（Singleton）生命周期的服务实例
        /// </summary>
        /// <typeparam name="TService">服务接口/类型（泛型约束：引用类型）</typeparam>
        /// <param name="instance">已创建的服务实例</param>
        /// <exception cref="ArgumentNullException">当传入的实例为 null 时抛出</exception>
        public void RegisterSingleton<TService>(TService instance) where TService : class
        {
            var serviceType = typeof(TService);
            // 单例实例不允许为 null，直接抛出参数空异常
            m_Singletons[serviceType] = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        /// <summary>
        /// 注册单例（Singleton）生命周期的服务工厂方法
        /// 特点：首次解析时执行工厂方法创建实例，后续直接返回缓存的单例实例
        /// </summary>
        /// <typeparam name="TService">服务接口/类型（泛型约束：引用类型）</typeparam>
        /// <param name="factory">创建服务实例的工厂方法（接收 DiContainer 实例，支持解析依赖）</param>
        /// <exception cref="ArgumentNullException">当工厂方法为 null 时抛出</exception>
        public void RegisterSingleton<TService>(Func<DiContainer , TService> factory) where TService : class
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var serviceType = typeof(TService);
            // 注册单例工厂方法：先检查缓存，无缓存则创建并缓存
            m_Factories[serviceType] = container =>
            {
                // 优先从单例缓存中获取，避免重复创建
                if(m_Singletons.TryGetValue(serviceType , out var cachedInstance))
                {
                    return cachedInstance;
                }

                // 执行工厂方法创建实例，并缓存到单例字典中
                var createdInstance = factory(container);
                m_Singletons[serviceType] = createdInstance;
                return createdInstance;
            };
        }

        /// <summary>
        /// 注册瞬时（Transient）生命周期的服务工厂方法
        /// 特点：每次解析时都会执行工厂方法创建新实例，无缓存
        /// </summary>
        /// <typeparam name="TService">服务接口/类型（泛型约束：引用类型）</typeparam>
        /// <param name="factory">创建服务实例的工厂方法（接收 DiContainer 实例，支持解析依赖）</param>
        /// <exception cref="ArgumentNullException">当工厂方法为 null 时抛出</exception>
        public void RegisterTransient<TService>(Func<DiContainer , TService> factory) where TService : class
        {
            if(factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            // 注册瞬时工厂方法：每次解析都执行工厂方法，不缓存实例
            m_Factories[typeof(TService)] = container => factory(container);
        }

        /// <summary>
        /// 解析指定类型的服务实例
        /// 解析逻辑优先级：单例缓存 > 工厂方法 > 抛出未注册异常
        /// </summary>
        /// <typeparam name="T">要解析的服务接口/类型（泛型约束：引用类型）</typeparam>
        /// <returns>解析成功的服务实例</returns>
        /// <exception cref="InvalidOperationException">当服务类型未注册时抛出</exception>
        public T Resolve<T>( ) where T : class
        {
            var serviceType = typeof(T);

            // 第一步：检查单例缓存，存在则直接返回
            if(m_Singletons.TryGetValue(serviceType , out var cachedSingleton))
            {
                return (T)cachedSingleton;
            }

            // 第二步：检查工厂方法，存在则执行工厂方法创建实例
            if(m_Factories.TryGetValue(serviceType , out var factoryMethod))
            {
                return (T)factoryMethod(this);
            }

            // 第三步：未注册该类型，抛出异常
            throw new InvalidOperationException($"服务类型未注册：{serviceType.FullName}");
        }
    }
}
