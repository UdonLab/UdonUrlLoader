
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;


namespace UdonLab.UrlLoader
{
    public class UrlsStringLoader : UrlsLoaderCore
    {
        // public string[] contents;
        public string[] cacheContents;
        void Start()
        {
            if (urls.Length > 0)
                LoadUrl();
        }
        public override void LoadUrl()
        {
            if (isLoading) return;
            isLoading = true;
            VRCStringDownloader.LoadUrl(urls[0], GetComponent<UdonBehaviour>());
        }
        public override void PushUrl(VRCUrl url, UdonBehaviour udonSendFunction, string sendCustomEvent, string setVariableName)
        {
            if (cacheContent && UdonArrayPlus.Contains(cacheUrls, url))
            {
                var index = UdonArrayPlus.IndexOf(cacheUrls, url);
                SendFunction(udonSendFunction, sendCustomEvent, setVariableName, cacheContents[index]);
            }
            else
            {
                urls = UdonArrayPlus.Add(urls, url);
                // contents = UdonArrayPlus.Add(contents, "");
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
            // contents = UdonArrayPlus.RemoveAt(contents, 0);
            udonSendFunctions = UdonArrayPlus.RemoveAt(udonSendFunctions, 0);
            sendCustomEvents = UdonArrayPlus.RemoveAt(sendCustomEvents, 0);
            setVariableNames = UdonArrayPlus.RemoveAt(setVariableNames, 0);
        }
        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            isLoading = false;
            _retryCount = 0;
            if (cacheContent)
            {
                cacheUrls = UdonArrayPlus.Add(cacheUrls, urls[0]);
                cacheContents = UdonArrayPlus.Add(cacheContents, result.Result);
            }
            // contents.SetValue(result.Result, 0);
            // SendFunction(udonSendFunctions[0], sendCustomEvents[0], setVariableNames[0], contents[0]);
            SendFunction(udonSendFunctions[0], sendCustomEvents[0], setVariableNames[0], result.Result);
            DelUrl();
            if (urls.Length > 0)
                LoadUrl();
        }
        public override void OnStringLoadError(IVRCStringDownload result)
        {
            isLoading = false;
            if (_retryCount < retryCount)
            {
                _retryCount++;
                Debug.LogWarning($"UdonLab.UrlLoader.UrlsStringLoader: {result.ErrorCode} Could not load {result.Url} with error: {result.Error} retrying {_retryCount}/{retryCount}");
                LoadUrl();
                return;
            }
            Debug.LogError($"UdonLab.UrlLoader.UrlsStringLoader: {result.ErrorCode} Could not load {result.Url} with error: {result.Error}");
            DelUrl();
            _retryCount = 0;
            if (urls.Length > 0)
                LoadUrl();
        }
    }
}
