using OriginRuntime;

namespace RuntimeLogic
{
    /// <summary>
    /// 计时器模块
    /// </summary>
    internal class TimerSystem:ITimerDriver, ISystemCore, IUpdateSystem
    {
        public int Priority => 0;

        public void InitSystem( )
        {

        }
        public void UpdateSystem(float elapseSeconds , float realElapseSeconds)
        {
            UnityEngine.Debug.Log($"逻辑时间:{elapseSeconds}真实时间:{realElapseSeconds}");
        }
        public void ShutdownSystem( )
        {

        }

        public int AddTimer(TimerCallback callback , float delay , bool isLoop = false , bool isUnscaled = false , params object[] args)
        {
            throw new System.NotImplementedException( );
        }

        public float GetLeftTime(int timerId)
        {
            throw new System.NotImplementedException( );
        }
        public void PauseTimer(int timerId)
        {
            throw new System.NotImplementedException( );
        }

        public void RemoveAllTimer( )
        {
            throw new System.NotImplementedException( );
        }

        public void RemoveTimer(int timerId)
        {
            throw new System.NotImplementedException( );
        }

        public void ResetTimer(int timerId , TimerCallback callback , float time , bool isLoop = false , bool isUnscaled = false)
        {
            throw new System.NotImplementedException( );
        }

        public void ResetTimer(int timerId , float time , bool isLoop , bool isUnscaled)
        {
            throw new System.NotImplementedException( );
        }

        public void ResumeTimer(int timerId)
        {
            throw new System.NotImplementedException( );
        }

        public bool Running(int timerId)
        {
            throw new System.NotImplementedException( );
        }
    }
}
