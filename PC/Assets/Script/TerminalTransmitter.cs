using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using extOSC;
public class TerminalTransmitter : MonoBehaviour
{
    #region Client
    public Text timeText;
    public Text localIpText;
    private OSCTransmitter oscClient;
    public InputField ipField;
    public Button linkButton;
    public RawImage ipImage;
    public InputField instructionField;
    public Button instructionButton;
    #endregion
    #region ARFoundation
    public RawImage positionImage;
    #endregion
    void Start()
    {
        positionImage.gameObject.SetActive(false);
        ipImage.color = Color.red;
        DisplayLocalIPAddress();
        oscClient = gameObject.GetComponent<OSCTransmitter>();

        linkButton.onClick.AddListener(delegate
        {
            string ip = ipField.text;
            SendPing(ip);
            oscClient.RemoteHost = ip;
            oscClient.RemotePort = 9000;
            // Create message
            var message1 = new OSCMessage("/terminal/");
            message1.AddValue(OSCValue.String("linked"));
            var message2 = new OSCMessage("/time/");
            message2.AddValue(OSCValue.String(timeText.text));
            // Send message
            oscClient.Send(message1);
            oscClient.Send(message2);
        });
        instructionButton.onClick.AddListener(delegate
        {
            string instruction = instructionField.text;
            SendInstruction(instruction);
        });
    }
    void Update()
    {
        // 获取游戏自开始以来的时间，转换为TimeSpan
        TimeSpan timeSinceStart = TimeSpan.FromSeconds(Time.time);

        // 转换为"小时:分钟:秒"的格式
        string timeTextString = string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSinceStart.Hours,
            timeSinceStart.Minutes,
            timeSinceStart.Seconds);

        // 更新timeText的文本
        timeText.text = timeTextString;
    }
    void SendInstruction(string instruction)
    {
        var message = new OSCMessage("/terminal/");
        message.AddValue(OSCValue.String(instruction));
        oscClient.Send(message);
    }
    void SendPing(string ip)
    {
        System.Net.IPAddress ipAddress;
        if (!System.Net.IPAddress.TryParse(ip, out ipAddress))
        {
            Debug.Log("Invalid IP address: " + ip);
            return;
        }

        System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
        PingReply reply = pingSender.Send(ip);

        if (reply.Status == IPStatus.Success)
        {
            Debug.Log("Ping to " + ip + " successful");
            ipImage.color = Color.green;
            linkButton.interactable = false;
            linkButton.GetComponentInChildren<Text>().text = "Linked";
            positionImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Ping to " + ip + " failed");
            ipImage.color = Color.red;
        }
    }
    void DisplayLocalIPAddress()
    {
        // 获取本机的主机名
        string hostName = Dns.GetHostName();

        // 获取本机的所有IP地址
        IPAddress[] addresses = Dns.GetHostAddresses(hostName);

        // 遍历所有IP地址，找到第一个IPv4地址
        foreach (IPAddress address in addresses)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                // 更新text的文本为本机的IP地址
                localIpText.text = "Local IP Address: " + address.ToString();
                break;
            }
        }
    }
}
