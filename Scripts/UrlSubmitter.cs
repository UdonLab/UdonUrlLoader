
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.UrlLoader
{
    public class UrlSubmitter : UdonSharpBehaviour
    {
        public UrlsLoaderCore UrlLoader;
        public VRCUrl url;
        public VRCUrl altUrl;
        public UdonBehaviour udonSendFunction;
        public string sendCustomEvent = "SendFunction";
        public string setVariableName = "value";
        public bool startOnLoad = false;
        public bool useUpdate = false;
        protected bool UseUpdate
        {
            get => UseUpdate;
            set
            {
                enabled = UseUpdate = value;
            }
        }
        void Start()
        {
            if (startOnLoad) SubmitUrl();
        }
        void Update()
        {
            if (UseUpdate)
            {
                SubmitUrl();
                UseUpdate = false;
            }
        }
        public void SubmitUrlWithUpdate() => useUpdate = true;
        public void SubmitUrl()
        {
            if (!string.IsNullOrEmpty(url.ToString())) UrlLoader.PushUrl(url, altUrl, udonSendFunction, sendCustomEvent, setVariableName);
        }
        public void SendFunction() => SubmitUrl();
        public void SendFunctions() => SubmitUrl();
    }
}
