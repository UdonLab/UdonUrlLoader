
using System;
using Sonic853.Udon.ArrayPlus;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.UrlLoader
{
    public class UrlsImageLoader : UrlsLoaderCore
    {
        VRCImageDownloader _imageDownloader;
        public Texture2D[] cacheContents;
        void Start()
        {
            _imageDownloader = new VRCImageDownloader();
            if (urls.Length > 0)
                UseUpdateDownload = true;
        }
        public override void LoadUrl(bool reload = false)
        {
            var url = useAlt ? altUrls[0] : urls[0];
            if (!reload && cacheContent && UdonArrayPlus.IndexOf(cacheUrls, url, out var index) != -1)
            {
                SendFunction(udonSendFunctions[0], sendCustomEvents[0], setVariableNames[0], cacheContents[index]);
                DelUrl();
                if (urls.Length > 0)
                    LoadUrl();
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
                _imageDownloader.DownloadImage(url, null, gameobj, null);
            }
        }
        public override void PushUrl(VRCUrl url, VRCUrl altUrl, UdonBehaviour udonSendFunction, string sendCustomEvent, string setVariableName, bool reload = false)
        {
            if (!reload && cacheContent && UdonArrayPlus.IndexOf(cacheUrls, url, out var index) != -1)
            {
                SendFunction(udonSendFunction, sendCustomEvent, setVariableName, cacheContents[index]);
            }
            else
            {
                UdonArrayPlus.Add(ref urls, url);
                UdonArrayPlus.Add(ref altUrls, altUrl);
                UdonArrayPlus.Add(ref udonSendFunctions, udonSendFunction);
                UdonArrayPlus.Add(ref sendCustomEvents, sendCustomEvent);
                UdonArrayPlus.Add(ref setVariableNames, setVariableName);
                UdonArrayPlus.Add(ref needReloads, reload);
            }
            if (urls.Length > 0)
                UseUpdateDownload = true;
        }
        public void DelUrl() => DelUrl(0);
        public void DelUrl(int index = 0)
        {
            UdonArrayPlus.RemoveAt(ref urls, index);
            UdonArrayPlus.RemoveAt(ref altUrls, index);
            UdonArrayPlus.RemoveAt(ref udonSendFunctions, index);
            UdonArrayPlus.RemoveAt(ref sendCustomEvents, index);
            UdonArrayPlus.RemoveAt(ref setVariableNames, index);
            UdonArrayPlus.RemoveAt(ref needReloads, index);
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            isLoading = false;
            _retryCount = 0;
            var url = useAlt ? altUrls[0] : urls[0];
            if (cacheContent)
            {
                UdonArrayPlus.IndexOf(cacheUrls, url, out var urli);
                if (urli == -1)
                {
                    UdonArrayPlus.Add(ref cacheUrls, url);
                    UdonArrayPlus.Add(ref cacheContents, result.Result);
                }
                else
                {
                    cacheContents[urli] = result.Result;
                }
            }
            SendFunction(udonSendFunctions[0], sendCustomEvents[0], setVariableNames[0], result.Result);
            DelUrl();
            UdonArrayPlus.IndexOf(urls, url, out var _urli);
            while (_urli != -1 && !needReloads[_urli])
            {
                SendFunction(udonSendFunctions[_urli], sendCustomEvents[_urli], setVariableNames[_urli], result.Result);
                DelUrl(_urli);
                UdonArrayPlus.IndexOf(urls, url, out _urli);
            }
            useAlt = false;
            if (urls.Length > 0)
                LoadUrl();
        }
        public override void OnImageLoadError(IVRCImageDownload result)
        {
            isLoading = false;
            if (_retryCount < retryCount)
            {
                _retryCount++;
                Debug.LogWarning($"UdonLab.UrlLoader.UrlsImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} retrying {_retryCount}/{retryCount}");
                LoadUrl();
                return;
            }
            if (
                !useAlt
                && altUrls[0] != null
                && !string.IsNullOrEmpty(altUrls[0].ToString())
                && altUrls[0].ToString() != urls[0].ToString()
            )
            {
                Debug.LogWarning($"UdonLab.UrlLoader.UrlsImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage} trying alt url");
                useAlt = true;
                _retryCount = 0;
                LoadUrl();
                return;
            }
            Debug.LogError($"UdonLab.UrlLoader.UrlsImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage}");
            DelUrl();
            _retryCount = 0;
            useAlt = false;
            if (urls.Length > 0)
                LoadUrl();
        }
    }
}
