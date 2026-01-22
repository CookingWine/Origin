using Obfuz;
using System;
using UnityEngine;
using OriginRuntime;
using RuntimeLogic.Resource;
using OriginRuntime.Resource;
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

        #endregion

        /// <summary>
        /// 时间切片
        /// </summary>
        private TimeSlicing _gameTimeSlicing;

        private RuntimeConfigSetting _gameBaseConfigSetting;

        /// <summary>
        /// 初始化加载静态密钥
        /// </summary>
        /// <remarks>初始化EncryptionService后被混淆的代码才能正常运行，因此尽可能地早地初始化它。</remarks>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeLoadStaticKey( )
        {
            EncryptionService<DefaultStaticEncryptionScope>.Encryptor = new Obfuz.EncryptionVM.GeneratedEncryptionVirtualMachine(Resources.Load<TextAsset>(OBFUZ_STATIC_KEY).bytes);
        }

        private void Awake( )
        {
            //初始化时间切片
            _gameTimeSlicing = new TimeSlicing( );
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch( );
            stopwatch.Start( );
            //注入系统
            BindSystemArchitecture( );
            _gameBaseConfigSetting = Resources.Load<RuntimeConfigSetting>(ORIGIN_HELPER_SETTING);
            //加载辅助器
            BuildingAuxiliaryTools(_gameBaseConfigSetting);

            stopwatch.Stop( );
            Log.Info($"{stopwatch.ElapsedMilliseconds}ms");
            DontDestroyOnLoad(this);
        }

        private void Start( )
        {
            Log.Info("Create ui root object");
        }

        private void Update( )
        {
            _gameTimeSlicing.BeginFrame( );
            //缓存一次快照,防止两次拷贝
            var frame = _gameTimeSlicing.Frame;
            ArchitectureCore.UpdateArchitecture(frame.DeltaTime , frame.UnscaledDeltaTime);
        }

        private void FixedUpdate( )
        {
            _gameTimeSlicing.BeginFixedFrame( );
        }

        private void OnApplicationQuit( )
        {
            StopAllCoroutines( );
            ArchitectureCore.ShutdownArchitecture( );
        }

        /// <summary>
        /// 注入系统架构
        /// </summary>
        private void BindSystemArchitecture( )
        {
            ArchitectureCore.BindSystemSingleton<IMonoBehaviourDriver>(mono => new MonoDriver( ));
            ArchitectureCore.BindSystemSingleton<ITimerDriver>(times => new TimerSystem( ));
            ArchitectureCore.BindSystemSingleton<IResourceModule>(resource => new ResourceSystem( ));
        }

        /// <summary>
        /// 构建辅助器
        /// </summary>
        /// <param name="helperSetting"></param>
        private void BuildingAuxiliaryTools(RuntimeConfigSetting helperSetting)
        {
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

            }

            if(!string.IsNullOrEmpty(helperSetting.VersionHelper))
            {

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
    }
}
