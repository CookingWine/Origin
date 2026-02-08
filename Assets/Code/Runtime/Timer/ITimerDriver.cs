namespace OriginRuntime
{
    /// <summary>
    /// 计时器回调委托
    /// </summary>
    /// <param name="args"></param>
    public delegate void TimerCallback(object[] args);

    /// <summary>
    /// 有关计时器的驱动接口
    /// </summary>
    public interface ITimerDriver
    {
        /// <summary>
        /// 添加一个基于服务器的计时器，在属性中的毫秒Interval数过后，该计时器在应用程序中引发Elapsed事件
        /// </summary>
        /// <param name="autoReset">设置一个布尔值，该值指示 Timer 是否应只引发一次 Elapsed 事件（(false) 或重复 (true)）。</param>
        /// <param name="enabled">设置一个值，该值指示 Timer 是否应引发 Elapsed 事件。</param>
        /// <param name="interval">设置引发 Elapsed 事件的间隔（以毫秒为单位）。</param>
        /// <param name="callback">达到间隔时发生。</param>
        /// <returns>基于服务器的计时器</returns>
        /// <remarks>更多信息请参考<see href="https://learn.microsoft.com/zh-cn/dotnet/api/system.timers.timer?view=net-8.0"/></remarks>
        System.Timers.Timer AddTimerSystem(bool autoReset , bool enabled , int interval , System.Action<object , System.Timers.ElapsedEventArgs> callback);

        /// <summary>
        /// 添加一个计时器
        /// </summary>
        /// <param name="delay">等待时间</param>
        /// <param name="callback">计时器回调</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="isUnscaled">是否受时间缩放影响</param>
        /// <param name="args">参数</param>
        /// <returns>计时器ID</returns>
        public int AddTimer(float delay , TimerCallback callback , bool isLoop = false , bool isUnscaled = false , params object[] args);

        /// <summary>
        /// 暂停计时器
        /// </summary>
        /// <param name="timerId">计时器ID</param>
        public void PauseTimer(int timerId);

        /// <summary>
        /// 恢复计时器
        /// </summary>
        /// <param name="timerId">计时器ID</param>
        public void ResumeTimer(int timerId);

        /// <summary>
        /// 计时器是否在运行中
        /// </summary>
        /// <param name="timerId">计时器ID</param>
        /// <returns>是否运行中</returns>
        public bool Running(int timerId);

        /// <summary>
        /// 获得计时器剩余时间。
        /// </summary>
        public float GetLeftTime(int timerId);

        /// <summary>
        /// 重置计时器。
        /// </summary>
        public void ResetTimer(int timerId , TimerCallback callback , float time , bool isLoop = false , bool isUnscaled = false);

        /// <summary>
        /// 重置计时器。
        /// </summary>
        public void ResetTimer(int timerId , float time , bool isLoop , bool isUnscaled);

        /// <summary>
        /// 移除计时器。
        /// </summary>
        /// <param name="timerId">计时器Id。</param>
        public void RemoveTimer(int timerId);

        /// <summary>
        /// 移除所有计时器。
        /// </summary>
        public void RemoveAllTimer( );


    }
}
