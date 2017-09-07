using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;

[RequireComponent(typeof(Animator))]
public class MyAvatarControllerRokoborba : MonoBehaviour
{       

    // Rate at which avatar will move through the scene. The rate multiplies the movement speed (.001f, i.e dividing by 1000, unity's framerate).
    protected int moveRate = 1;

    // Slerp smooth factor
    public float smoothFactor = 5f;
    
    // The body root node
    protected Transform bodyRoot;

    // A required variable if you want to rotate the model in space.
    protected GameObject offsetNode;

    // Variable to hold all them bones. It will initialize the same size as initialRotations.
    protected Transform[] bones;

    // Rotations of the bones when the Kinect tracking starts.
    protected Quaternion[] initialRotations;
    protected Quaternion[] initialLocalRotations;

    // Initial position and rotation of the transform
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;

    // Calibration Offset Variables for Character Position.
    protected bool offsetCalibrated = false;
    protected float xOffset, yOffset, zOffset;

    protected clientRokoborba c;   

    // transform caching gives performance boost since Unity calls GetComponent<Transform>() each time you call transform 
    private Transform _transformCache;
    public new Transform transform
    {
        get
        {
            if (!_transformCache)
                _transformCache = base.transform;

            return _transformCache;
        }
    }       

    public void Awake()
    {
        // check for double start
        if (bones != null)
            return;

        // inits the bones array
        bones = new Transform[22];

        // Initial rotations and directions of the bones.
        initialRotations = new Quaternion[bones.Length];
        initialLocalRotations = new Quaternion[bones.Length];

        // Map bones to the points the Kinect tracks
        MapBones();

        // Get initial bone rotations
        GetInitialRotations();
    }
     

    // Update the avatar each frame.
    public void Update()
    {       

        if (!transform.gameObject.activeInHierarchy)
            return;

        // Get the KinectManager instance
        if (c == null)
        {
            c = clientRokoborba.Instance;
        }

        else if (c.reset) {
            Debug.Log("RESETIRAAAM");
            c.reset = false;
            ResetToInitialPosition();
        }

        // move the avatar to its Kinect position
        MoveAvatar();

        for (var boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            if (!bones[boneIndex])
                continue;

            if (boneIndex2JointMap.ContainsKey(boneIndex))
            {
                clientRokoborba.SkeletonPositionIndex joint =  boneIndex2JointMap[boneIndex];
                TransformBone(joint, boneIndex);
            }           
        }
    }

