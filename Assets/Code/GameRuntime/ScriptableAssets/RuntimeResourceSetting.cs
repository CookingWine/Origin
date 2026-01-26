using OriginRuntime.Resource;
using UnityEngine;
using YooAsset;
namespace RuntimeLogic
{
    /// <summary>
    /// 运行时的资源配置
    /// </summary>
    [CreateAssetMenu(menuName = "Origin/Resource Config" , fileName = "RuntimeResourceSetting")]
    public class RuntimeResourceSetting:ScriptableObject
    {
        [SerializeField] private EPlayMode m_GamePlayMode;
        [SerializeField] private EncryptionType m_EncryptionType;
        [SerializeField] private bool m_UpdatableWhilePlaying;
        [SerializeField] private int m_DownloadingMaxNum;
        [SerializeField] private bool m_PreorderUnloadUnusedAssets = false;
        [SerializeField] private bool m_PerformGCCollect = false;
        [SerializeField] private float m_MinUnloadUnusedAssetsInterval = 30f;
        [SerializeField] private float m_MaxUnloadUnusedAssetsInterval = 60f;
        [SerializeField] private bool m_UseSystemUnloadUnusedAssets = true;
        [SerializeField] private string m_HostServerURL;
        [SerializeField] private string m_FallbackHostServerURL;
        [SerializeField] private string m_DefaultPackageName;

        /// <summary>
        ///  获取运行模式
        /// </summary>
        public EPlayMode GamePlayMode
        {
            get
            {
#if UNITY_EDITOR
                return m_GamePlayMode;
#else
                return EPlayMode.OfflinePlayMode;
#endif
            }
        }

        /// <summary>
        /// 资源加密方式。
        /// </summary>
        public EncryptionType EncryptionType => m_EncryptionType;
        /// <summary>
        /// 是否边玩边下载。
        /// </summary>
        public bool UpdatableWhilePlaying => m_UpdatableWhilePlaying;
        /// <summary>
        /// 同时下载的最大数目。
        /// </summary>
        public int DownloadingMaxNum => m_DownloadingMaxNum;
        /// <summary>
        /// 预卸载未使用的资产
        /// </summary>
        public bool PreorderUnloadUnusedAssets => m_PreorderUnloadUnusedAssets;
        /// <summary>
        /// 执行GC收集
        /// </summary>
        public bool PerformGCCollect => m_PerformGCCollect;
        /// <summary>
        /// 无用资源释放的最小间隔时间，以秒为单位
        /// </summary>
        public float MinUnloadUnusedAssetsInterval => m_MinUnloadUnusedAssetsInterval;
        /// <summary>
        /// 无用资源释放的最大间隔时间，以秒为单位
        /// </summary>
        public float MaxUnloadUnusedAssetsInterval => m_MaxUnloadUnusedAssetsInterval;
        /// <summary>
        /// 使用系统释放无用资源策略
        /// </summary>
        public bool UseSystemUnloadUnusedAssets => m_UseSystemUnloadUnusedAssets;

        /// <summary>
        /// 热更链接URL。
        /// </summary>
        public string HostServerURL => m_HostServerURL;

        /// <summary>
        /// 备用热更URL。
        /// </summary>
        public string FallbackHostServerURL => m_FallbackHostServerURL;

        /// <summary>
        /// 默认资源包名称。
        /// </summary>
        public string DefaultPackageName => m_DefaultPackageName;
    }
}
