using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;

public class CooplayerCoords : MonoBehaviour {
    //sprejemanje
    public UdpClient udpServer;
    public IPEndPoint remoteEP;

    PlayerHealth playerHealth;
    public int attackDamage = 10;               // The amount of health taken away per attack.


    humanBody saveHuman;
    private Matrix4x4 kinectToWorld;
    private static CooplayerCoords instance;
    public float SensorHeight = 1.0f;
    public int SensorAngle = 0;
    private GameObject player;
    public bool kill;
    public bool bekilled;

    //štop
    UdpClient udpClient = new UdpClient();
    public byte[] data;
    public IPEndPoint newIncomingEndPoint;

    public static CooplayerCoords Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Start () {        

        udpServer = new UdpClient(12000);
        udpServer.Client.ReceiveTimeout = 15000;
        remoteEP = new IPEndPoint(IPAddress.Any, 0);        
        var data = udpServer.Receive(ref remoteEP); // listen on port 12000
        udpServer.Send(new byte[] { 1 },1, remoteEP);    
        Start_Now();
    }

    void Update()
    {        
        if (kill)
        {
            player = GameObject.Find("Player Dimp");
            playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.GiveDamage(attackDamage);
            kill = false;
        }
        if (bekilled)
        {
            player = GameObject.Find("Player Dimp");
            playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(attackDamage);
            bekilled = false;
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
            newIncomingEndPoint = remoteEP;
            data = udpServer.EndReceive(ar, ref newIncomingEndPoint);
            //udpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse("192.168.1.5"), 12345));
            udpServer.BeginReceive(callback, null);
            String json = Encoding.ASCII.GetString(data, 0, data.Length);

            if (json == String.Empty)
                saveHuman = null;
            else if (json == "a")
                kill = true;
            else if (json == "b")
                bekilled = true;
            else
            {
                saveHuman = JsonUtility.FromJson<humanBody>(json);
            }


        };
        udpServer.BeginReceive(callback, null);
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
