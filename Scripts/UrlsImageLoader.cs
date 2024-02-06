
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonLab.UrlLoader
{
    public class UrlsImageLoader : UrlsLoaderCore
    {
        public Texture2D[] contents;
        VRCImageDownloader _imageDownloader;
        public Texture2D[] cacheContents;
        void Start()
        {
            _imageDownloader = new VRCImageDownloader();
            if (urls.Length > 0)
                LoadUrl();
        }
        public override void LoadUrl()
        {
            if (isLoading) return;
            isLoading = true;
            _imageDownloader.DownloadImage(urls[0], null, GetComponent<UdonBehaviour>(), null);
        }
        public void PushUrl(VRCUrl url, UdonBehaviour udonSendFunction, string sendCustomEvent, string setVariableName)
        {
            if (cacheContent && UdonArrayPlus.Contains(cacheUrls, url))
            {
                var index = UdonArrayPlus.IndexOf(cacheUrls, url);
                SendFunction(udonSendFunction, sendCustomEvent, setVariableName, cacheContents[index]);
            }
            else
            {
                urls = UdonArrayPlus.Add(urls, url);
                contents = UdonArrayPlus.Add(contents, null);
                isLoaded = UdonArrayPlus.Add(isLoaded, false);
                udonSendFunctions = UdonArrayPlus.Add(udonSendFunctions, udonSendFunction);
                sendCustomEvents = UdonArrayPlus.Add(sendCustomEvents, sendCustomEvent);
                setVariableNames = UdonArrayPlus.Add(setVariableNames, setVariableName);
            }
            if (urls.Length > 0)
                LoadUrl();
        }
        public void DelUrl()
        {
            urls = UdonArrayPlus.RemoveAt(urls, 0);
            contents = UdonArrayPlus.RemoveAt(contents, 0);
            udonSendFunctions = UdonArrayPlus.RemoveAt(udonSendFunctions, 0);
            sendCustomEvents = UdonArrayPlus.RemoveAt(sendCustomEvents, 0);
            setVariableNames = UdonArrayPlus.RemoveAt(setVariableNames, 0);
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            isLoading = false;
            _retryCount = 0;
            if (cacheContent)
            {
                cacheUrls = UdonArrayPlus.Add(cacheUrls, urls[0]);
                cacheContents = UdonArrayPlus.Add(cacheContents, result.Result);
            }
            contents.SetValue(result.Result, 0);
            SendFunction(udonSendFunctions[0], sendCustomEvents[0], setVariableNames[0], contents[0]);
            DelUrl();
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
            Debug.LogError($"UdonLab.UrlLoader.UrlsImageLoader: {result.Error} Could not load {result.Url} : {result.ErrorMessage}");
            DelUrl();
            _retryCount = 0;
            if (urls.Length > 0)
                LoadUrl();
        }
    }
}
