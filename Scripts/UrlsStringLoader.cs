
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
        public override void LoadUrl() => LoadUrl(needReloads[0]);
        public override void LoadUrl(bool reload = false)
        {
            if (!reload && cacheContent && UdonArrayPlus.IndexOf(cacheUrls, urls[0], out var index) != -1)
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
                VRCStringDownloader.LoadUrl(urls[0], GetComponent<UdonBehaviour>());
            }
        }
        public override void PushUrl(VRCUrl url, UdonBehaviour udonSendFunction, string sendCustomEvent, string setVariableName, bool reload = false)
        {
            if (!reload && cacheContent && UdonArrayPlus.IndexOf(cacheUrls, url, out var index) != -1)
            {
                SendFunction(udonSendFunction, sendCustomEvent, setVariableName, cacheContents[index]);
            }
            else
            {
                UdonArrayPlus.Add(ref urls, url);
                UdonArrayPlus.Add(ref isLoaded, false);
                UdonArrayPlus.Add(ref udonSendFunctions, udonSendFunction);
                UdonArrayPlus.Add(ref sendCustomEvents, sendCustomEvent);
                UdonArrayPlus.Add(ref setVariableNames, setVariableName);
                UdonArrayPlus.Add(ref needReloads, reload);
            }
            if (urls.Length > 0)
                LoadUrl();
        }
        public void DelUrl() => DelUrl(0);
        public void DelUrl(int index)
        {
            UdonArrayPlus.RemoveAt(ref urls, index);
            UdonArrayPlus.RemoveAt(ref udonSendFunctions, index);
            UdonArrayPlus.RemoveAt(ref sendCustomEvents, index);
            UdonArrayPlus.RemoveAt(ref setVariableNames, index);
            UdonArrayPlus.RemoveAt(ref needReloads, index);
        }
        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            isLoading = false;
            _retryCount = 0;
            var url = urls[0];
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
            // contents.SetValue(result.Result, 0);
            // SendFunction(udonSendFunctions[0], sendCustomEvents[0], setVariableNames[0], contents[0]);
            SendFunction(udonSendFunctions[0], sendCustomEvents[0], setVariableNames[0], result.Result);
            DelUrl();
            UdonArrayPlus.IndexOf(urls, url, out var _urli);
            while (_urli != -1 && !needReloads[_urli])
            {
                SendFunction(udonSendFunctions[_urli], sendCustomEvents[_urli], setVariableNames[_urli], result.Result);
                DelUrl(_urli);
                UdonArrayPlus.IndexOf(urls, url, out _urli);
            }
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
