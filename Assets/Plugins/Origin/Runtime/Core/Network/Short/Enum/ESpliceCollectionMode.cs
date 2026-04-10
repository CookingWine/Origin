namespace Origin.Network
{
    /// <summary>
    /// Splice 集合参数拼接模式
    /// </summary>
    public enum ESpliceCollectionMode:byte
    {
        /// <summary>
        /// 普通值模式
        /// 例如：?id=10001
        /// 如果传入的是集合，则默认按逗号拼接
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 集合逗号拼接
        /// 例如：?propId=10001,10002,10003
        /// </summary>
        CommaSeparated = 1,

        /// <summary>
        /// 集合重复 Key 拼接
        /// 例如：?propId=10001&propId=10002&propId=10003
        /// </summary>
        RepeatKey = 2
    }
}
