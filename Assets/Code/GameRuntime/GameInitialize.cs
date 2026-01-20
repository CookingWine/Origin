using Obfuz;
using OriginRuntime;
using UnityEngine;
namespace RuntimeLogic
{
    /// <summary>
    /// 游戏初始化
    /// </summary>
    public sealed class GameInitialize:MonoBehaviour
    {
        #region Regarding Resources.Load resource path related

        private const string OBFUZ_STATIC_KEY = "Obfuz/defaultStaticSecretKey";
        private const string ORIGIN_HELPER_SETTING = "Origin/HelperSetting";

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
        }

        private void Awake( )
        {
            //初始化时间切片
            _gameTimeSlicing = new TimeSlicing( );
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch( );
            stopwatch.Start( );
            //注入系统
            BindSystemArchitecture( );
            //加载辅助器
            BuildingAuxiliaryTools(Resources.Load<HelperSetting>(ORIGIN_HELPER_SETTING));

            stopwatch.Stop( );
            Debug.Log($"{stopwatch.ElapsedMilliseconds}ms");
            DontDestroyOnLoad(this);
        }

        private void Start( )
        {
            Debug.Log("Create ui root object");
        }

        private void Update( )
        {
            _gameTimeSlicing.BeginFrame( );
            ArchitectureCore.UpdateArchitecture(_gameTimeSlicing.Frame.DeltaTime , _gameTimeSlicing.Frame.UnscaledDeltaTime);
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
        }

        /// <summary>
        /// 构建辅助器
        /// </summary>
        /// <param name="helperSetting"></param>
        private void BuildingAuxiliaryTools(HelperSetting helperSetting)
        {
            Debug.Log(helperSetting == null);
        }
    }
}
