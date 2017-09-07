using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class CooplayerCoords : MonoBehaviour {
    UdpClient udpServer;
    IPEndPoint remoteEP;
    //sprejemanje
    UdpClient udpServerReceive;
    IPEndPoint remoteEPReceive;
    humanBody saveHuman;
    private Matrix4x4 kinectToWorld;
    private static CooplayerCoords instance;
    public float SensorHeight = 1.0f;
    public int SensorAngle = 0;
    clientRokoborba coords;
    bool semNwtr = false;
    public byte[] data;
    public IPEndPoint newIncomingEndPoint;

    UdpClient udpClient = new UdpClient();

    public static CooplayerCoords Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Start () {       
        GameObject player = GameObject.Find("Canvas");
        coords = player.GetComponent<clientRokoborba>();
    }
	
	// Update is called once per frame
	void Update () {
        if (coords.udpClient1 != null && coords.ep1 != null && !semNwtr)
        {
            udpServerReceive = new UdpClient(13000);
            remoteEPReceive = new IPEndPoint(coords.ep1.Address, coords.ep1.Port);
            semNwtr = true;
            Start_Now();
        }
    }

    void Start_Now()
    {
        instance = this;
        Quaternion quatTiltAngle = new Quaternion();
        quatTiltAngle.eulerAngles = new Vector3(-SensorAngle, 0.0f, 0.0f);
        kinectToWorld.SetTRS(new Vector3(0.0f, SensorHeight, 0.0f), quatTiltAngle, Vector3.one);

        AsyncCallback callback = null;
        callback = ar =>
        {
            newIncomingEndPoint = remoteEPReceive;
            data = udpServerReceive.EndReceive(ar, ref newIncomingEndPoint);
            udpServerReceive.BeginReceive(callback, null);
            String json = Encoding.ASCII.GetString(data, 0, data.Length);

            if (json == String.Empty)
                saveHuman = null;
            else
            {
                saveHuman = JsonUtility.FromJson<humanBody>(json);
            }

        };
        udpServerReceive.BeginReceive(callback, null);
    }

    public Quaternion GetJointOrientation(int joint)
    {
        if (saveHuman == null)
            return Quaternion.identity;
        switch (joint)
        {
            case 0:
                return saveHuman.HipCenter;
            case 1:
                return saveHuman.Spine;
            case 2:
                return saveHuman.ShoulderCenter;
            case 3:
                return saveHuman.Head;
            case 4:
                return saveHuman.ShoulderLeft;
            case 5:
                return saveHuman.ElbowLeft;
            case 6:
                return saveHuman.WristLeft;
            case 7:
                return saveHuman.HandLeft;
            case 8:
                return saveHuman.ShoulderRight;
            case 9:
                return saveHuman.ElbowRight;
            case 10:
                return saveHuman.WristRight;
            case 11:
                return saveHuman.HandRight;
            case 12:
                return saveHuman.HipLeft;
            case 13:
                return saveHuman.KneeLeft;
            case 14:
                return saveHuman.AnkleLeft;
            case 15:
                return saveHuman.FootLeft;
            case 16:
                return saveHuman.HipRight;
            case 17:
                return saveHuman.KneeRight;
            case 18:
                return saveHuman.AnkleRight;
            case 19:
                return saveHuman.FootRight;
            default:
                return Quaternion.identity;
        }
    }

    public Vector3 GetUserPosition()
    {
        if (saveHuman != null)
            return kinectToWorld.MultiplyPoint3x4((saveHuman.pos));
        return Vector3.zero;
    }

    public bool IsJointTracked()
    {
        if (saveHuman != null)
            return true;

        return false;
    }
}
