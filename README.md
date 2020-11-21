# Unity-Android-NetworkInformation-Reachability

NetworkInformation to fetch internet connectivity stats

```
ping
packetloss
```
Platforms:
Standalone,
Android

# Why Unity's Reachability Test doesn't work

Unity's API only checks if the device is connected either to a Wireless network or a LAN. It fails, when
the device is connected but has no internet.

# NetworkInformation Class

Source contains the Java Source. 

NetworkInformation.cs contains the C# Wrapper (also works with Unity) for both Standalone and Android platforms.

# Usage

```
 networkInfo.Call("GetNetworkStats", new object[] { "<url>", callback });
 ```
 
 pass in the url to test the connection with. Eg - Google.com or any appwerver, webserver etc., which has ICMP enabled.
 
 The java library will run a command and send the results back through the callback.
 
 See NetworkInformation.cs for example.
