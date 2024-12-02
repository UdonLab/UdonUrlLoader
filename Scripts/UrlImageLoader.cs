
using System;
using Sonic853.Udon.ArrayPlus;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.UrlLoader
{
    public class UrlImageLoader : UrlLoaderCore
    {
        public Texture2D content;
        VRCImageDownloader _imageDownloader;
        public Texture2D[] cacheContents;
        void Start()
        {
            _imageDownloader = new VRCImageDownloader();
            if (loadOnStart)
            {
                LoadUrl();
            }
        }
        public override void LoadUrl(bool reload = false)
        {
            if (!reload && cacheContent && UdonArrayPlus.IndexOf(cacheUrls, url, out var index) != -1)
            {
                SendFunction(udonSendFunction, sendCustomEvent, setVariableName, cacheContents[index]);
            }
            else
            {
                if (string.IsNullOrEmpty(url.ToString()))
                    return;
                isLoaded = false;
                _imageDownloader.DownloadImage(url, null, GetComponent<UdonBehaviour>(), null);
            }
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            isLoaded = true;
            _retryCount = 0;
            if (cacheContent)
            {
                var urli = UdonArrayPlus.IndexOf(cacheUrls, url);
                if (urli == -1)
                {
                    cacheUrls = UdonArrayPlus.Add(cacheUrls, url);
                    cacheContents = UdonArrayPlus.Add(cacheContents, result.Result);
                }
                else
                {
                    cacheContents[urli] = result.Result;
                }
            }
            content = result.Result;
            SendFunction(udonSendFunction, sendCustomEvent, setVariableName, content);
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {
            isLoaded = true;
            if (_retryCount < retryCount)
            {
                _retryCount++;
                Debug.LogWarning($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} retrying {_retryCount}/{retryCount}");
                LoadUrl();
                return;
            }
            Debug.LogError($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} ");
            _retryCount = 0;
        }
    }
}
