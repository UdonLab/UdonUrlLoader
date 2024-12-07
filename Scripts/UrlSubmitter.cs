
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
        public UdonBehaviour udonSendFunction;
        public string sendCustomEvent = "SendFunction";
        public string setVariableName = "value";
        public bool startOnLoad = false;
        void Start()
        {
            if (startOnLoad) SubmitUrl();
        }
        public void SubmitUrl()
        {
            if (!string.IsNullOrEmpty(url.ToString())) UrlLoader.PushUrl(url, udonSendFunction, sendCustomEvent, setVariableName);
        }
        public void SendFunction() => SubmitUrl();
        public void SendFunctions() => SubmitUrl();
    }
}
