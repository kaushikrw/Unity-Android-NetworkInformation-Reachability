using System;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

class NetworkInformation
{
    public static string packetLoss = "0%";
    public static float latency = 1000f;

    private bool speedTestInProgress = false;
    private bool pingTestInProgress = false;

    private AndroidJavaObject networkInfo;
    private AndroidPluginCallback callback;

    private void CheckPacketLoss()
    {
        if (!pingTestInProgress)
        {
#if UNITY_STANDALONE
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            PingOptions options = new PingOptions();

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data); // 32 bytes of data

            var pingAmount = 4;
            var failedPings = 0;
            var latencySum = 0;

            for (int i = 0; i < pingAmount; i++)
            {
                PingReply reply = ping.Send(GlobalConstants.mediaServerAddress, 1000, buffer, options);

                if (reply != null)
                {
                    if (reply.Status != IPStatus.Success)
                        failedPings += 1;
                    else
                        latencySum += (int)reply.RoundtripTime;
                }
            }

            float averagePing = (latencySum / pingAmount);
            latency = averagePing;
            packetLoss = (Convert.ToDouble(failedPings) / Convert.ToDouble(pingAmount) * 100).ToString() + " %";
            Debug.Log("Ping test: " + averagePing + "|| Loss  :  " + packetLoss.ToString() + " %", true);
            pingTestInProgress = false;
        }
#endif
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject networkInfo = new AndroidJavaObject("com.kuiperx.networkinformation.NetworkInformation");
                AndroidPluginCallback callback = new AndroidPluginCallback();
                callback.SetCallback(CheckPacketLossCallback);
                UnityEngine.Debug.Log("Starting Ping Test");
                networkInfo.Call("GetNetworkStats", new object[] { GlobalConstants.mediaServerAddress, callback });
            }
#endif
        }
    }

    private void CheckPacketLossCallback(string result)
    {
        Debug.Log("Ping test: " + result);
        if (result.Contains("failed"))
        {
            packetLoss = "999";
        }
        else
        {
            latency = float.Parse(result.Split(',')[0]);
            packetLoss = result.Split(',')[1];
        }
        pingTestInProgress = false;
    }

    public class AndroidPluginCallback : AndroidJavaProxy
    {
        public string result;

        private Action<string> callbackAction;

        public AndroidPluginCallback() : base("com.kuiperx.networkinformation.PluginCallback") { }

        public void onResult(string result)
        {
            this.result = result;
            UnityEngine.Debug.Log(result);
            callbackAction?.Invoke(result);
        }

        public void SetCallback(Action<string> action)
        {
            callbackAction = action;
        }
    }

}
