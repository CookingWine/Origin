using System;
using YooAsset;
using OriginRuntime;
using OriginRuntime.Resource;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace RuntimeLogic.Resource
{
    /// <summary>
    /// 资源模块
    /// </summary>
    internal sealed class ResourceSystem:IResourceModule, ISystemCore, IUpdateSystem
    {
        public int Priority => 4;

        public string ApplicableVersion { get; private set; }

        public int InternalResourceVersion { get; private set; }

        public string PackageVersion { get; private set; }

        public EPlayMode GamePlayMode { get; private set; }

        public EncryptionType EncryptionType { get; private set; }

        public bool UpdatableWhilePlaying { get; private set; }

        public int DownloadingMaxNum { get; private set; }

        public int FailedTryAgain { get; private set; }

        public string DefaultPackageName { get; private set; }

        public long Milliseconds { get; private set; }

        public bool AutoUnloadBundleWhenUnused { get; private set; }

        public string HostServerURL { get; private set; }

        public string FallbackHostServerURL { get; private set; }

        public ELoadResWayWebGL LoadResWayWebGL { get; private set; }

        public float AssetAutoReleaseInterval { get; private set; }

        public int AssetCapacity { get; private set; }

        public float AssetExpireTime { get; private set; }

        public int AssetPriority { get; private set; }

        public ResourceDownloaderOperation Downloader { get; private set; }


        #region 

        private AsyncOperation m_AsyncOperation = null;

        /// <summary>
        /// 强制卸载未使用的资源
        /// </summary>
        private bool m_ForceUnloadUnusedAssets;
        /// <summary>
        /// 预卸载未使用的资产
        /// </summary>
        private bool m_PreorderUnloadUnusedAssets;

        /// <summary>
        /// 执行GC收集
        /// </summary>
        private bool m_PerformGCCollect;

        /// <summary>
        /// 无用资源释放的等待时长，以秒为单位
        /// </summary>
        private float m_LastUnloadUnusedAssetsOperationElapseSeconds;
        /// <summary>
        /// 无用资源释放的最小间隔时间，以秒为单位
        /// </summary>
        private float m_MinUnloadUnusedAssetsInterval;
        /// <summary>
        /// 无用资源释放的最大间隔时间，以秒为单位
        /// </summary>
        private float m_MaxUnloadUnusedAssetsInterval;
        /// <summary>
        /// 使用系统释放无用资源策略
        /// </summary>
        private bool m_UseSystemUnloadUnusedAssets;

        #endregion

        public void Initialize( )
        {

        }

        public UniTask<InitializationOperation> InitializePackage(string customPackageName , bool needInitMainFest = false)
        {
            throw new NotImplementedException( );
        }

        public void InitSystem( )
        {
            Debug.Log("Start load resource config");
            var resourceSetting = Resources.Load<RuntimeResourceSetting>("Origin/RuntimeResourceSetting");
            GamePlayMode = resourceSetting.GamePlayMode;
            EncryptionType = resourceSetting.EncryptionType;
            UpdatableWhilePlaying = resourceSetting.UpdatableWhilePlaying;
            DownloadingMaxNum = resourceSetting.DownloadingMaxNum;
            m_ForceUnloadUnusedAssets = resourceSetting.ForceUnloadUnusedAssets;
            m_PreorderUnloadUnusedAssets = resourceSetting.PreorderUnloadUnusedAssets;
            m_PerformGCCollect = resourceSetting.PerformGCCollect;
            m_LastUnloadUnusedAssetsOperationElapseSeconds = 0f;
            m_MinUnloadUnusedAssetsInterval = resourceSetting.MinUnloadUnusedAssetsInterval;
            m_MaxUnloadUnusedAssetsInterval = resourceSetting.MaxUnloadUnusedAssetsInterval;
            m_UseSystemUnloadUnusedAssets = resourceSetting.UseSystemUnloadUnusedAssets;
        }

        public void UpdateSystem(float elapseSeconds , float realElapseSeconds)
        {
            m_LastUnloadUnusedAssetsOperationElapseSeconds += Time.unscaledDeltaTime;
            if(m_AsyncOperation == null &&
                ( m_ForceUnloadUnusedAssets ||
                m_LastUnloadUnusedAssetsOperationElapseSeconds >= m_MaxUnloadUnusedAssetsInterval ||
                m_PreorderUnloadUnusedAssets && m_LastUnloadUnusedAssetsOperationElapseSeconds >= m_MinUnloadUnusedAssetsInterval ))
            {
                Log.Info("Unload unused assets...");
                m_ForceUnloadUnusedAssets = false;
                m_PreorderUnloadUnusedAssets = false;
                m_LastUnloadUnusedAssetsOperationElapseSeconds = 0f;
                m_AsyncOperation = Resources.UnloadUnusedAssets( );
                if(m_UseSystemUnloadUnusedAssets)
                {
                    UnloadUnusedAssets( );
                }
            }
            if(m_AsyncOperation is { isDone: true })
            {
                m_AsyncOperation = null;
                if(m_PerformGCCollect)
                {
                    Log.Info("GC.Collect..");
                    m_PerformGCCollect = false;
                    GC.Collect( );
                }
            }
        }

        public bool CheckLocationValid(string location , string packageName = "")
        {
            return true;
        }

        public AssetInfo GetAssetInfo(string location , string packageName = "")
        {
            throw new NotImplementedException( );
        }

        public AssetInfo[] GetAssetInfos(string resTag , string packageName = "")
        {
            throw new NotImplementedException( );
        }

        public AssetInfo[] GetAssetInfos(string[] tags , string packageName = "")
        {
            throw new NotImplementedException( );
        }

        public HasAssetResult HasAsset(string location , string packageName = "")
        {
            throw new NotImplementedException( );
        }
        public void SetForceUnloadUnusedAssetsAction(Action<bool> action)
        {

        }

        public void SetRemoteServicesUrl(string defaultHostServer , string fallbackHostServer)
        {

        }

        public void UnloadUnusedAssets( )
        {

        }

        public void OnLowMemory( )
        {
            Log.Warning("Low memory reported...");
            m_ForceUnloadUnusedAssets = true;
            m_PerformGCCollect = true;
        }


        public void ShutdownSystem( )
        {

        }
    }
}
