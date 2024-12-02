
using System;
using Sonic853.Udon.ArrayPlus;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.UrlLoader
{
    public class UrlStringLoader : UrlLoaderCore
    {
        public string content;
        public string[] cacheContents;
        void Start()
        {
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
                VRCStringDownloader.LoadUrl(url, GetComponent<UdonBehaviour>());
            }
        }
        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            isLoaded = true;
            _retryCount = 0;
            content = result.Result;
            SendFunction(udonSendFunction, sendCustomEvent, setVariableName, content);
        }
        public override void OnStringLoadError(IVRCStringDownload result)
        {
            isLoaded = true;
            if (_retryCount < retryCount)
            {
                _retryCount++;
                Debug.LogWarning($"UdonLab.UrlLoader.UrlStringLoader: {result.ErrorCode} Could not load {result.Url} with error: {result.Error} retrying {_retryCount}/{retryCount}");
                LoadUrl();
                return;
            }
            Debug.LogError($"UdonLab.UrlLoader.UrlStringLoader: {result.ErrorCode} Could not load {result.Url} with error: {result.Error}");
            _retryCount = 0;
        }
    }
}
