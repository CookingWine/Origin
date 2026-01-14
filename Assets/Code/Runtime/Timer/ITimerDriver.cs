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
        /// 添加一个计时器
        /// </summary>
        /// <param name="callback">计时器回调</param>
        /// <param name="delay">等待时间</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="isUnscaled">是否受时间缩放影响</param>
        /// <param name="args">参数</param>
        /// <returns>计时器ID</returns>
        public int AddTimer(TimerCallback callback , float delay , bool isLoop = false , bool isUnscaled = false , params object[] args);

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


    }
}
