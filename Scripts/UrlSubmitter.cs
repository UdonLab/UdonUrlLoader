
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonLab.UrlLoader
{
    public class UrlSubmitter : UdonSharpBehaviour
    {
        public UrlsLoaderCore UrlLoader;
        public VRCUrl url;
        public UdonBehaviour udonSendFunction;
        public string sendCustomEvent = "SendFunction";
        public string setVariableName = "value";
        public void SubmitUrl()
        {
            UrlLoader.PushUrl(url, udonSendFunction, sendCustomEvent, setVariableName);
        }
        public void SendFunction() => SubmitUrl();
        public void SendFunctions() => SubmitUrl();
    }
}
