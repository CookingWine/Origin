using UnityEngine;

namespace RuntimeLogic
{
    /// <summary>
    /// 运行时得必要配置
    /// </summary>
    [CreateAssetMenu(menuName = "Origin/Runtime Config" , fileName = "RuntimeConfigSetting")]
    public class RuntimeConfigSetting:ScriptableObject
    {
        #region game helper
        [SerializeField] private string m_VersionHelper;
        [SerializeField] private string m_LogHelper;
        [SerializeField] private string m_CompressionHelper;
        [SerializeField] private string m_JsonHelper;
        [SerializeField]
        private ReferenceStrictCheckType m_EnableStrictCheck = ReferenceStrictCheckType.AlwaysEnable;

        /// <summary>
        /// 版本辅助器
        /// </summary>
        public string VersionHelper => m_VersionHelper;
        /// <summary>
        /// 日志辅助器
        /// </summary>
        public string LogHelper => m_LogHelper;
        /// <summary>
        /// 压缩辅助器
        /// </summary>
        public string CompressionHelper => m_CompressionHelper;
        /// <summary>
        /// JSON辅助器
        /// </summary>
        public string JsonHelper => m_JsonHelper;

        #endregion

        [SerializeField] private int m_FrameRate = 60;
        [SerializeField] private float m_GameSpeed = 1.0f;

        /// <summary>
        /// 游戏帧率
        /// </summary>
        public int FrameRate => m_FrameRate;

        /// <summary>
        /// 游戏速度
        /// </summary>
        public float GameSpeed
        {
            get
            {
#if UNITY_EDITOR
                return m_GameSpeed;
#else
                return 1;
#endif
            }
        }

        /// <summary>
        /// 获取是否开启强制检查。
        /// </summary>
        public ReferenceStrictCheckType EnableReferenceStrictCheck => m_EnableStrictCheck;



        #region enum

        /// <summary>
        /// 引用强制检查类型。
        /// </summary>
        public enum ReferenceStrictCheckType:byte
        {
            /// <summary>
            /// 总是启用。
            /// </summary>
            AlwaysEnable = 0,

            /// <summary>
            /// 仅在开发模式时启用。
            /// </summary>
            OnlyEnableWhenDevelopment,

            /// <summary>
            /// 仅在编辑器中启用。
            /// </summary>
            OnlyEnableInEditor,

            /// <summary>
            /// 总是禁用。
            /// </summary>
            AlwaysDisable,
        }

        #endregion
    }
}
