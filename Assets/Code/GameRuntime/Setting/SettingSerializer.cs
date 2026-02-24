using OriginRuntime;
namespace RuntimeLogic
{
    internal class SettingSerializer:GameFrameworkSerializer<SettingSystem>
    {
        private static readonly byte[] Header = new byte[] { (byte)'G' , (byte)'F' , (byte)'S' };

        /// <summary>
        /// 初始化默认游戏配置序列化器的新实例。
        /// </summary>
        public SettingSerializer( )
        {
        }

        /// <summary>
        /// 获取默认游戏配置头标识。
        /// </summary>
        /// <returns>默认游戏配置头标识。</returns>
        protected override byte[] GetHeader( )
        {
            return Header;
        }
    }
}
