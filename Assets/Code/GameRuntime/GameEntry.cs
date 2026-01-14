using UnityEngine;
using OriginRuntime;
namespace RuntimeLogic
{
    public class GameEntry:MonoBehaviour
    {
        private void Awake( )
        {
            GameTimeSystem.StartFrame( );
        }

        private void Update( )
        {
            GameTimeSystem.StartFrame( );
            ArchitectureCore.UpdateArchitecture(GameTimeSystem.DeltaTime , GameTimeSystem.UnscaledDeltaTime);
        }

        private void FixedUpdate( )
        {
            GameTimeSystem.StartFrame( );
        }
        private void LateUpdate( )
        {
            GameTimeSystem.StartFrame( );
        }

        private void OnApplicationQuit( )
        {
            StopAllCoroutines( );
        }
    }
}
