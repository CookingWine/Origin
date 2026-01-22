using System;
using OriginRuntime;
using System.Collections.Generic;

namespace RuntimeLogic
{
    /// <summary>
    /// 计时器模块
    /// </summary>
    internal sealed class TimerSystem:ITimerDriver, ISystemCore, IUpdateSystem
    {

        class Timer
        {
            public int TimerId { get; set; } = 0;

            public float CurrentTimer { get; set; } = 0;

            public float Time { get; set; } = 0;

            public bool IsLoop { get; set; } = false;

            public bool IsNeedRemove { get; set; } = false;

            public bool IsRunning { get; set; } = false;

            public bool IsUnscaled { get; set; } = false;

            public object[] Args { get; set; } = null;

            public TimerCallback Callback { get; set; }
        }
        public int Priority => 0;

        /// <summary>
        /// 当前计时器的id
        /// </summary>
        private int m_CurrentTimerID = 0;

        private readonly List<Timer> m_TimeList = new List<Timer>( );
        private readonly List<Timer> m_UnscaledTimerList = new List<Timer>( );
        private readonly List<int> m_CacheRemoveTimers = new List<int>( );
        private readonly List<int> m_CacheRemoveUnscaledTimers = new List<int>( );

        private readonly List<System.Timers.Timer> m_Ticker = new List<System.Timers.Timer>( );
        public void InitSystem( )
        {
            m_CurrentTimerID = 0;
        }
        public void UpdateSystem(float elapseSeconds , float realElapseSeconds)
        {
            UpdateTimer(elapseSeconds);
            UpdateUnscaledTimer(realElapseSeconds);
        }
        public void ShutdownSystem( )
        {
            RemoveAllTimer( );
            foreach(var ticker in m_Ticker)
            {
                ticker?.Stop( );
            }
        }

        public System.Timers.Timer AddTimerSystem(bool autoReset , bool enabled , int interval , Action<object , System.Timers.ElapsedEventArgs> callback)
        {
            var timerTick = new System.Timers.Timer(interval);
            timerTick.AutoReset = autoReset;
            timerTick.Enabled = enabled;
            timerTick.Elapsed += new System.Timers.ElapsedEventHandler(callback);
            m_Ticker.Add(timerTick);
            return timerTick;
        }

        public int AddTimer(TimerCallback callback , float delay , bool isLoop = false , bool isUnscaled = false , params object[] args)
        {
            Timer timer = new Timer( )
            {
                TimerId = ++m_CurrentTimerID ,
                CurrentTimer = delay ,
                Callback = callback ,
                IsLoop = isLoop ,
                IsUnscaled = isUnscaled ,
                Args = args ,
                IsNeedRemove = false ,
                IsRunning = true
            };
            InsertTimer(timer);
            return timer.TimerId;
        }

        public float GetLeftTime(int timerId)
        {
            Timer timer = GetTimer(timerId);
            if(timer == null)
                return 0;
            return timer.CurrentTimer;
        }
        public void PauseTimer(int timerId)
        {
            Timer timer = GetTimer(timerId);
            if(timer != null)
                timer.IsRunning = true;
        }

        public void RemoveAllTimer( )
        {
            m_TimeList.Clear( );
            m_UnscaledTimerList.Clear( );
        }

        public void RemoveTimer(int timerId)
        {
            for(int i = 0, len = m_TimeList.Count; i < len; i++)
            {
                if(m_TimeList[i].TimerId == timerId)
                {
                    m_TimeList[i].IsNeedRemove = true;
                    return;
                }
            }
            for(int i = 0, len = m_UnscaledTimerList.Count; i < len; i++)
            {
                if(m_UnscaledTimerList[i].TimerId == timerId)
                {
                    m_UnscaledTimerList[i].IsNeedRemove = true;
                    return;
                }
            }
        }

        public void ResetTimer(int timerId , TimerCallback callback , float time , bool isLoop = false , bool isUnscaled = false)
        {
            Timer timer = GetTimer(timerId);
            if(timer != null)
            {
                timer.CurrentTimer = time;
                timer.Time = time;
                timer.IsLoop = isLoop;
                timer.Callback = callback;
                timer.IsNeedRemove = false;
                if(timer.IsUnscaled != isUnscaled)
                {
                    RemoveTimerImmediate(timerId);
                    timer.IsUnscaled = isUnscaled;
                    InsertTimer(timer);
                }
            }
        }

        public void ResetTimer(int timerId , float time , bool isLoop , bool isUnscaled)
        {
            Timer timer = GetTimer(timerId);
            if(timer != null)
            {
                timer.CurrentTimer = time;
                timer.Time = time;
                timer.IsLoop = isLoop;
                timer.IsNeedRemove = false;
                if(timer.IsUnscaled != isUnscaled)
                {
                    RemoveTimerImmediate(timerId);
                    timer.IsNeedRemove = isUnscaled;
                    InsertTimer(timer);
                }
            }
        }

        public void ResumeTimer(int timerId)
        {
            Timer timer = GetTimer(timerId);
            if(timer != null)
                timer.IsRunning = true;
        }

        public bool Running(int timerId)
        {
            return GetTimer(timerId) is { IsRunning: true };
        }

