using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class TerminalReceiver : MonoBehaviour
{
    public Transform coordinateCenterObject;
    public GameObject pilotObject;
    private OSCReceiver oscReceiver;
    private Vector3 positionValue;
    void Start()
    {
        // Creating a receiver.
        oscReceiver = gameObject.GetComponent<OSCReceiver>();
        oscReceiver.LocalPort = 9000;
        oscReceiver.Bind("/pilot/", PositionReceived);
    }
    void PositionReceived(OSCMessage message)
    {
        positionValue.x = message.Values[0].FloatValue;
        positionValue.y = message.Values[1].FloatValue;
        positionValue.z = message.Values[2].FloatValue;
    }
    void Update()
    {
        pilotObject.transform.position = positionValue + coordinateCenterObject.position;
    }
}
