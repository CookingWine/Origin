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
            EncryptionService<DefaultStaticEncryptionScope>.Encryptor = new Obfuz.EncryptionVM.GeneratedEncryptionVirtualMachine(Resources.Load<TextAsset>("Obfuz/defaultStaticSecretKey").bytes);
        }
        private void Awake( )
        {
            //初始化时间切片
            _gameTimeSlicing = new TimeSlicing( );

            //TODO:示例注册===>ArchitectureCore.BindSystemSingleton<interface>(c => new class:interface( ));

            DontDestroyOnLoad(this);
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
    }
}
