using Obfuz;
using System;
using UnityEngine;
using OriginRuntime;
using RuntimeLogic.Resource;
using OriginRuntime.Resource;
using OriginRuntime.ObjectPool;
using RuntimeLogic.ObjectPool;
namespace RuntimeLogic
{
    /// <summary>
    /// 游戏初始化
    /// </summary>
    public sealed class GameInitialize:MonoBehaviour
    {
        #region Regarding Resources.Load resource path related

        private const string OBFUZ_STATIC_KEY = "Obfuz/defaultStaticSecretKey";
        private const string ORIGIN_HELPER_SETTING = "Origin/RuntimeConfigSetting";
        private const string ORIGIN_UI_ROOT_PATH = "Origin/OriginUI";

        #endregion

        /// <summary>
        /// 时间切片
        /// </summary>
        private TimeSlicing _gameTimeSlicing;

        /// <summary>
        /// 初始化加载静态密钥
        /// </summary>
        /// <remarks>初始化EncryptionService后被混淆的代码才能正常运行，因此尽可能地早地初始化它。</remarks>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeLoadStaticKey( )
        {
            EncryptionService<DefaultStaticEncryptionScope>.Encryptor = new Obfuz.EncryptionVM.GeneratedEncryptionVirtualMachine(Resources.Load<TextAsset>(OBFUZ_STATIC_KEY).bytes);

            CustomPlayerLoop.CreateCustomPlayerLoop( );
        }

        private void Awake( )
        {
            //初始化时间切片
            _gameTimeSlicing = new TimeSlicing( );
            //注入系统
            BindSystemArchitecture( );
            //加载基础配置
            var runtimeConfig = Resources.Load<RuntimeConfigSetting>(ORIGIN_HELPER_SETTING);
            //加载辅助器
            BuildingAuxiliaryTools(runtimeConfig);
            //设置引用池检查模式
            BuildingReferencePoolSetting(runtimeConfig);

            //构建循环周期
            BuildingCyclePeriod( );
            Application.lowMemory += OnLowMemory;
            DontDestroyOnLoad(this);
        }

        private void Start( )
        {
            //初始化资源
            ArchitectureCore.GetSystem<IResourceModule>( ).Initialize( );

            //构建加载UI根节点数据
            BuildingUIRootData( );

        }

        private void OnApplicationQuit( )
        {
            Application.lowMemory -= OnLowMemory;
            StopAllCoroutines( );
            ArchitectureCore.ShutdownArchitecture( );
            CustomPlayerLoop.UnCustomPlayerLoop( );
        }

        /// <summary>
        /// 注入系统架构
        /// </summary>
        private void BindSystemArchitecture( )
        {
            ArchitectureCore.BindSystemSingleton<IMonoBehaviourDriver>(mono => new MonoDriver( ));
            ArchitectureCore.BindSystemSingleton<ITimerDriver>(times => new TimerSystem( ));
            ArchitectureCore.BindSystemSingleton<IResourceModule>(resource => new ResourceSystem( ));
            ArchitectureCore.BindSystemSingleton<IObjectPoolManager>(pool => new ObjectPoolSystem( ));
            ArchitectureCore.BindSystemSingleton<IProcedureSystem>(procedure => new ProcedureSystem( ));
        }

        /// <summary>
        /// 构建辅助器
        /// </summary>
        /// <param name="helperSetting"></param>
        private void BuildingAuxiliaryTools(RuntimeConfigSetting helperSetting)
        {
            CheckRunningConfiguration(helperSetting);

            if(!string.IsNullOrEmpty(helperSetting.LogHelper))
            {
                GameFrameworkLog.SetLogHelper(CreateHelper<GameFrameworkLog.ILogHelper>(helperSetting.LogHelper));
            }

            if(!string.IsNullOrEmpty(helperSetting.JsonHelper))
            {
                Utility.Json.SetJsonHelper(CreateHelper<Utility.Json.IJsonHelper>(helperSetting.JsonHelper));
            }
            if(!string.IsNullOrEmpty(helperSetting.CompressionHelper))
            {
                Utility.Compression.SetCompressionHelper(CreateHelper<Utility.Compression.ICompressionHelper>(helperSetting.CompressionHelper));
            }

            if(!string.IsNullOrEmpty(helperSetting.VersionHelper))
            {
                OriginRuntime.Version.SetVersionHelper(CreateHelper<OriginRuntime.Version.IVersionHelper>(helperSetting.VersionHelper));
            }

        }

