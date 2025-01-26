
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.UrlLoader
{
    public class UrlLoaderCore : UdonSharpBehaviour
    {
        public VRCUrl url;
        public VRCUrl altUrl;
        public bool useAlt = false;
        public bool loadOnStart = true;
        public bool isLoading = false;
        // [NonSerialized] public bool isLoaded = false;
        public UdonBehaviour udonSendFunction;
        public string sendCustomEvent = "SendFunction";
        public string setVariableName = "value";
        public int retryCount = 3;
        protected int _retryCount = 0;
        public bool cacheContent = false;
        public VRCUrl[] cacheUrls;
        bool useUpdateDownload = false;
        protected bool UseUpdateDownload
        {
            get => useUpdateDownload;
            set
            {
                enabled = useUpdateDownload = value;
            }
        }
        void Update()
        {
            if (UseUpdateDownload)
            {
                LoadUrl();
                UseUpdateDownload = false;
            }
        }
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
