﻿
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.UrlLoader
{
    public class UrlsLoaderCore : UdonSharpBehaviour
    {
        public VRCUrl[] urls;
        public bool isLoading = false;
        [NonSerialized] public bool[] isLoaded = new bool[0];
        public UdonBehaviour[] udonSendFunctions;
        public string[] sendCustomEvents;
        public string[] setVariableNames;
        public bool[] needReloads;
        public int retryCount = 3;
        protected int _retryCount = 0;
        public bool cacheContent = false;
        public VRCUrl[] cacheUrls;
        public virtual void LoadUrl() => LoadUrl(false);
        public virtual void LoadUrl(bool reload = false)
        {
            Debug.LogError("Don't use this class directly, use UrlsImageLoader or UrlsStringLoader");
        }
        public virtual void PushUrl(VRCUrl url, UdonBehaviour udonSendFunction, string sendCustomEvent, string setVariableName) => PushUrl(url, udonSendFunction, sendCustomEvent, setVariableName, false);
        public virtual void PushUrl(VRCUrl url, UdonBehaviour udonSendFunction, string sendCustomEvent, string setVariableName, bool reload = false)
        {
            Debug.LogError("Don't use this class directly, use UrlsImageLoader or UrlsStringLoader");
        }
        public void SendFunction(UdonBehaviour udonBehaviour, string customEvent = "", string variableName = "", object value = null)
        {
            if (!string.IsNullOrWhiteSpace(variableName))
                udonBehaviour.SetProgramVariable(variableName, value);
            if (!string.IsNullOrWhiteSpace(customEvent))
                udonBehaviour.SendCustomEvent(customEvent);
        }
        public void SendFunction() => LoadUrl();
        public void SendFunctions() => LoadUrl();
    }
}
