using System;
using System.Runtime.Serialization;
namespace Origin
{
    /// <summary>
    /// Origin异常类
    /// </summary>
    [Serializable]
    internal class OriginException:Exception
    {
        /// <summary>
        /// 初始化Origin异常类的新实例
        /// </summary>
        internal OriginException( ) : base( ) { }

        /// <summary>
        /// 使用指定错误消息初始化Origin异常类的新实例
        /// </summary>
        /// <param name="message">描述错误的消息</param>
        internal OriginException(string message) : base(message) { }

        /// <summary>
        /// 使用指定错误消息和对导致此异常的内部异常的引用来初始化Origin异常类的新实例
        /// </summary>
        /// <param name="message">解释异常原因的错误消息</param>
        /// <param name="innerException">导致当前异常的异常。如果 innerException 参数不为空引用，则在处理内部异常的 catch 块中引发当前异常</param>
        internal OriginException(string message , Exception innerException) : base(message , innerException) { }

        /// <summary>
        /// 使用序列化数据初始化Origin异常类的新实例
        /// </summary>
        /// <param name="info">存有有关所引发异常的序列化的对象数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        protected OriginException(SerializationInfo info , StreamingContext context) : base(info , context) { }
    }
}
