using System;
using YooAsset;
using OriginRuntime;
using OriginRuntime.Resource;
using Cysharp.Threading.Tasks;
namespace RuntimeLogic.Resource
{
    /// <summary>
    /// 资源模块
    /// </summary>
    internal sealed class ResourceSystem:IResourceModule, ISystemCore
    {
        public int Priority => 0;

        public string ApplicableVersion => "";

        public int InternalResourceVersion => 1;

        public string PackageVersion { set => throw new NotImplementedException( ); }

        public EPlayMode GamePlayMode => throw new NotImplementedException( );

        public EncryptionType EncryptionType => throw new NotImplementedException( );

        public bool UpdatableWhilePlaying => throw new NotImplementedException( );

        public int DownloadingMaxNum => throw new NotImplementedException( );

        public int FailedTryAgain => throw new NotImplementedException( );

        public string DefaultPackageName => throw new NotImplementedException( );

        public long Milliseconds => throw new NotImplementedException( );

        public bool AutoUnloadBundleWhenUnused => throw new NotImplementedException( );

        public string HostServerURL => throw new NotImplementedException( );

        public string FallbackHostServerURL => throw new NotImplementedException( );

        public ELoadResWayWebGL LoadResWayWebGL => throw new NotImplementedException( );

        public float AssetAutoReleaseInterval => throw new NotImplementedException( );

        public int AssetCapacity => throw new NotImplementedException( );

        public float AssetExpireTime => throw new NotImplementedException( );

        public int AssetPriority => throw new NotImplementedException( );

        public ResourceDownloaderOperation Downloader => throw new NotImplementedException( );

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

        public void Initialize( )
        {
            
        }

        public UniTask<InitializationOperation> InitializePackage(string customPackageName , bool needInitMainFest = false)
        {
            throw new NotImplementedException( );
        }

        public void InitSystem( )
        {
            
        }

        public void OnLowMemory( )
        {
            throw new NotImplementedException( );
        }

        public void SetForceUnloadUnusedAssetsAction(Action<bool> action)
        {
            throw new NotImplementedException( );
        }

        public void SetRemoteServicesUrl(string defaultHostServer , string fallbackHostServer)
        {
            throw new NotImplementedException( );
        }

        public void ShutdownSystem( )
        {
            
        }
    }
}
