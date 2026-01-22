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


    }
}
