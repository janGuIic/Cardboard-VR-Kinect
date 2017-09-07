using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Kinect;
using UnityEngine;
using Newtonsoft.Json;
using System.Diagnostics;

namespace serverKinect
{
    public partial class MainWindow : Window
    {    
        private KinectSensor kinectSensor = null;
        private CoordinateMapper coordinateMapper = null;        
        private BodyFrameReader bodyFrameReader = null;      
        private Body[] bodies = null;
        private List<Tuple<JointType, JointType>> bones;
        private int displayWidth;
        private int displayHeight;
        private const float InferredZPositionClamp = 0.1f;        

        //private KinectSensor sensor;
        private byte[] data;
        private Vector3[] player1JointsPos;
        private bool[] player1JointsTracked;
        private Matrix4x4[] player1JointsOri;
        bool sendNoUser = true;

        //udp
        UdpClient udpServer;
        IPEndPoint remoteEP;      

        public MainWindow()
        {     
            //server
            udpServer = new UdpClient(11000);
            remoteEP = new IPEndPoint(IPAddress.Any, 11000);           
            udpServer.Receive(ref remoteEP); // listen on port 11000                       

            //kinect
            player1JointsTracked = new bool[25];
            player1JointsPos = new Vector3[25];
            player1JointsOri = new Matrix4x4[25];         

            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();
          
            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // open the sensor
            this.kinectSensor.Open();
            
            // initialize the components (controls) of the window
            this.InitializeComponent();           
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;               
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }            
        }

        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            if (udpServer.Available > 0)
            {
                try
                {
                    System.Threading.Thread.Sleep(500);
                    udpServer.Receive(ref remoteEP);
                }
                catch (SocketException ee)
                {
                    Console.Write(ee);
                }                              
            }    

            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }                       
            }

            if (dataReceived)
            {   
                CameraSpacePoint pos = new CameraSpacePoint();
                foreach (Body body in this.bodies)
                {
                    if (body.IsTracked)
                    {                       
                        IReadOnlyDictionary<JointType, Microsoft.Kinect.Joint> joints = body.Joints;

                        var j = 0;
                        pos = joints[JointType.SpineMid].Position;
                        foreach (JointType jointType in joints.Keys)
                        {
                            if (joints[jointType].TrackingState == TrackingState.Tracked && (int)jointType < 20)
                            {
                                player1JointsTracked[j] = true;
                                player1JointsPos[j] = skelToVector(joints[jointType].Position);
                            }
                            else
                                player1JointsTracked[j] = false;
                            j++;
                        }

                        GetSkeletonJointOrientation(ref player1JointsPos, ref player1JointsTracked, ref player1JointsOri);                                                                    
                        
                        Object obj = new Object();
                        obj.pos = skelToVector2(pos);
                        obj.HipCenter = quarToVector(ConvertMatrixToQuat(player1JointsOri[0], 0));
                        obj.Spine = quarToVector(ConvertMatrixToQuat(player1JointsOri[1], 1));
                        obj.ShoulderCenter = quarToVector(ConvertMatrixToQuat(player1JointsOri[2], 2));
                        obj.Head = quarToVector(ConvertMatrixToQuat(player1JointsOri[3], 3));
                        obj.ShoulderLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[4], 4));
                        obj.ElbowLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[5], 5));
                        obj.WristLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[6], 6));
                        obj.HandLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[7], 7));
                        obj.ShoulderRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[8], 8));
                        obj.ElbowRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[9], 9));
                        obj.WristRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[10], 10));
                        obj.HandRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[11], 11));
                        obj.HipLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[12], 12));
                        obj.KneeLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[13], 13));
                        obj.AnkleLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[14], 14));
                        obj.FootLeft = quarToVector(ConvertMatrixToQuat(player1JointsOri[15], 15));
                        obj.HipRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[16], 16));
                        obj.KneeRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[17], 17));
                        obj.AnkleRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[18], 18));
                        obj.FootRight = quarToVector(ConvertMatrixToQuat(player1JointsOri[19], 19));
                       
                        data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
                        udpServer.Send(data, data.Length, remoteEP);    
                    }
                }                
            }            
        }        
        
        public static void GetSkeletonJointOrientation(ref Vector3[] jointsPos, ref bool[] jointsTracked, ref Matrix4x4[] jointOrients)
        {
            Vector3 vx;
            Vector3 vy;
            Vector3 vz;

            // NUI_SKELETON_POSITION_HIP_CENTER
            if (jointsTracked[(int)NuiSkeletonPositionIndex.HipCenter] && jointsTracked[(int)NuiSkeletonPositionIndex.Spine] &&
                jointsTracked[(int)NuiSkeletonPositionIndex.HipLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.HipRight])
            {
                vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.HipCenter, NuiSkeletonPositionIndex.Spine);
                vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.HipRight);
                MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.HipCenter]);

                // Debug.Log(jointsPos[(int)NuiSkeletonPositionIndex.HipCenter]);
                // make a correction of about 40 degrees back to the front
                //Matrix4x4 mat = jointOrients[(int)NuiSkeletonPositionIndex.HipCenter];
                //Quaternion quat = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
                //quat *= Quaternion.Euler(-40, 0, 0);
                //jointOrients[(int)NuiSkeletonPositionIndex.HipCenter].SetTRS(Vector3.zero, quat, Vector3.one);
            }

            if (jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderRight])
            {
                // NUI_SKELETON_POSITION_SPINE
                if (jointsTracked[(int)NuiSkeletonPositionIndex.Spine] && jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter])
                {
                    vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                    vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ShoulderRight);
                    MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.Spine]);
                }

                if (jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter] && jointsTracked[(int)NuiSkeletonPositionIndex.Head])
                {
                    // NUI_SKELETON_POSITION_SHOULDER_CENTER
                    //if(jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter] && jointsTracked[(int)NuiSkeletonPositionIndex.Head])
                    //{
                    vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.Head);
                    vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ShoulderRight);
                    MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.ShoulderCenter]);
                    //}

                    // NUI_SKELETON_POSITION_HEAD
                    //if(jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter] && jointsTracked[(int)NuiSkeletonPositionIndex.Head])
                    //{
                    //			        vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderCenter, NuiSkeletonPositionIndex.Head);
                    //			        vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ShoulderRight);
                    MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.Head]);
                    //MakeMatrixFromY(vy, ref jointOrients[(int)NuiSkeletonPositionIndex.Head]);
                    //}
                }
            }

            if (jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.ElbowLeft] &&
                //jointsTracked[(int)NuiSkeletonPositionIndex.WristLeft])
                jointsTracked[(int)NuiSkeletonPositionIndex.Spine] && jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter])
            {
                // NUI_SKELETON_POSITION_SHOULDER_LEFT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.ElbowLeft])
                {
                    vx = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ElbowLeft);
                    //vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.WristLeft);
                    vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                    MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.ShoulderLeft]);
                }

                // NUI_SKELETON_POSITION_ELBOW_LEFT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.ElbowLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.WristLeft])
                {
                    vx = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ElbowLeft, NuiSkeletonPositionIndex.WristLeft);
                    //vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderLeft, NuiSkeletonPositionIndex.ElbowLeft);
                    vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                    MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.ElbowLeft]);
                }
            }

            if (jointsTracked[(int)NuiSkeletonPositionIndex.WristLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.HandLeft] &&
             jointsTracked[(int)NuiSkeletonPositionIndex.Spine] && jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter])
            {
                // NUI_SKELETON_POSITION_WRIST_LEFT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.WristLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.HandLeft])
                //{
                vx = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.WristLeft, NuiSkeletonPositionIndex.HandLeft);
                //MakeMatrixFromX(vx, ref jointOrients[(int)NuiSkeletonPositionIndex.WristLeft], false);
                vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.WristLeft]);
                //}

                // NUI_SKELETON_POSITION_HAND_LEFT:
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.WristLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.HandLeft])
                //{
                //		        vx = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.WristLeft, NuiSkeletonPositionIndex.HandLeft);
                //		        //MakeMatrixFromX(vx, ref jointOrients[(int)NuiSkeletonPositionIndex.HandLeft], false);
                //				vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.HandLeft]);
                //Debug.Log( jointsPos[(int)NuiSkeletonPositionIndex.HandLeft]);
                //}
            }

            if (jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderRight] && jointsTracked[(int)NuiSkeletonPositionIndex.ElbowRight] &&
                //jointsTracked[(int)NuiSkeletonPositionIndex.WristRight])
                jointsTracked[(int)NuiSkeletonPositionIndex.Spine] && jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter])
            {
                // NUI_SKELETON_POSITION_SHOULDER_RIGHT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderRight] && jointsTracked[(int)NuiSkeletonPositionIndex.ElbowRight])
                {
                    vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ElbowRight);
                    //vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.WristRight);
                    vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                    MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.ShoulderRight]);
                }

                // NUI_SKELETON_POSITION_ELBOW_RIGHT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.ElbowRight] && jointsTracked[(int)NuiSkeletonPositionIndex.WristRight])
                {
                    vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ElbowRight, NuiSkeletonPositionIndex.WristRight);
                    //vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.ShoulderRight, NuiSkeletonPositionIndex.ElbowRight);
                    vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                    MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.ElbowRight]);
                }
            }

            if (jointsTracked[(int)NuiSkeletonPositionIndex.WristRight] && jointsTracked[(int)NuiSkeletonPositionIndex.HandRight] &&
                jointsTracked[(int)NuiSkeletonPositionIndex.Spine] && jointsTracked[(int)NuiSkeletonPositionIndex.ShoulderCenter])
            {
                // NUI_SKELETON_POSITION_WRIST_RIGHT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.WristRight] && jointsTracked[(int)NuiSkeletonPositionIndex.HandRight])
                //{
                //Console.Write(jointsPos[(int)NuiSkeletonPositionIndex.HandRight].x + " " + jointsPos[(int)NuiSkeletonPositionIndex.HandRight].y + "  " + jointsPos[(int)NuiSkeletonPositionIndex.HandRight].z + "\n");
                vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.WristRight, NuiSkeletonPositionIndex.HandRight);
                //MakeMatrixFromX(vx, ref jointOrients[(int)NuiSkeletonPositionIndex.WristRight], true);
                vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.WristRight]);
                //}

                // NUI_SKELETON_POSITION_HAND_RIGHT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.WristRight] && jointsTracked[(int)NuiSkeletonPositionIndex.HandRight])
                //{
                //		        vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.WristRight, NuiSkeletonPositionIndex.HandRight);
                //		        //MakeMatrixFromX(vx, ref jointOrients[(int)NuiSkeletonPositionIndex.HandRight], true);
                //				vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.Spine, NuiSkeletonPositionIndex.ShoulderCenter);
                MakeMatrixFromXY(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.HandRight]);
                //}
            }

            // NUI_SKELETON_POSITION_HIP_LEFT
            if (jointsTracked[(int)NuiSkeletonPositionIndex.HipLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.KneeLeft] &&
                jointsTracked[(int)NuiSkeletonPositionIndex.HipRight])
            {
                vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.HipLeft);
                vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.HipRight);
                MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.HipLeft]);

                // NUI_SKELETON_POSITION_KNEE_LEFT
                if (jointsTracked[(int)NuiSkeletonPositionIndex.AnkleLeft])
                {
                    vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft);
                    //vz = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.AnkleLeft, NuiSkeletonPositionIndex.FootLeft);
                    //MakeMatrixFromYZ(vy, vz, ref jointOrients[(int)NuiSkeletonPositionIndex.KneeLeft]);
                    vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.HipRight);
                    MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.KneeLeft]);
                }
            }

            if (jointsTracked[(int)NuiSkeletonPositionIndex.KneeLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.AnkleLeft] &&
                jointsTracked[(int)NuiSkeletonPositionIndex.FootLeft])
            {
                // NUI_SKELETON_POSITION_ANKLE_LEFT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.AnkleLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.FootLeft])
                //{
                vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft);
                vz = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.FootLeft, NuiSkeletonPositionIndex.AnkleLeft);
                MakeMatrixFromYZ(vy, vz, ref jointOrients[(int)NuiSkeletonPositionIndex.AnkleLeft]);
                //MakeMatrixFromZ(vz, ref jointOrients[(int)NuiSkeletonPositionIndex.AnkleLeft]);
                //}

                // NUI_SKELETON_POSITION_FOOT_LEFT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.AnkleLeft] && jointsTracked[(int)NuiSkeletonPositionIndex.FootLeft])
                //{
                //		        vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeLeft, NuiSkeletonPositionIndex.AnkleLeft);
                //		        vz = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.FootLeft, NuiSkeletonPositionIndex.AnkleLeft);
                MakeMatrixFromYZ(vy, vz, ref jointOrients[(int)NuiSkeletonPositionIndex.FootLeft]);
                //MakeMatrixFromZ(vz, ref jointOrients[(int)NuiSkeletonPositionIndex.FootLeft]);
                //}
            }

            // NUI_SKELETON_POSITION_HIP_RIGHT
            if (jointsTracked[(int)NuiSkeletonPositionIndex.HipRight] && jointsTracked[(int)NuiSkeletonPositionIndex.KneeRight] &&
                jointsTracked[(int)NuiSkeletonPositionIndex.HipLeft])
            {
                vy = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.HipRight);
                vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.HipRight);
                MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.HipRight]);

                // NUI_SKELETON_POSITION_KNEE_RIGHT
                if (jointsTracked[(int)NuiSkeletonPositionIndex.AnkleRight])
                {
                    vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight);
                    //vz = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.AnkleRight, NuiSkeletonPositionIndex.FootRight);
                    //MakeMatrixFromYZ(vy, vz, ref jointOrients[(int)NuiSkeletonPositionIndex.KneeRight]);
                    vx = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.HipLeft, NuiSkeletonPositionIndex.HipRight);
                    MakeMatrixFromYX(vx, vy, ref jointOrients[(int)NuiSkeletonPositionIndex.KneeRight]);
                }
            }

            if (jointsTracked[(int)NuiSkeletonPositionIndex.KneeRight] && jointsTracked[(int)NuiSkeletonPositionIndex.AnkleRight] &&
                jointsTracked[(int)NuiSkeletonPositionIndex.FootRight])
            {
                // NUI_SKELETON_POSITION_ANKLE_RIGHT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.AnkleRight] && jointsTracked[(int)NuiSkeletonPositionIndex.FootRight])
                //{
                vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight);
                vz = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.FootRight, NuiSkeletonPositionIndex.AnkleRight);
                MakeMatrixFromYZ(vy, vz, ref jointOrients[(int)NuiSkeletonPositionIndex.AnkleRight]);
                //MakeMatrixFromZ(vz, ref jointOrients[(int)NuiSkeletonPositionIndex.AnkleRight]);
                //}

                // NUI_SKELETON_POSITION_FOOT_RIGHT
                //if(jointsTracked[(int)NuiSkeletonPositionIndex.AnkleRight] && jointsTracked[(int)NuiSkeletonPositionIndex.FootRight])
                //{
                //		        vy = -GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.KneeRight, NuiSkeletonPositionIndex.AnkleRight);
                //		        vz = GetPositionBetweenIndices(ref jointsPos, NuiSkeletonPositionIndex.FootRight, NuiSkeletonPositionIndex.AnkleRight);
                MakeMatrixFromYZ(vy, vz, ref jointOrients[(int)NuiSkeletonPositionIndex.FootRight]);
                //MakeMatrixFromZ(vz, ref jointOrients[(int)NuiSkeletonPositionIndex.FootRight]);
                //}
            }
        }

        public enum NuiSkeletonPositionIndex : int
        {
            HipCenter = 0,
            Spine = 1,
            ShoulderCenter = 2,
            Head = 3,
            ShoulderLeft = 4,
            ElbowLeft = 5,
            WristLeft = 6,
            HandLeft = 7,
            ShoulderRight = 8,
            ElbowRight = 9,
            WristRight = 10,
            HandRight = 11,
            HipLeft = 12,
            KneeLeft = 13,
            AnkleLeft = 14,
            FootLeft = 15,
            HipRight = 16,
            KneeRight = 17,
            AnkleRight = 18,
            FootRight = 19,
            Count = 20
        }

        private static Vector3 GetPositionBetweenIndices(ref Vector3[] jointsPos, NuiSkeletonPositionIndex p1, NuiSkeletonPositionIndex p2)
        {
            Vector3 pVec1 = jointsPos[(int)p1];
            Vector3 pVec2 = jointsPos[(int)p2];

            return pVec2 - pVec1;
        }

        private static void MakeMatrixFromXY(Vector3 xUnnormalized, Vector3 yUnnormalized, ref Matrix4x4 jointOrientation)
        {
            //matrix columns
            Vector3 xCol;
            Vector3 yCol;
            Vector3 zCol;

            //set up the three different columns to be rearranged and flipped
            xCol = xUnnormalized.normalized;
            zCol = Vector3.Cross(xCol, yUnnormalized.normalized).normalized;
            yCol = Vector3.Cross(zCol, xCol).normalized;
            //yCol = yUnnormalized.normalized;
            //zCol = Vector3.Cross(xCol, yCol).normalized;

            //copy values into matrix
            PopulateMatrix(ref jointOrientation, xCol, yCol, zCol);
        }

        private static void MakeMatrixFromYZ(Vector3 yUnnormalized, Vector3 zUnnormalized, ref Matrix4x4 jointOrientation)
        {
            //matrix columns
            Vector3 xCol;
            Vector3 yCol;
            Vector3 zCol;

            //set up the three different columns to be rearranged and flipped
            yCol = yUnnormalized.normalized;
            xCol = Vector3.Cross(yCol, zUnnormalized.normalized).normalized;
            zCol = Vector3.Cross(xCol, yCol).normalized;
            //zCol = zUnnormalized.normalized;
            //xCol = Vector3.Cross(yCol, zCol).normalized;

            //copy values into matrix
            PopulateMatrix(ref jointOrientation, xCol, yCol, zCol);
        }

        private static void MakeMatrixFromYX(Vector3 xUnnormalized, Vector3 yUnnormalized, ref Matrix4x4 jointOrientation)
        {

            //matrix columns
            Vector3 xCol;
            Vector3 yCol;
            Vector3 zCol;

            //set up the three different columns to be rearranged and flipped
            yCol = yUnnormalized.normalized;
            zCol = Vector3.Cross(xUnnormalized.normalized, yCol).normalized;
            xCol = Vector3.Cross(yCol, zCol).normalized;
            //xCol = xUnnormalized.normalized;
            //zCol = Vector3.Cross(xCol, yCol).normalized;

            //copy values into matrix
            PopulateMatrix(ref jointOrientation, xCol, yCol, zCol);
        }

        private static void PopulateMatrix(ref Matrix4x4 jointOrientation, Vector3 xCol, Vector3 yCol, Vector3 zCol)
        {

            jointOrientation.SetColumn(0, xCol);
            jointOrientation.SetColumn(1, yCol);
            jointOrientation.SetColumn(2, zCol);
        }

        public static Quaternion QuaternionLookRotation(Vector3 forward, Vector3 up)
        {
            forward.Normalize();

            Vector3 vector = Vector3.Normalize(forward);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            var m00 = vector2.x;
            var m01 = vector2.y;
            var m02 = vector2.z;
            var m10 = vector3.x;
            var m11 = vector3.y;
            var m12 = vector3.z;
            var m20 = vector.x;
            var m21 = vector.y;
            var m22 = vector.z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = (float)Math.Sqrt(num8 + 1f);
                quaternion.w = num * 0.5f;
                num = 0.5f / num;
                quaternion.x = (m12 - m21) * num;
                quaternion.y = (m20 - m02) * num;
                quaternion.z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.x = 0.5f * num7;
                quaternion.y = (m01 + m10) * num4;
                quaternion.z = (m02 + m20) * num4;
                quaternion.w = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.x = (m10 + m01) * num3;
                quaternion.y = 0.5f * num6;
                quaternion.z = (m21 + m12) * num3;
                quaternion.w = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.x = (m20 + m02) * num2;
            quaternion.y = (m21 + m12) * num2;
            quaternion.z = 0.5f * num5;
            quaternion.w = (m01 - m10) * num2;
            return quaternion;
        }

        private Quaternion ConvertMatrixToQuat(Matrix4x4 mOrient, int joint)
        {
            if (player1JointsTracked[joint] == false)
                return Quaternion.identity;

            UnityEngine.Vector4 vZ = mOrient.GetColumn(2);
            UnityEngine.Vector4 vY = mOrient.GetColumn(1);

            vZ.x = -vZ.x;
            vZ.y = -vZ.y;
            vY.z = -vY.z;

            if (vZ.x != 0.0f || vZ.y != 0.0f || vZ.z != 0.0f)
                return QuaternionLookRotation(vZ, vY);
            else
                return Quaternion.identity;
        }

        private Vector3 skelToVector(CameraSpacePoint p)
        {
            Vector3 mv = new Vector3();
            mv.x = p.X;
            mv.y = p.Y + 1;//poglej kaj je tu

            if (p.Z < 0)
                p.Z = InferredZPositionClamp;
            mv.z = p.Z;

            return mv;
        }
        
        private MyVector3 skelToVector2(CameraSpacePoint p)
        {
            MyVector3 mv = new MyVector3(Math.Round(p.X,1), Math.Round(p.Y, 1), Math.Round(p.Z, 1));   
            return mv;
        }

        private MyVector4 quarToVector(Quaternion q)
        {
            MyVector4 v = new MyVector4(q.x,q.y,q.z,q.w);
            
            return v;
        }        
    }    
}

public class Object
{
    public MyVector3 pos;
    public MyVector4 HipCenter;
    public MyVector4 Spine;
    public MyVector4 ShoulderCenter;
    public MyVector4 Head;
    public MyVector4 ShoulderLeft;    
    public MyVector4 ElbowLeft;
    public MyVector4 WristLeft;
    public MyVector4 HandLeft;
    public MyVector4 ShoulderRight;
    public MyVector4 ElbowRight;
    public MyVector4 WristRight;
    public MyVector4 HandRight;
    public MyVector4 HipLeft;
    public MyVector4 KneeLeft;
    public MyVector4 AnkleLeft;
    public MyVector4 FootLeft;
    public MyVector4 HipRight;
    public MyVector4 KneeRight;
    public MyVector4 AnkleRight;
    public MyVector4 FootRight;
   
 public Object()
    {        
    }
}

public class MyVector3
{
    public double x, y, z;
    public MyVector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class MyVector4
{
    public double x, y, z, w;    

    public MyVector4(double x, double y, double z, double w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}

