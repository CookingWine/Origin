namespace OriginRuntime.Resource
{
    /// <summary>
    /// WebGL平台下，加载模式
    /// </summary>
    public enum ELoadResWayWebGL
    {
        /// <summary>
        /// 访问远程资源
        /// </summary>
        Remote,
        /// <summary>
        /// 跳过远程下载资源直接访问StreamingAssets
        /// </summary>
        StreamingAssets,
    }
}
