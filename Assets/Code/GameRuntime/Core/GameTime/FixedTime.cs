namespace RuntimeLogic
{
    /// <summary>
    /// 固定时间域（FixedUpdate）
    /// 用于存储固定帧（FixedUpdate回调）的时间维度数据，适配物理更新等固定步长逻辑
    /// FixedUpdate的执行间隔由Time.fixedDeltaTime配置，不受帧率波动影响
    /// </summary>
    public struct FixedTime
    {
        /// <summary>
        /// 带时间缩放的固定帧游戏运行总时间（对应Unity的Time.fixedTime）
        /// 受Time.timeScale影响，每次FixedUpdate执行时按固定步长增加
        /// </summary>
        public float Time { get; private set; }

        /// <summary>
        /// 固定帧的时间间隔（对应Unity的Time.fixedDeltaTime）
        /// 该值为固定常量（默认0.02秒），不受帧率/Time.timeScale影响，保证物理逻辑稳定
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// 内部采样方法，用于更新FixedTime的时间属性
        /// 该方法仅在内部调用，避免外部直接修改固定帧时间数据
        /// </summary>
        /// <param name="time">带缩放的固定帧总时间</param>
        /// <param name="deltaTime">固定帧间隔时间</param>
        internal void Sample(float time , float deltaTime)
        {
            Time = time;
            DeltaTime = deltaTime;
        }
    }
}
