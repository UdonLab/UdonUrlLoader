
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
                UseUpdateDownload = true;
        }
        public override void LoadUrl(bool reload = false)
        {
            var _url = useAlt ? altUrl : url;
            if (!reload && cacheContent && UdonArrayPlus.IndexOf(cacheUrls, _url, out var index) != -1)
            {
                useAlt = false;
                SendFunction(udonSendFunction, sendCustomEvent, setVariableName, cacheContents[index]);
            }
            else
            {
                if (isLoading) return;
                isLoading = true;
                if (string.IsNullOrEmpty(_url.ToString()))
                    return;
                VRCStringDownloader.LoadUrl(_url, GetComponent<UdonBehaviour>());
            }
        }
        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            isLoading = false;
            _retryCount = 0;
            var _url = useAlt ? altUrl : url;
            if (cacheContent)
            {
                UdonArrayPlus.IndexOf(cacheUrls, _url, out var urli);
                if (urli == -1)
                {
                    UdonArrayPlus.Add(ref cacheUrls, _url);
                    UdonArrayPlus.Add(ref cacheContents, result.Result);
                }
                else
                {
                    cacheContents[urli] = result.Result;
                }
            }
            content = result.Result;
            useAlt = false;
            SendFunction(udonSendFunction, sendCustomEvent, setVariableName, content);
        }
        public override void OnStringLoadError(IVRCStringDownload result)
        {
            isLoading = false;
            if (_retryCount < retryCount)
            {
                _retryCount++;
                Debug.LogWarning($"UdonLab.UrlLoader.UrlStringLoader: {result.ErrorCode} Could not load {result.Url} with error: {result.Error} retrying {_retryCount}/{retryCount}");
                LoadUrl();
                return;
            }
            if (
                !useAlt
                && altUrl != null
                && !string.IsNullOrEmpty(altUrl.ToString())
                && altUrl.ToString() != url.ToString()
            )
            {
                Debug.LogWarning($"UdonLab.UrlLoader.UrlStringLoader: {result.Error} Could not load {result.Url} : {result.Error} trying alt url");
                useAlt = true;
                _retryCount = 0;
                LoadUrl();
                return;
            }
            Debug.LogError($"UdonLab.UrlLoader.UrlStringLoader: {result.ErrorCode} Could not load {result.Url} with error: {result.Error}");
            useAlt = false;
            _retryCount = 0;
        }
    }
}
