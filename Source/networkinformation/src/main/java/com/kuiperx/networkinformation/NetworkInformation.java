package com.kuiperx.networkinformation;

import java.io.BufferedReader;
import java.io.InputStreamReader;

public class NetworkInformation
{
    public void GetNetworkStats(String url, PluginCallback callback)
    {
        NetworkStatusThread networkStatusThread = new NetworkStatusThread(url, callback);
        networkStatusThread.start();
    }

    class NetworkStatusThread extends Thread
    {
        String url;
        PluginCallback callback;

        public NetworkStatusThread(String url, PluginCallback callback)
        {
            this.url = url;
            this.callback = callback;
        }

        @Override
        public void run()
        {
            String lost = new String();
            String delay = new String();
            try {
                Process p = Runtime.getRuntime().exec("ping -c 4 " + url);
                BufferedReader buf = new BufferedReader(new InputStreamReader(p.getInputStream()));
                String str = new String();
                while ((str = buf.readLine()) != null) {
                    if (str.contains("packet loss")) {
                        int i = str.indexOf("received");
                        int j = str.indexOf("%");
                        System.out.println(":" + str.substring(i + 10, j + 1));
//                  System.out.println(":"+str.substring(j-3, j+1));
                        lost = str.substring(i + 10, j + 1);
                    }
                    if (str.contains("avg")) {
                        int i = str.indexOf("/", 20);
                        int j = str.indexOf(".", i);
                        System.out.println(":" + str.substring(i + 1, j));
                        delay = str.substring(i + 1, j);
                    }
                }
                callback.onResult(delay + "," + lost);
            }
            catch (Exception e)
            {
                System.out.println(e);
                callback.onResult("failed" + e.getMessage());
            }
        }
    }
}
