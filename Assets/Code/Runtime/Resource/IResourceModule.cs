using System;
using YooAsset;
using Cysharp.Threading.Tasks;
namespace OriginRuntime.Resource
{
    /// <summary>
    /// 资源模块
    /// </summary>
    public interface IResourceModule
    {
        /// <summary>
        /// 获取当前资源适用的版本号
        /// </summary>
        string ApplicableVersion { get; }

        /// <summary>
        /// 获取当前内部资源版本号
        /// </summary>
        int InternalResourceVersion { get; }

        /// <summary>
        /// 当前最新的包裹版本。
        /// </summary>
        string PackageVersion { set; }

        /// <summary>
        /// 获取运行模式
        /// </summary>
        EPlayMode GamePlayMode { get; }

        /// <summary>
        /// 资源加密方式。
        /// </summary>
        EncryptionType EncryptionType { get; }

        /// <summary>
        /// 是否边玩边下载。
        /// </summary>
        bool UpdatableWhilePlaying { get; }

        /// <summary>
        /// 同时下载的最大数目。
        /// </summary>
        int DownloadingMaxNum { get; }

        /// <summary>
        /// 失败重试最大数目。
        /// </summary>
        int FailedTryAgain { get; }

        /// <summary>
        /// 默认资源包名称。
        /// </summary>
        string DefaultPackageName { get; }

        /// <summary>
        /// 获取或设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）。
        /// </summary>
        long Milliseconds { get; }

        /// <summary>
        /// 自动释放资源引用计数为0的资源包
        /// </summary>
        bool AutoUnloadBundleWhenUnused { get; }

        /// <summary>
        /// 热更链接URL。
        /// </summary>
        string HostServerURL { get; }

        /// <summary>
        /// 备用热更URL。
        /// </summary>
        string FallbackHostServerURL { get; }

        /// <summary>
        /// WebGL平台加载本地资源/加载远程资源。
        /// </summary>
        ELoadResWayWebGL LoadResWayWebGL { get; }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float AssetAutoReleaseInterval { get; }

        /// <summary>
        /// 获取或设置资源对象池的容量。
        /// </summary>
        int AssetCapacity { get; }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数。
        /// </summary>
        float AssetExpireTime { get; }

        /// <summary>
        /// 获取或设置资源对象池的优先级。
        /// </summary>
        int AssetPriority { get; }

        /// <summary>
        /// 资源下载器，用于下载当前资源版本所有的资源包文件。
        /// </summary>
        ResourceDownloaderOperation Downloader { get; }

        /// <summary>
        /// 初始化接口
        /// </summary>
        void Initialize( );

        /// <summary>
        /// 初始化资源包
        /// </summary>
        /// <param name="customPackageName">资源包名称</param>
        /// <param name="needInitMainFest">是否需要直接初始化资源清单【单机模式下需要使用】</param>
        /// <returns>初始化资源包</returns>
        UniTask<InitializationOperation> InitializePackage(string customPackageName , bool needInitMainFest = false);

        /// <summary>
        /// 设置远程服务Url。
        /// </summary>
        /// <param name="defaultHostServer">默认远端资源地址。</param>
        /// <param name="fallbackHostServer">备用远端资源地址。</param>
        void SetRemoteServicesUrl(string defaultHostServer , string fallbackHostServer);

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="packageName">资源包名称</param>
        /// <returns>资源存在状态</returns>
        public HasAssetResult HasAsset(string location , string packageName = "");

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="packageName">资源包名称，不传则使用默认资源包</param>
        /// <returns></returns>
        bool CheckLocationValid(string location , string packageName = "");

        /// <summary>
        /// 获取资源信息列表。
        /// </summary>
        /// <param name="resTag">资源标签。</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源信息列表。</returns>
        AssetInfo[] GetAssetInfos(string resTag , string packageName = "");

        /// <summary>
        /// 获取资源信息列表。
        /// </summary>
        /// <param name="tags">资源标签列表。</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源信息列表。</returns>
        AssetInfo[] GetAssetInfos(string[] tags , string packageName = "");

        /// <summary>
        /// 获取资源信息。
        /// </summary>
        /// <param name="location">资源的定位地址。</param>
        /// <param name="packageName">指定资源包的名称。不传使用默认资源包</param>
        /// <returns>资源信息。</returns>
        AssetInfo GetAssetInfo(string location , string packageName = "");

        /// <summary>
        /// 低内存行为。
        /// </summary>
        void OnLowMemory( );

        /// <summary>
        /// 低内存回调保护。
        /// </summary>
        /// <param name="action">低内存行为。</param>
        void SetForceUnloadUnusedAssetsAction(Action<bool> action);
    }
}