    // Set bones to their initial positions and rotations
    public void ResetToInitialPosition()
    {
        if (bones == null)
            return;

        if (offsetNode != null)
        {
            offsetNode.transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }

        // For each bone that was defined, reset to initial position.
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                bones[i].rotation = initialRotations[i];
            }
        }

        if (bodyRoot != null)
        {
            bodyRoot.localPosition = Vector3.zero;
            bodyRoot.localRotation = Quaternion.identity;
        }

        // Restore the offset's position and rotation
        if (offsetNode != null)
        {
            offsetNode.transform.position = initialPosition;
            offsetNode.transform.rotation = initialRotation;
        }
        else
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
    }

   
    // Apply the rotations tracked by kinect to the joints.
    protected void TransformBone(clientRokoborba.SkeletonPositionIndex joint, int boneIndex)
    {
        Transform boneTransform = bones[boneIndex];
        if (boneTransform == null || c == null)
            return;

        int iJoint = (int)joint;
        if (iJoint < 0)
            return;

        // Get Kinect joint orientation
        Quaternion jointRotation = c.GetJointOrientation(iJoint);
        //Debug.Log(jointRotation.x + " " + jointRotation.y + " " + jointRotation.z + " " + jointRotation.w + "\n");
        //Debug.Log(jointRotation);    
        if (jointRotation == Quaternion.identity)
            return;
        // Smoothly transition to the new rotation
        Quaternion newRotation = Kinect2AvatarRot(jointRotation, boneIndex);

        if (smoothFactor != 0f)
            boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, smoothFactor * Time.deltaTime);
        else
            boneTransform.rotation = newRotation;
    }


    // Moves the avatar in 3D space - pulls the tracked position of the spine and applies it to root.
    // Only pulls positional, not rotational.
    protected void MoveAvatar()
    {
        if (bodyRoot == null || c == null)
            return;
        if (!c.IsJointTracked())
            return;

        // Get the position of the body and store it.
        Vector3 trans = c.GetUserPosition();

        // If this is the first time we're moving the avatar, set the offset. Otherwise ignore it.
        if (!offsetCalibrated)
        {
            offsetCalibrated = true;

            xOffset =  trans.x * moveRate;
            yOffset = trans.y * moveRate;
            zOffset = -trans.z * moveRate;
                       
        }

        // Smoothly transition to the new position
        Vector3 targetPos = Kinect2AvatarPos(trans);

        if (smoothFactor != 0f)
            bodyRoot.localPosition = Vector3.Lerp(bodyRoot.localPosition, targetPos, smoothFactor * Time.deltaTime);
        else
            bodyRoot.localPosition = targetPos;
    }

    // If the bones to be mapped have been declared, map that bone to the model.
    protected virtual void MapBones()
    {
        // make OffsetNode as a parent of model transform.
        offsetNode = new GameObject(name + "Ctrl") { layer = transform.gameObject.layer, tag = transform.gameObject.tag };
        offsetNode.transform.position = transform.position;
        offsetNode.transform.rotation = transform.rotation;
        offsetNode.transform.parent = transform.parent;

        transform.parent = offsetNode.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // take model transform as body root
        bodyRoot = transform;

        // get bone transforms from the animator component
        var animatorComponent = GetComponent<Animator>();

        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            if (!boneIndex2MecanimMap.ContainsKey(boneIndex))
                continue;

            bones[boneIndex] = animatorComponent.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
        }
    }

    // Capture the initial rotations of the bones
    protected void GetInitialRotations()
    {
        // save the initial rotation
        if (offsetNode != null)
        {
            initialPosition = offsetNode.transform.position;
            initialRotation = offsetNode.transform.rotation;


            offsetNode.transform.rotation = Quaternion.identity;
        }
        else
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            transform.rotation = Quaternion.identity;
        }

        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                initialRotations[i] = bones[i].rotation; // * Quaternion.Inverse(initialRotation);
                initialLocalRotations[i] = bones[i].localRotation;
            }
        }

        // Restore the initial rotation
        if (offsetNode != null)
        {
            offsetNode.transform.rotation = initialRotation;
        }
        else
        {
            transform.rotation = initialRotation;
        }
    }

    // Converts kinect joint rotation to avatar joint rotation, depending on joint initial rotation and offset rotation
    protected Quaternion Kinect2AvatarRot(Quaternion jointRotation, int boneIndex)
    {
        // Apply the new rotation.
        Quaternion newRotation = jointRotation * initialRotations[boneIndex];

        //If an offset node is specified, combine the transform with its
        //orientation to essentially make the skeleton relative to the node
        if (offsetNode != null)
        {
            // Grab the total rotation by adding the Euler and offset's Euler.
            Vector3 totalRotation = newRotation.eulerAngles + offsetNode.transform.rotation.eulerAngles;
            // Grab our new rotation.
            newRotation = Quaternion.Euler(totalRotation);
        }

        return newRotation;
    }

    // Converts Kinect position to avatar skeleton position, depending on initial position, mirroring and move rate
    protected Vector3 Kinect2AvatarPos(Vector3 jointPosition)
    {
        float xPos;
        float yPos;
        float zPos;

        // If movement is mirrored, reverse it.
        xPos = jointPosition.x * moveRate - xOffset;
       

        yPos = jointPosition.y * moveRate - yOffset;
        zPos = -jointPosition.z * moveRate - zOffset;

        // If we are tracking vertical movement, update the y. Otherwise leave it alone.
        Vector3 avatarJointPos = new Vector3(xPos, yPos, zPos);

        return avatarJointPos;
    }

    // dictionaries to speed up bones' processing
    // the author of the terrific idea for kinect-joints to mecanim-bones mapping
    // along with its initial implementation, including following dictionary is
    // Mikhail Korchun (korchoon@gmail.com). Big thanks to this guy!
    private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
    {
        {0, HumanBodyBones.Hips},
        {1, HumanBodyBones.Spine},
        {2, HumanBodyBones.Neck},
        {3, HumanBodyBones.Head},

        {4, HumanBodyBones.LeftShoulder},
        {5, HumanBodyBones.LeftUpperArm},
        {6, HumanBodyBones.LeftLowerArm},
        {7, HumanBodyBones.LeftHand},
        {8, HumanBodyBones.LeftIndexProximal},

        {9, HumanBodyBones.RightShoulder},
        {10, HumanBodyBones.RightUpperArm},
        {11, HumanBodyBones.RightLowerArm},
        {12, HumanBodyBones.RightHand},
        {13, HumanBodyBones.RightIndexProximal},

        {14, HumanBodyBones.LeftUpperLeg},
        {15, HumanBodyBones.LeftLowerLeg},
        {16, HumanBodyBones.LeftFoot},
        {17, HumanBodyBones.LeftToes},

        {18, HumanBodyBones.RightUpperLeg},
        {19, HumanBodyBones.RightLowerLeg},
        {20, HumanBodyBones.RightFoot},
        {21, HumanBodyBones.RightToes},
    };

    protected readonly Dictionary<int, clientRokoborba.SkeletonPositionIndex> boneIndex2JointMap = new Dictionary<int, clientRokoborba.SkeletonPositionIndex>
    {
        {0, clientRokoborba.SkeletonPositionIndex.HipCenter},
        {1, clientRokoborba.SkeletonPositionIndex.Spine},
        {2, clientRokoborba.SkeletonPositionIndex.ShoulderCenter},
        {3, clientRokoborba.SkeletonPositionIndex.Head},

        {5, clientRokoborba.SkeletonPositionIndex.ShoulderLeft},
        {6, clientRokoborba.SkeletonPositionIndex.ElbowLeft},
        {7, clientRokoborba.SkeletonPositionIndex.WristLeft},
        {8, clientRokoborba.SkeletonPositionIndex.HandLeft},

        {10, clientRokoborba.SkeletonPositionIndex.ShoulderRight},
        {11, clientRokoborba.SkeletonPositionIndex.ElbowRight},
        {12, clientRokoborba.SkeletonPositionIndex.WristRight},
        {13, clientRokoborba.SkeletonPositionIndex.HandRight},

        {14, clientRokoborba.SkeletonPositionIndex.HipLeft},
        {15, clientRokoborba.SkeletonPositionIndex.KneeLeft},
        {16, clientRokoborba.SkeletonPositionIndex.AnkleLeft},
        {17, clientRokoborba.SkeletonPositionIndex.FootLeft},

        {18, clientRokoborba.SkeletonPositionIndex.HipRight},
        {19, clientRokoborba.SkeletonPositionIndex.KneeRight},
        {20, clientRokoborba.SkeletonPositionIndex.AnkleRight},
        {21, clientRokoborba.SkeletonPositionIndex.FootRight},
    };

    protected readonly Dictionary<int, List<clientRokoborba.SkeletonPositionIndex>> specIndex2JointMap = new Dictionary<int, List<clientRokoborba.SkeletonPositionIndex>>
    {
        {4, new List<clientRokoborba.SkeletonPositionIndex> { clientRokoborba.SkeletonPositionIndex.ShoulderLeft, clientRokoborba.SkeletonPositionIndex.ShoulderCenter} },
        {9, new List<clientRokoborba.SkeletonPositionIndex> { clientRokoborba.SkeletonPositionIndex.ShoulderRight, clientRokoborba.SkeletonPositionIndex.ShoulderCenter} },
    };

    protected readonly Dictionary<int, clientRokoborba.SkeletonPositionIndex> boneIndex2MirrorJointMap = new Dictionary<int, clientRokoborba.SkeletonPositionIndex>
    {
        {0, clientRokoborba.SkeletonPositionIndex.HipCenter},
        {1, clientRokoborba.SkeletonPositionIndex.Spine},
        {2, clientRokoborba.SkeletonPositionIndex.ShoulderCenter},
        {3, clientRokoborba.SkeletonPositionIndex.Head},

        {5, clientRokoborba.SkeletonPositionIndex.ShoulderRight},
        {6, clientRokoborba.SkeletonPositionIndex.ElbowRight},
        {7, clientRokoborba.SkeletonPositionIndex.WristRight},
        {8, clientRokoborba.SkeletonPositionIndex.HandRight},

        {10, clientRokoborba.SkeletonPositionIndex.ShoulderLeft},
        {11, clientRokoborba.SkeletonPositionIndex.ElbowLeft},
        {12, clientRokoborba.SkeletonPositionIndex.WristLeft},
        {13, clientRokoborba.SkeletonPositionIndex.HandLeft},

        {14, clientRokoborba.SkeletonPositionIndex.HipRight},
        {15, clientRokoborba.SkeletonPositionIndex.KneeRight},
        {16, clientRokoborba.SkeletonPositionIndex.AnkleRight},
        {17, clientRokoborba.SkeletonPositionIndex.FootRight},

        {18, clientRokoborba.SkeletonPositionIndex.HipLeft},
        {19, clientRokoborba.SkeletonPositionIndex.KneeLeft},
        {20, clientRokoborba.SkeletonPositionIndex.AnkleLeft},
        {21, clientRokoborba.SkeletonPositionIndex.FootLeft},
    };

    protected readonly Dictionary<int, List<clientRokoborba.SkeletonPositionIndex>> specIndex2MirrorJointMap = new Dictionary<int, List<clientRokoborba.SkeletonPositionIndex>>
    {
        {4, new List<clientRokoborba.SkeletonPositionIndex> { clientRokoborba.SkeletonPositionIndex.ShoulderRight, clientRokoborba.SkeletonPositionIndex.ShoulderCenter} },
        {9, new List<clientRokoborba.SkeletonPositionIndex> { clientRokoborba.SkeletonPositionIndex.ShoulderLeft, clientRokoborba.SkeletonPositionIndex.ShoulderCenter} },
    };

}

