
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
                var gameobj = (UdonBehaviour)GetComponent(typeof(UdonBehaviour));
                if (gameobj == null || _imageDownloader == null)
                {
                    isLoading = false;
                    UseUpdateDownload = true;
                    return;
                }
                _imageDownloader.DownloadImage(_url, null, gameobj, null);
            }
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
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
        public override void OnImageLoadError(IVRCImageDownload result)
        {
            isLoading = false;
            if (_retryCount < retryCount)
            {
                _retryCount++;
                Debug.LogWarning($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} retrying {_retryCount}/{retryCount}");
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
                Debug.LogWarning($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} trying alt url");
                useAlt = true;
                _retryCount = 0;
                LoadUrl();
                return;
            }
            Debug.LogError($"UdonLab.UrlLoader.UrlImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} ");
            useAlt = false;
            _retryCount = 0;
        }
    }
}
