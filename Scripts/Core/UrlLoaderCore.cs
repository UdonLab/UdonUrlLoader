
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonLab.UrlLoader
{
    public class UrlLoaderCore : UdonSharpBehaviour
    {
        public VRCUrl url;
        public bool loadOnStart = true;
        [NonSerialized] public bool isLoaded = false;
        public UdonBehaviour udonSendFunction;
        public string sendCustomEvent = "SendFunction";
        public string setVariableName = "value";
        public int retryCount = 3;
        protected int _retryCount = 0;
        public bool cacheContent = false;
        public VRCUrl[] cacheUrls;
        public virtual void LoadUrl() => LoadUrl(false);
        public virtual void LoadUrl(bool reload = false)
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
