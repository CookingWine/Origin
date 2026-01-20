using UnityEngine;

namespace RuntimeLogic
{
    /// <summary>
    /// 游戏时间切片（帧级权威时间源）
    /// 只允许在 Update 中推进
    /// </summary>
    public sealed class TimeSlicing
    {
        private FrameTime _frame;
        private FixedTime _fixed;

        /// <summary>
        /// 当前帧时间快照（Update）
        /// 注意：返回的是 struct 的值拷贝（快照），外部只读消费
        /// </summary>
        public FrameTime Frame => _frame;

        /// <summary>
        /// 固定时间域（FixedUpdate）
        /// 注意：返回的是 struct 的值拷贝（快照），外部只读消费
        /// </summary>
        public FixedTime Fixed => _fixed;

        /// <summary>
        /// 初始化游戏时间切片
        /// </summary>
        public TimeSlicing( )
        {
            _frame = default;
            _fixed = default;
        }

        /// <summary>
        /// Update 中调用：推进一帧时间
        /// </summary>
        public void BeginFrame( )
        {
            _frame.Sample(Time.time , Time.deltaTime , Time.unscaledDeltaTime , Time.unscaledTime , Time.frameCount);
        }

        /// <summary>
        /// FixedUpdate 中调用：推进固定时间
        /// </summary>
        public void BeginFixedFrame( )
        {
            _fixed.Sample(Time.fixedTime , Time.fixedDeltaTime);
        }
    }
}
