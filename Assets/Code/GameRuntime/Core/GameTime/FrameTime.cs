namespace RuntimeLogic
{
    /// <summary>
    /// 普通帧时间（Update）
    /// 用于存储每帧（Update回调）的时间维度数据，包含缩放/非缩放的时间及帧计数信息
    /// </summary>
    public struct FrameTime
    {
        /// <summary>
        /// 带时间缩放的游戏运行总时间（对应Unity的Time.time）
        /// 受Time.timeScale影响，暂停时停止增加
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// 带时间缩放的上一帧到当前帧的时间间隔（对应Unity的Time.deltaTime）
        /// 受Time.timeScale影响，帧率变化会导致该值改变
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// 无时间缩放的上一帧到当前帧的时间间隔（对应Unity的Time.unscaledDeltaTime）
        /// 不受Time.timeScale影响，可用于暂停状态下的时间计算
        /// </summary>
        public float UnscaledDeltaTime { get; private set; }

        /// <summary>
        /// 无时间缩放的游戏运行总时间（对应Unity的Time.unscaledTime）
        /// 不受Time.timeScale影响，暂停时仍持续增加
        /// </summary>
        public float UnscaledTime { get; private set; }

        /// <summary>
        /// 游戏启动以来的总帧数（对应Unity的Time.frameCount）
        /// 每执行一次Update，该值加1
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// 内部采样方法，用于更新FrameTime的所有时间属性
        /// 该方法仅在内部调用，避免外部直接修改时间数据
        /// </summary>
        /// <param name="time">带缩放的总时间</param>
        /// <param name="deltaTime">带缩放的帧间隔时间</param>
        /// <param name="unscaledDeltaTime">无缩放的帧间隔时间</param>
        /// <param name="unscaledTime">无缩放的总时间</param>
        /// <param name="frameCount">总帧数</param>
        internal void Sample(float time , float deltaTime , float unscaledDeltaTime , float unscaledTime , int frameCount)
        {
            Time = time;
            DeltaTime = deltaTime;
            UnscaledDeltaTime = unscaledDeltaTime;
            UnscaledTime = unscaledTime;
            FrameCount = frameCount;
        }
    }
}
