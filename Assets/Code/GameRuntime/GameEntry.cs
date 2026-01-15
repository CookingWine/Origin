using UnityEngine;
using OriginRuntime;
namespace RuntimeLogic
{
    public class GameEntry:MonoBehaviour
    {
        private GameTimeSystem m_GameTimeSystem;

        private void Awake( )
        {
            m_GameTimeSystem = new GameTimeSystem( );
            m_GameTimeSystem.Initialize( );
            var dic = new DiContainer( );
            ArchitectureCore.InitializeArchitecture(dic);
        }
        private void Update( )
        {
            m_GameTimeSystem.BeginFrame( );

            ArchitectureCore.UpdateArchitecture(m_GameTimeSystem.Frame.DeltaTime , m_GameTimeSystem.Frame.UnscaledDeltaTime);
        }

        private void FixedUpdate( )
        {
            m_GameTimeSystem.BeginFixedFrame( );
        }
        private void LateUpdate( )
        {

        }

        private void OnApplicationQuit( )
        {
            StopAllCoroutines( );
            ArchitectureCore.ShutdownArchitecture( );
        }
    }
}
