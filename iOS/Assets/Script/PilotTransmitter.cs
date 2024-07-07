using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using extOSC;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using Unity.Burst.Intrinsics;
public class PilotTransmitter : MonoBehaviour
{
    #region NetworkPermissionTrigger
    // URL to trigger network permission - it should be a valid URL
    private string testUrl = "https://apple.com";
    #endregion
    #region Transmitter
    private OSCTransmitter oscTransmitter;
    public InputField ipField;
    #endregion
    #region ARFoundation
    public XROrigin xROrigin;
    public ARTrackedImageManager trackedImageManager;
    public Text iPhonePositionText;
    private ARTrackedImage trackedImage;
    #endregion
    void Awake()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }
    void Start()
    {
        #region NetworkPermissionTrigger
        StartCoroutine(RequestNetworkPermission());
        #endregion
        oscTransmitter = gameObject.GetComponent<OSCTransmitter>();
        oscTransmitter.RemotePort = 9000;
    }
    void Update()
    {
        string ip = ipField.text;
        oscTransmitter.RemoteHost = ip;
        // 计算iPhone在新坐标系中的位置
        Vector3 relativePosition = trackedImage.transform.InverseTransformPoint(xROrigin.Camera.transform.localPosition);
        relativePosition = new Vector3(relativePosition.x, relativePosition.z, relativePosition.y); // iPhone在新坐标系中的位置，我们需要将其y和z坐标互换
        relativePosition -= new Vector3(0, 0, 0.5f); // iPhone在新坐标系中的位置，我们需要将其向前移动0.5个单位
        relativePosition = new Vector3(relativePosition.x, relativePosition.y, -relativePosition.z); // iPhone在新坐标系中的位置，我们需要将其z坐标取反
        iPhonePositionText.text = "iPhone Position:" + " X: " + relativePosition.x.ToString() + " Y: " + relativePosition.y.ToString() + " Z: " + relativePosition.z.ToString();
        Vector3 iPhoneRotation = Camera.main.transform.eulerAngles;
        // Create message
        var message = new OSCMessage("/pilot/");
        message.AddValue(OSCValue.Float(relativePosition.x));
        message.AddValue(OSCValue.Float(relativePosition.y));
        message.AddValue(OSCValue.Float(relativePosition.z));
        // Send message
        oscTransmitter.Send(message);
    }
    #region NetworkPermissionTrigger
    IEnumerator RequestNetworkPermission()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(testUrl))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log($"Error requesting network permission: {webRequest.error}");
            }
            else
            {
                Debug.Log("Network permission has been triggered successfully.");
            }
        }
    }
    #endregion
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newTrackedImage in eventArgs.added)
        {
            trackedImage = newTrackedImage;
        }

        foreach (var updatedTrackedImage in eventArgs.updated)
        {
            trackedImage = updatedTrackedImage;
        }
    }
}

