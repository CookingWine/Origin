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
        private GameTimeSystem _gameTimeSystem;

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
            _gameTimeSystem = new GameTimeSystem( );


            DontDestroyOnLoad(this);
        }

        private void Update( )
        {
            _gameTimeSystem.BeginFrame( );
            ArchitectureCore.UpdateArchitecture(_gameTimeSystem.Frame.DeltaTime , _gameTimeSystem.Frame.UnscaledDeltaTime);
        }

        private void FixedUpdate( )
        {
            _gameTimeSystem.BeginFixedFrame( );
        }

        private void OnDestroy( )
        {

        }

        private void OnApplicationQuit( )
        {
            StopAllCoroutines( );
            ArchitectureCore.ShutdownArchitecture( );
        }
    }
}
