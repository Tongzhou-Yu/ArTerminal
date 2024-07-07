using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using extOSC;
using UnityEngine;
using UnityEngine.UI;
public class PilotReceiver : MonoBehaviour
{
    #region Receiver
    public Text timeText;
    Boolean isLinked = false;
    private string timeValue;
    private float startTime;
    public Text localIpText;
    private OSCReceiver oscReceiver;
    public Text instructionText;
    private string stringValue;
    public RawImage linkedImage;
    #endregion
    void Start()
    {
        linkedImage.color = Color.red;
        DisplayLocalIPAddress();
        // Creating a receiver.
        oscReceiver = gameObject.AddComponent<OSCReceiver>();
        oscReceiver.LocalPort = 9000;
        oscReceiver.Bind("/terminal/", MessageReceived);
        oscReceiver.Bind("/time/", TimeReceived);
    }
    void MessageReceived(OSCMessage message)
    {
        stringValue = message.Values[0].StringValue;
    }
    void TimeReceived(OSCMessage message)
    {
        timeValue = message.Values[0].StringValue;
    }
    void Update()
    {
        instructionText.text = stringValue;
        if (stringValue == "linked" && !isLinked)
        {
            linkedImage.color = Color.green;
            startTime = Time.time;
            isLinked = true;
        }
        UpdateTimeValue(timeValue);
    }
    void UpdateTimeValue(string additionalTime)
    {
        // 获取游戏自开始以来的时间，转换为TimeSpan
        TimeSpan timeSinceStart = TimeSpan.FromSeconds(Time.time - startTime + 1f);
        // 将timeValue转换为TimeSpan
        TimeSpan additionalTimeSpan = TimeSpan.Parse(additionalTime);
        // 将两个TimeSpan相加
        TimeSpan newTimeSpan = timeSinceStart + additionalTimeSpan;

        // 转换为"小时:分钟:秒"的格式
        string timeTextString = string.Format("{0:D2}:{1:D2}:{2:D2}",
            newTimeSpan.Hours,
            newTimeSpan.Minutes,
            newTimeSpan.Seconds);
        // 更新timeText的文本
        timeText.text = timeTextString;
        Debug.Log("timeValue: " + timeText.text);
    }
    void DisplayLocalIPAddress()
    {
        /*
        在iOS设备上获取IP地址可能会比在桌面操作系统上更复杂，
        因为iOS设备可能同时连接到蜂窝数据网络和Wi-Fi网络，
        而且每个网络接口都有自己的IP地址。
        这段代码会遍历设备上的所有网络接口，
        并检查每个接口的所有IP地址。
        当找到第一个IPv4地址时，
        它会停止搜索并显示该地址。
        */
        string localIP = "";
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || item.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.Address.ToString();
                        break;
                    }
                }
            }
            if (localIP != "")
            {
                break;
            }
        }
        localIpText.text = "Local IP Address: " + localIP;
    }
}