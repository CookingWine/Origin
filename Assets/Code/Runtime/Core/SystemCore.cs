namespace OriginRuntime
{
    /// <summary>
    /// 系统接口
    /// </summary>
    public interface ISystemCore
    {
        /// <summary>
        /// 系统优先级
        /// </summary>
        /// <remarks>优先级高的优先轮询，并且关闭操作会后进行</remarks>
        public int Priority { get; }

        /// <summary>
        /// 初始化系统
        /// </summary>
        public void InitSystem( );

        /// <summary>
        /// 关闭并清理系统
        /// </summary>
        public void ShutdownSystem( );
    }
    /// <summary>
    /// 系统轮询接口
    /// </summary>
    public interface IUpdateSystem
    {
        /// <summary>
        /// 系统轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void UpdateSystem(float elapseSeconds , float realElapseSeconds);
    }
}
