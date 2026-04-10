using System;
namespace Origin.Network
{
    /// <summary>
    /// 列表类型反序列化值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property , Inherited = true , AllowMultiple = false)]
    public sealed class BodyRootAttribute:Attribute
    {

    }
}
