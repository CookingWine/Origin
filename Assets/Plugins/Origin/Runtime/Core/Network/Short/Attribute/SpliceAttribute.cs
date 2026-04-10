using System;
namespace Origin.Network
{
    /// <summary>
    /// GET 请求参数拼接特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field , Inherited = true , AllowMultiple = false)]
    public sealed class SpliceAttribute:Attribute
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string KeyValue { get; }

        /// <summary>
        /// 集合参数拼接模式
        /// </summary>
        public ESpliceCollectionMode CollectionMode { get; }

        /// <summary>
        /// GET 请求参数拼接
        /// </summary>
        /// <param name="value">参数名</param>
        /// <param name="collectionMode">集合拼接模式</param>
        public SpliceAttribute(string value , ESpliceCollectionMode collectionMode = ESpliceCollectionMode.Normal)
        {
            KeyValue = value;
            CollectionMode = collectionMode;
        }
    }
}