        private void InsertTimer(Timer timer)
        {
            bool isInsert = false;
            if(timer.IsUnscaled)
            {
                for(int i = 0, len = m_UnscaledTimerList.Count; i < len; i++)
                {
                    if(m_UnscaledTimerList[i].CurrentTimer > timer.CurrentTimer)
                    {
                        m_UnscaledTimerList.Insert(i , timer);
                        isInsert = true;
                        break;
                    }
                }
                if(!isInsert)
                    m_UnscaledTimerList.Add(timer);
            }
            else
            {
                for(int i = 0, len = m_TimeList.Count; i < len; i++)
                {
                    if(m_TimeList[i].CurrentTimer > timer.CurrentTimer)
                    {
                        m_TimeList.Insert(i , timer);
                        isInsert = true;
                        break;
                    }
                }
                if(!isInsert)
                    m_TimeList.Add(timer);
            }
        }

        private Timer GetTimer(int timerId)
        {
            for(int i = 0, len = m_TimeList.Count; i < len; i++)
                if(m_TimeList[i].TimerId == timerId)
                    return m_TimeList[i];

            for(int i = 0, len = m_UnscaledTimerList.Count; i < len; i++)
                if(m_UnscaledTimerList[i].TimerId == timerId)
                    return m_UnscaledTimerList[i];

            return null;
        }

        private void UpdateTimer(float elapseSeconds)
        {
            bool isLoopCall = false;
            for(int i = 0, len = m_TimeList.Count; i < len; i++)
            {
                Timer timer = m_TimeList[i];
                if(timer.IsNeedRemove)
                {
                    m_CacheRemoveTimers.Add(i);
                    continue;
                }
                if(!timer.IsRunning)
                    continue;
                timer.CurrentTimer -= elapseSeconds;
                if(timer.CurrentTimer <= 0)
                {
                    timer.Callback?.Invoke(timer.Args);
                    if(timer.IsLoop)
                    {
                        timer.CurrentTimer += timer.Time;
                        if(timer.CurrentTimer <= 0)
                            isLoopCall = true;
                    }
                    else
                    {
                        m_CacheRemoveTimers.Add(i);
                    }
                }
            }
            for(int i = m_CacheRemoveTimers.Count - 1; i >= 0; i--)
            {
                m_TimeList.RemoveAt(m_CacheRemoveTimers[i]);
                m_CacheRemoveTimers.RemoveAt(i);
            }
            if(isLoopCall)
                LoopCallInBadFrame( );
        }

        private void UpdateUnscaledTimer(float realElapseSeconds)
        {
            bool isLoopCall = false;
            for(int i = 0, len = m_UnscaledTimerList.Count; i < len; i++)
            {
                Timer timer = m_UnscaledTimerList[i];
                if(timer.IsNeedRemove)
                {
                    m_CacheRemoveUnscaledTimers.Add(i);
                    continue;
                }

                if(!timer.IsRunning)
                    continue;
                timer.CurrentTimer -= realElapseSeconds;
                if(timer.CurrentTimer <= 0)
                {
                    timer.Callback?.Invoke(timer.Args);
                    if(timer.IsLoop)
                    {
                        timer.CurrentTimer += timer.Time;
                        if(timer.CurrentTimer <= 0)
                            isLoopCall = true;
                    }
                    else
                    {
                        m_CacheRemoveUnscaledTimers.Add(i);
                    }
                }
            }

            for(int i = m_CacheRemoveUnscaledTimers.Count - 1; i >= 0; i--)
            {
                m_UnscaledTimerList.RemoveAt(m_CacheRemoveUnscaledTimers[i]);
                m_CacheRemoveUnscaledTimers.RemoveAt(i);
            }

            if(isLoopCall)
            {
                LoopCallUnscaledInBadFrame( );
            }
        }

        private void LoopCallInBadFrame( )
        {
            bool isLoopCall = false;
            for(int i = 0, len = m_TimeList.Count; i < len; i++)
            {
                Timer timer = m_TimeList[i];
                if(timer.IsLoop && timer.CurrentTimer <= 0)
                {
                    timer.Callback?.Invoke(timer.Args);
                    timer.CurrentTimer += timer.Time;
                    if(timer.CurrentTimer <= 0)
                    {
                        isLoopCall = true;
                    }
                }
            }
            if(isLoopCall)
                LoopCallInBadFrame( );
        }

        private void LoopCallUnscaledInBadFrame( )
        {
            bool isLoopCall = false;
            for(int i = 0, len = m_UnscaledTimerList.Count; i < len; i++)
            {
                Timer timer = m_UnscaledTimerList[i];
                if(timer.IsLoop && timer.CurrentTimer <= 0)
                {
                    timer.Callback?.Invoke(timer.Args);

                    timer.CurrentTimer += timer.Time;
                    if(timer.CurrentTimer <= 0)
                    {
                        isLoopCall = true;
                    }
                }
            }
            if(isLoopCall)
                LoopCallUnscaledInBadFrame( );
        }

        /// <summary>
        /// 立即移除计时器
        /// </summary>
        /// <param name="timerId">计时器ID</param>
        private void RemoveTimerImmediate(int timerId)
        {
            for(int i = 0, len = m_TimeList.Count; i < len; i++)
            {
                if(m_TimeList[i].TimerId == timerId)
                {
                    m_TimeList.RemoveAt(i);
                    return;
                }
            }
            for(int i = 0, len = m_UnscaledTimerList.Count; i < len; i++)
            {
                if(m_UnscaledTimerList[i].TimerId == timerId)
                {
                    m_UnscaledTimerList.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
