using System;
using UnityEngine;

namespace RuntimeLogic
{
    /// <summary>
    /// 游戏时间系统（帧级权威时间源）
    /// 只允许在 Update 中推进
    /// </summary>
    public sealed class GameTimeSystem
    {
        /// <summary>
        /// 当前帧时间快照
        /// </summary>
        public FrameTime Frame { get; private set; }

        /// <summary>
        /// 固定时间域（物理 / FixedTick）
        /// </summary>
        public FixedTime Fixed { get; private set; }

        public GameTimeSystem( )
        {
            Frame = new FrameTime( );
            Fixed = new FixedTime( );
        }

        /// <summary>
        /// Update 中调用：推进一帧时间
        /// </summary>
        public void BeginFrame( )
        {
            Frame.Sample(Time.time , Time.deltaTime , Time.unscaledDeltaTime , Time.unscaledTime , Time.frameCount);
        }

        /// <summary>
        /// FixedUpdate 中调用：推进固定时间
        /// </summary>
        public void BeginFixedFrame( )
        {
            Fixed.Sample(Time.fixedTime , Time.fixedDeltaTime);
        }
    }
}
