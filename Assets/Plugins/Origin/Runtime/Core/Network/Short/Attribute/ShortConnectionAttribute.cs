using System;
namespace Origin.Network
{
    /// <summary>
    /// 短链接请求特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class , Inherited = false , AllowMultiple = false)]
    public sealed class ShortConnectionAttribute:Attribute
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public EConnectionModle Method { get; }

        /// <summary>
        /// 是否废弃
        /// </summary>
        public bool Deprecated { get; }

        /// <summary>
        /// 响应对象类型
        /// </summary>
        public Type ResponseType { get; }

        /// <summary>
        /// 响应解析器类型
        /// </summary>
        public Type ParserType { get; }

        /// <summary>
        /// 短链接请求特性
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求方式</param>
        /// <param name="responseType">响应对象类型</param>
        /// <param name="parserType">响应解析器类型</param>
        /// <param name="deprecated">是否废弃</param>
        public ShortConnectionAttribute(string url , EConnectionModle method , Type responseType , Type parserType , bool deprecated = false)
        {
            Url = url;
            Method = method;
            ResponseType = responseType;
            ParserType = parserType;
            Deprecated = deprecated;
        }
    }
}