        /// <summary>
        /// 构建循环周期
        /// </summary>
        private void BuildingCyclePeriod( )
        {
            CustomPlayerLoop.OnCustomUpdate += ( ) =>
            {
                _gameTimeSlicing.BeginFrame( );
                //缓存一次快照,防止两次拷贝
                var frame = _gameTimeSlicing.Frame;
                ArchitectureCore.UpdateArchitecture(frame.DeltaTime , frame.UnscaledDeltaTime);
            };

            CustomPlayerLoop.OnCustomLateUpdate += ( ) =>
            {

            };
            CustomPlayerLoop.OnCustomFixedUpdate += ( ) =>
            {
                _gameTimeSlicing.BeginFixedFrame( );
            };
        }

        /// <summary>
        /// 构建UI界面数据
        /// </summary>
        private void BuildingUIRootData( )
        {
            GameObject uiRoot = Instantiate(Resources.Load<GameObject>(ORIGIN_UI_ROOT_PATH));
            uiRoot.name = "OriginUIRoot";
            uiRoot.transform.SetParent(this.transform , false);
        }

        /// <summary>
        /// 构建引用池设置
        /// </summary>
        /// <param name="runtimeConfig"></param>
        private void BuildingReferencePoolSetting(RuntimeConfigSetting runtimeConfig)
        {
            CheckRunningConfiguration(runtimeConfig);

            //设置引用是否开启强制检查
            ReferencePool.EnableStrictCheck = runtimeConfig.EnableReferenceStrictCheck switch
            {
                RuntimeConfigSetting.ReferenceStrictCheckType.AlwaysEnable => true,
                RuntimeConfigSetting.ReferenceStrictCheckType.OnlyEnableWhenDevelopment => Debug.isDebugBuild,
                RuntimeConfigSetting.ReferenceStrictCheckType.OnlyEnableInEditor => Application.isEditor,
                _ => false,
            };
            if(ReferencePool.EnableStrictCheck)
            {
                Log.Info("Strict checking is enabled for the Reference Pool. It will drastically affect the performance.");
            }
        }

        /// <summary>
        /// 创建辅助器
        /// </summary>
        /// <typeparam name="T">辅助器类型</typeparam>
        /// <param name="helperName">辅助器全名</param>
        /// <returns>辅助器</returns>
        private T CreateHelper<T>(string helperName)
        {
            if(string.IsNullOrEmpty(helperName))
                throw new GameFrameworkException("Helper name is empty");
            Type helperType = Utility.Assembly.GetType(helperName) ?? throw new GameFrameworkException(Utility.Text.Format("Can not find helper type '{0}'" , helperName));
            T helper = (T)Activator.CreateInstance(helperType);
            return helper == null
                ? throw new GameFrameworkException(Utility.Text.Format("Can not create helper instance '{0}'" , helperName))
                : helper;
        }

        private void CheckRunningConfiguration(RuntimeConfigSetting configSetting)
        {
            if(configSetting == null)
                throw new GameFrameworkException("RuntimeConfigSetting is null");
        }

        private void OnLowMemory( )
        {
            Log.Info("Low memory reported...");
            var objectPoolSystem = ArchitectureCore.GetSystem<IObjectPoolManager>( );
            objectPoolSystem?.ReleaseAllUnused( );
        }
    }
}
