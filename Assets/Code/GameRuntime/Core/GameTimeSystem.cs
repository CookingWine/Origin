namespace RuntimeLogic
{
    /// <summary>
    /// 游戏时间。
    /// </summary>
    /// <remarks>提供从Unity获取时间信息的接口。</remarks>
    public static class GameTimeSystem
    {
        /// <summary>
        /// 此帧开始时的时间（只读）。
        /// </summary>
        public static float Time { get; private set; }

        /// <summary>
        /// 从上一帧到当前帧的间隔（秒）（只读）。
        /// </summary>
        public static float DeltaTime { get; private set; }

        /// <summary>
        /// timeScale从上一帧到当前帧的独立时间间隔（以秒为单位）（只读）。
        /// </summary>
        public static float UnscaledDeltaTime { get; private set; }

        /// <summary>
        /// 执行物理和其他固定帧速率更新的时间间隔（以秒为单位）。
        /// <remarks>如MonoBehavior的MonoBehaviour.FixedUpdate。</remarks>
        /// </summary>
        public static float FixedDeltaTime { get; private set; }

        /// <summary>
        /// 自游戏开始以来的总帧数（只读）。
        /// </summary>
        public static float FrameCount { get; private set; }

        /// <summary>
        /// timeScale此帧的独立时间（只读）。这是自游戏开始以来的时间（以秒为单位）。
        /// </summary>
        public static float UnscaledTime { get; private set; }

        /// <summary>
        /// 采样一帧的时间。
        /// </summary>
        public static void StartFrame( )
        {
            Time = UnityEngine.Time.time;
            DeltaTime = UnityEngine.Time.deltaTime;
            UnscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
            FixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            FrameCount = UnityEngine.Time.frameCount;
            UnscaledTime = UnityEngine.Time.unscaledTime;
        }
    }
}
