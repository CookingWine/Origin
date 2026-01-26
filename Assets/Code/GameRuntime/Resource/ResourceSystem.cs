using System;
using YooAsset;
using UnityEngine;
using OriginRuntime;
using OriginRuntime.Resource;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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
        private bool m_ForceUnloadUnusedAssets = false;
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

        #region internal

        /// <summary>
        /// 资源包映射列表
        /// </summary>
        private readonly Dictionary<string , ResourcePackage> m_ResourcePackageMaps = new Dictionary<string , ResourcePackage>( );

        /// <summary>
        /// 资源信息列表
        /// </summary>
        private readonly Dictionary<string , AssetInfo> m_AssetInfoMaps = new Dictionary<string , AssetInfo>( );

        /// <summary>
        /// 正在加载的资源列表
        /// </summary>
        private readonly HashSet<string> m_AssetLoadingList = new HashSet<string>( );

        #endregion


        public void InitSystem( )
        {
            Debug.Log("Start load resource config");
            var resourceSetting = Resources.Load<RuntimeResourceSetting>("Origin/RuntimeResourceSetting");
            GamePlayMode = resourceSetting.GamePlayMode;
            EncryptionType = resourceSetting.EncryptionType;
            DefaultPackageName = resourceSetting.DefaultPackageName;
            UpdatableWhilePlaying = resourceSetting.UpdatableWhilePlaying;
            DownloadingMaxNum = resourceSetting.DownloadingMaxNum;
            m_PreorderUnloadUnusedAssets = resourceSetting.PreorderUnloadUnusedAssets;
            m_PerformGCCollect = resourceSetting.PerformGCCollect;
            m_LastUnloadUnusedAssetsOperationElapseSeconds = 0f;
            m_MinUnloadUnusedAssetsInterval = resourceSetting.MinUnloadUnusedAssetsInterval;
            m_MaxUnloadUnusedAssetsInterval = resourceSetting.MaxUnloadUnusedAssetsInterval;
            m_UseSystemUnloadUnusedAssets = resourceSetting.UseSystemUnloadUnusedAssets;
            HostServerURL = resourceSetting.HostServerURL;
            FallbackHostServerURL = resourceSetting.FallbackHostServerURL;
        }
        public void Initialize( )
        {
            //初始化资源系统
            YooAssets.Initialize((YooAsset.ILogger)GameFrameworkLog.GetLogHelper( ));
            YooAssets.SetOperationSystemMaxTimeSlice(Milliseconds);
            ResourcePackage defaultPackage = YooAssets.TryGetPackage(DefaultPackageName);
            defaultPackage ??= YooAssets.GetPackage(DefaultPackageName);
            YooAssets.SetDefaultPackage(defaultPackage);

        }

        public async UniTask<InitializationOperation> InitializePackage(string customPackageName , bool needInitMainFest = false)
        {
            Log.Info(Utility.Text.Format("Current loading resource mode:{0}" , GamePlayMode));

            if(m_ResourcePackageMaps.TryGetValue(customPackageName , out var resourcePackage))
            {
                if(resourcePackage.InitializeStatus is EOperationStatus.Succeed)
                {
                    Log.Error(Utility.Text.Format("Resource package '{0}' has been initialized." , customPackageName));
                    return null;
                }
                else
                {
                    m_ResourcePackageMaps.Remove(customPackageName);
                }
            }
            //创建资源包裹
            var package = YooAssets.TryGetPackage(customPackageName);
            package ??= YooAssets.GetPackage(customPackageName);
            m_ResourcePackageMaps[customPackageName] = package;
            InitializationOperation initializationOperation = null;
            if(GamePlayMode == EPlayMode.EditorSimulateMode)
            {
                PackageInvokeBuildResult buildResul = EditorSimulateModeHelper.SimulateBuild(customPackageName);
                EditorSimulateModeParameters createParameters = new EditorSimulateModeParameters( )
                {
                    EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(buildResul.PackageRootDirectory) ,
                    AutoUnloadBundleWhenUnused = AutoUnloadBundleWhenUnused
                };
                initializationOperation = package.InitializeAsync(createParameters);
            }

            await initializationOperation.ToUniTask( );

            if(needInitMainFest)
            {

            }
            return initializationOperation;
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
            return string.IsNullOrEmpty(packageName) ? YooAssets.CheckLocationValid(location) : YooAssets.GetPackage(packageName).CheckLocationValid(location);
        }

        public AssetInfo GetAssetInfo(string location , string packageName = "")
        {
            if(string.IsNullOrEmpty(location))
                throw new GameFrameworkException("Asset name is invalid.");
            if(string.IsNullOrEmpty(packageName))
            {
                if(m_AssetInfoMaps.TryGetValue(location , out AssetInfo assetInfo))
                {
                    return assetInfo;
                }
                assetInfo = YooAssets.GetAssetInfo(location);
                m_AssetInfoMaps[location] = assetInfo;
                return assetInfo;
            }
            else
            {
                string key = Utility.Text.Format("{0}/{1}" , packageName , location);
                if(m_AssetInfoMaps.TryGetValue(key , out AssetInfo assetInfo))
                {
                    return assetInfo;
                }
                var package = YooAssets.GetPackage(packageName) ?? throw new GameFrameworkException(Utility.Text.Format("Resource package '{0}' is not exist." , packageName));
                assetInfo = package.GetAssetInfo(location);
                m_AssetInfoMaps[key] = assetInfo;
                return assetInfo;
            }
        }

        public AssetInfo[] GetAssetInfos(string resTag , string packageName = "")
        {
            return string.IsNullOrEmpty(packageName) ? YooAssets.GetAssetInfos(resTag) : YooAssets.GetPackage(packageName).GetAssetInfos(resTag);
        }

        public AssetInfo[] GetAssetInfos(string[] tags , string packageName = "")
        {
            return string.IsNullOrEmpty(packageName) ? YooAssets.GetAssetInfos(tags) : YooAssets.GetPackage(packageName).GetAssetInfos(tags);
        }

        public HasAssetResult HasAsset(string location , string packageName = "")
        {
            if(string.IsNullOrEmpty(location))
                throw new GameFrameworkException("Asset name is invalid.");
            AssetInfo assetInfo = GetAssetInfo(location , packageName);
            if(CheckLocationValid(location))
                return HasAssetResult.Valid;

            if(assetInfo == null)
                return HasAssetResult.NotExist;

            if(IsNeedDownloadFromRemote(assetInfo))
                return HasAssetResult.AssetOnline;

            return HasAssetResult.AssetOnDisk;
        }

        public void SetRemoteServicesUrl(string defaultHostServer , string fallbackHostServer)
        {
            HostServerURL = defaultHostServer;
            FallbackHostServerURL = fallbackHostServer;
        }

        public void UnloadUnusedAssets( )
        {
            foreach(var package in m_ResourcePackageMaps.Values)
            {
                if(package is { InitializeStatus: EOperationStatus.Succeed })
                {
                    package.UnloadUnusedAssetsAsync( );
                }
            }
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


        #region private

        /// <summary>
        /// 是否需要从远程下载资源
        /// </summary>
        /// <param name="assetInfo"></param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        private bool IsNeedDownloadFromRemote(AssetInfo assetInfo , string packageName = "")
        {
            return string.IsNullOrEmpty(packageName) ?
                YooAssets.IsNeedDownloadFromRemote(assetInfo) :
                YooAssets.GetPackage(packageName).IsNeedDownloadFromRemote(assetInfo);
        }
        #endregion
    }
}
