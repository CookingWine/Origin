using OriginRuntime;

namespace RuntimeLogic
{
    internal sealed class UISystem:ISystemCore, IUpdateSystem
    {
        public int Priority => 0;

        public void InitSystem( )
        {

        }

        public void UpdateSystem(float elapseSeconds , float realElapseSeconds)
        {

        }

        public void ShutdownSystem( )
        {

        }
    }
}
