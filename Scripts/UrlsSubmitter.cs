
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonLab.UrlLoader
{
    public class UrlsSubmitter : UdonSharpBehaviour
    {
        public UrlsLoaderCore UrlLoader;
        public VRCUrl[] urls;
        public UdonBehaviour[] udonSendFunctions;
        public string[] sendCustomEvents = new string[0];
        public string[] setVariableNames = new string[0];
        public void SubmitUrl()
        {
            for (int i = 0; i < urls.Length; i++)
            {
                UrlLoader.PushUrl(urls[i], udonSendFunctions[i], sendCustomEvents[i], setVariableNames[i]);
            }
        }
        public void SendFunction() => SubmitUrl();
        public void SendFunctions() => SubmitUrl();
    }
}
