
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonLab.UrlLoader
{
    public class UrlImageLoader : UrlLoaderCore
    {
        public Texture2D content;
        VRCImageDownloader _imageDownloader;
        void Start()
        {
            _imageDownloader = new VRCImageDownloader();
            if (loadOnStart)
            {
                LoadUrl();
            }
        }
        public override void LoadUrl()
        {
            if (string.IsNullOrEmpty(url.ToString()))
                return;
            isLoaded = false;
            _imageDownloader.DownloadImage(url, null, GetComponent<UdonBehaviour>(), null);
        }
        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            isLoaded = true;
            _retryCount = 0;
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
