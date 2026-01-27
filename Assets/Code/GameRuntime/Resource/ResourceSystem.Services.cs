using OriginRuntime;
using System;
using System.IO;
using UnityEngine;
using YooAsset;

namespace RuntimeLogic.Resource
{
    /// <summary>
    /// 资源文件偏移加载解密服务
    /// </summary>
    internal class FileOffsetDecryption:IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        DecryptResult IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo)
        {
            DecryptResult decryptResult = new DecryptResult( );
            decryptResult.ManagedStream = null;
            decryptResult.Result =
                AssetBundle.LoadFromFile(fileInfo.FileLoadPath , 0 , GetFileOffset( ));
            return decryptResult;
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        DecryptResult IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo)
        {
            DecryptResult decryptResult = new DecryptResult( );
            decryptResult.ManagedStream = null;
            decryptResult.CreateRequest =
                AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath , 0 , GetFileOffset( ));
            return decryptResult;
        }

        /// <summary>
        /// 后备方式获取解密的资源包对象
        /// </summary>
        DecryptResult IDecryptionServices.LoadAssetBundleFallback(DecryptFileInfo fileInfo)
        {
            return new DecryptResult( );
        }

        /// <summary>
        /// 获取解密的字节数据
        /// </summary>
        byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException( );
        }

        /// <summary>
        /// 获取解密的文本数据
        /// </summary>
        string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException( );
        }

        private static ulong GetFileOffset( )
        {
            return 32;
        }
    }

    /// <summary>
    /// 资源文件流加载解密类
    /// </summary>
    internal class FileStreamDecryption:IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        DecryptResult IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo)
        {
            BundleStream bundleStream =
                new BundleStream(fileInfo.FileLoadPath , FileMode.Open , FileAccess.Read , FileShare.Read);
            DecryptResult decryptResult = new DecryptResult( );
            decryptResult.ManagedStream = bundleStream;
            decryptResult.Result =
                AssetBundle.LoadFromStream(bundleStream , 0 , GetManagedReadBufferSize( ));
            return decryptResult;
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        DecryptResult IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo)
        {
            BundleStream bundleStream =
                new BundleStream(fileInfo.FileLoadPath , FileMode.Open , FileAccess.Read , FileShare.Read);
            DecryptResult decryptResult = new DecryptResult( );
            decryptResult.ManagedStream = bundleStream;
            decryptResult.CreateRequest =
                AssetBundle.LoadFromStreamAsync(bundleStream , 0 , GetManagedReadBufferSize( ));
            return decryptResult;
        }

        /// <summary>
        /// 后备方式获取解密的资源包对象
        /// </summary>
        DecryptResult IDecryptionServices.LoadAssetBundleFallback(DecryptFileInfo fileInfo)
        {
            return new DecryptResult( );
        }

        /// <summary>
        /// 获取解密的字节数据
        /// </summary>
        byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException( );
        }

        /// <summary>
        /// 获取解密的文本数据
        /// </summary>
        string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException( );
        }

        private static uint GetManagedReadBufferSize( )
        {
            return 1024;
        }
    }

    /// <summary>
    /// 资源文件解密流
    /// </summary>
    internal class BundleStream:FileStream
    {
        public const byte KEY = 64;

        public BundleStream(string path , FileMode mode , FileAccess access , FileShare share) : base(path , mode , access ,
            share)
        {
        }

        public BundleStream(string path , FileMode mode) : base(path , mode)
        {
        }

        public override int Read(byte[] array , int offset , int count)
        {
            var index = base.Read(array , offset , count);
            for(int i = 0; i < array.Length; i++)
            {
                array[i] ^= KEY;
            }
            return index;
        }
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    internal class RemoteServices:IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;
        public RemoteServices(string defaultHostServer , string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return Utility.Text.Format("{0}/{1}" , _defaultHostServer , fileName);
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return Utility.Text.Format("{0}/{1}" , _fallbackHostServer , fileName);
        }
    }

    /// <summary>
    /// 资源文件偏移加载解密类
    /// </summary>
    internal class FileOffsetWebDecryption:IWebDecryptionServices
    {
        public WebDecryptResult LoadAssetBundle(WebDecryptFileInfo fileInfo)
        {
            int offset = GetFileOffset( );
            byte[] decryptedData = new byte[fileInfo.FileData.Length - offset];
            Buffer.BlockCopy(fileInfo.FileData , offset , decryptedData , 0 , decryptedData.Length);
            // 从内存中加载AssetBundle
            WebDecryptResult decryptResult = new WebDecryptResult( );
            decryptResult.Result = AssetBundle.LoadFromMemory(decryptedData);
            return decryptResult;
        }

        private static int GetFileOffset( )
        {
            return 32;
        }
    }

    internal class FileStreamWebDecryption:IWebDecryptionServices
    {
        public WebDecryptResult LoadAssetBundle(WebDecryptFileInfo fileInfo)
        {
            // 优化：使用Buffer批量操作替代逐字节异或
            byte[] decryptedData = new byte[fileInfo.FileData.Length];
            Buffer.BlockCopy(fileInfo.FileData , 0 , decryptedData , 0 , fileInfo.FileData.Length);

            for(int i = 0; i < decryptedData.Length; i++)
            {
                decryptedData[i] ^= BundleStream.KEY;
            }

            WebDecryptResult decryptResult = new WebDecryptResult( );
            decryptResult.Result = AssetBundle.LoadFromMemory(decryptedData);
            return decryptResult;
        }
    }
}
