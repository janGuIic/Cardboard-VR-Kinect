using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Filter to correct the joint locations and joint orientations to constraint to range of viable human motion.
/// </summary>
public class BoneOrientationsConstraint
{
	public enum ConstraintAxis { X = 0, Y = 1, Z = 2 }
    
	
    // The Joint Constraints.  
    private readonly List<BoneOrientationConstraint> jointConstraints = new List<BoneOrientationConstraint>();

	//private GameObject debugText;
	

    /// Initializes a new instance of the BoneOrientationConstraints class.
    public BoneOrientationsConstraint()
    {
		//debugText = GameObject.Find("CalibrationText");
    }
	
	// find the bone constraint structure for given joint
	// returns the structure index in the list, or -1 if the bone structure is not found
	private int FindBoneOrientationConstraint(int joint)
	{
		for(int i = 0; i < jointConstraints.Count; i++)
		{
			if(jointConstraints[i].joint == joint)
				return i;
		}
		
		// not found
		return -1;
	}

    // AddJointConstraint - Adds a joint constraint to the system.  
    public void AddBoneOrientationConstraint(int joint, ConstraintAxis axis, float angleMin, float angleMax)
    {
		int index = FindBoneOrientationConstraint(joint);
		
		BoneOrientationConstraint jc = index >= 0 ? jointConstraints[index] : new BoneOrientationConstraint(joint);
		
		if(index < 0)
		{
			index = jointConstraints.Count;
			jointConstraints.Add(jc);
		}
		
		AxisOrientationConstraint constraint = new AxisOrientationConstraint(axis, angleMin, angleMax);
		jc.axisConstrainrs.Add(constraint);
		
		jointConstraints[index] = jc;
     }

    // AddDefaultConstraints - Adds a set of default joint constraints for normal human poses.  
    // This is a reasonable set of constraints for plausible human bio-mechanics.
    public void AddDefaultConstraints()
    {
//        // Spine
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.X, -10f, 45f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.Y, -10f, 30f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.Spine, ConstraintAxis.Z, -10f, 30f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.Spine, ConstraintAxis.X, -90f, 95f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.Spine, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.Spine, ConstraintAxis.Z, -90f, 90f);

        // ShoulderCenter
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderCenter, ConstraintAxis.X, -30f, 10f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderCenter, ConstraintAxis.Y, -20f, 20f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderCenter, ConstraintAxis.Z, -20f, 20f);

        // ShoulderLeft, ShoulderRight
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderLeft, ConstraintAxis.X, 0f, 30f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderRight, ConstraintAxis.X, 0f, 30f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderLeft, ConstraintAxis.Y, -60f, 60f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderRight, ConstraintAxis.Y, -30f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderLeft, ConstraintAxis.Z, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ShoulderRight, ConstraintAxis.Z, -90f, 90f);

//        // ElbowLeft, ElbowRight
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.X, 300f, 360f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.X, 300f, 360f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Y, 210f, 340f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Y, 0f, 120f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Z, -90f, 30f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Z, 0f, 120f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ElbowLeft, ConstraintAxis.Z, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.ElbowRight, ConstraintAxis.Z, -90f, 90f);

        // WristLeft, WristRight
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.WristLeft, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.WristRight, ConstraintAxis.X, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.WristLeft, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.WristRight, ConstraintAxis.Y, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.WristLeft, ConstraintAxis.Z, -90f, 90f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.WristRight, ConstraintAxis.Z, -90f, 90f);

//        // HipLeft, HipRight
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft, ConstraintAxis.X, 0f, 90f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipRight, ConstraintAxis.X, 0f, 90f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft, ConstraintAxis.Y, 0f, 0f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipRight, ConstraintAxis.Y, 0f, 0f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft, ConstraintAxis.Z, 270f, 360f);
//        AddBoneOrientationConstraint((int)KinectWrapper.NuiSkeletonPositionIndex.HipRight, ConstraintAxis.Z, 0f, 90f);

        // KneeLeft, KneeRight
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.KneeLeft, ConstraintAxis.X, 270f, 360f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.KneeRight, ConstraintAxis.X, 270f, 360f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.KneeLeft, ConstraintAxis.Y, 0f, 0f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.KneeRight, ConstraintAxis.Y, 0f, 0f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.KneeLeft, ConstraintAxis.Z, 0f, 0f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.KneeRight, ConstraintAxis.Z, 0f, 0f);

        // AnkleLeft, AnkleRight
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.AnkleLeft, ConstraintAxis.X, 0f, 0f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.AnkleRight, ConstraintAxis.X, 0f, 0f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.AnkleLeft, ConstraintAxis.Y, -40f, 40f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.AnkleRight, ConstraintAxis.Y, -40f, 40f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.AnkleLeft, ConstraintAxis.Z, 0f, 0f);
        AddBoneOrientationConstraint((int)NuiSkeletonPositionIndex.AnkleRight, ConstraintAxis.Z, 0f, 0f);
        
	}

    // ApplyBoneOrientationConstraints and constrain rotations.
    public void Constrain(ref Matrix4x4[] jointOrientations, ref bool[] jointTracked)
    {
        // Constraints are defined as a vector with respect to the parent bone vector, and a constraint angle, 
        // which is the maximum angle with respect to the constraint axis that the bone can move through.

        // Calculate constraint values. 0.0-1.0 means the bone is within the constraint cone. Greater than 1.0 means 
        // it lies outside the constraint cone.
        for (int i = 0; i < this.jointConstraints.Count; i++)
        {
            BoneOrientationConstraint jc = this.jointConstraints[i];

            if (!jointTracked[i] || jc.joint == (int)NuiSkeletonPositionIndex.HipCenter) 
            {
                // End joint is not tracked or Hip Center has no parent to perform this calculation with.
                continue;
            }

            // If the joint has a parent, constrain the bone direction to be within the constraint cone
            int parentIdx = GetSkeletonJointParent(jc.joint);

            // Local bone orientation relative to parent
            Matrix4x4 localOrientationMatrix = Invert(jointOrientations[parentIdx]) * jointOrientations[jc.joint];
			
			Vector3 localOrientationZ = (Vector3)localOrientationMatrix.GetColumn(2);
			Vector3 localOrientationY = (Vector3)localOrientationMatrix.GetColumn(1);
			if(localOrientationZ == Vector3.zero || localOrientationY == Vector3.zero)
				continue;

            Quat localRotation = Quat.LookRotation(localOrientationZ, localOrientationY);
			Vector3 eulerAngles = localRotation.eulerAngles;
			bool isConstrained = false;
			
			//Matrix4x4 globalOrientationMatrix = jointOrientations[jc.joint];
			//Quaternion globalRotation = Quaternion.LookRotation(globalOrientationMatrix.GetColumn(2), globalOrientationMatrix.GetColumn(1));
			
			for(int a = 0; a < jc.axisConstrainrs.Count; a++)
			{
				AxisOrientationConstraint ac = jc.axisConstrainrs[a];

                Quat axisRotation = Quat.AngleAxis(localRotation.eulerAngles[ac.axis], ac.rotateAround);
				//Quaternion axisRotation = Quaternion.AngleAxis(globalRotation.eulerAngles[ac.axis], ac.rotateAround);
				float angleFromMin = Quat.Angle(axisRotation, quatToQuat2(ac.minQuaternion));
				float angleFromMax = Quat.Angle(axisRotation, quatToQuat2(ac.maxQuaternion));
				 
				if(!(angleFromMin <= ac.angleRange && angleFromMax <= ac.angleRange))
				{
					// Keep the current rotations around other axes and only
					// correct the axis that has fallen out of range.
					//Vector3 euler = globalRotation.eulerAngles;
					
					if(angleFromMin > angleFromMax)
					{
						eulerAngles[ac.axis] = ac.angleMax;
					}
					else
					{
						eulerAngles[ac.axis] = ac.angleMin;
					}
					
					isConstrained = true;
				}
			}
			
			if(isConstrained)
			{
				Quat constrainedRotation = Quat.Euler(eulerAngles);

                // Put it back into the bone orientations
                localOrientationMatrix = Transform(localOrientationMatrix, quatToQuat(constrainedRotation)); 
				jointOrientations[jc.joint] = jointOrientations[parentIdx] * localOrientationMatrix;
				//globalOrientationMatrix.SetTRS(Vector3.zero, constrainedRotation, Vector3.one); 
				//jointOrientations[jc.joint] = globalOrientationMatrix;
				
				switch(jc.joint)
				{
					case (int)NuiSkeletonPositionIndex.ShoulderCenter:
						jointOrientations[(int)NuiSkeletonPositionIndex.Head] = jointOrientations[jc.joint];
						break;
					case (int)NuiSkeletonPositionIndex.WristLeft:
						jointOrientations[(int)NuiSkeletonPositionIndex.HandLeft] = jointOrientations[jc.joint];
						break;
					case (int)NuiSkeletonPositionIndex.WristRight:
						jointOrientations[(int)NuiSkeletonPositionIndex.HandRight] = jointOrientations[jc.joint];
						break;
					case (int)NuiSkeletonPositionIndex.AnkleLeft:
						jointOrientations[(int)NuiSkeletonPositionIndex.FootLeft] = jointOrientations[jc.joint];
						break;
					case (int)NuiSkeletonPositionIndex.AnkleRight:
						jointOrientations[(int)NuiSkeletonPositionIndex.FootRight] = jointOrientations[jc.joint];
						break;
				}
			}
			
//			globalRotation = Quaternion.LookRotation(globalOrientationMatrix.GetColumn(2), globalOrientationMatrix.GetColumn(1));
//			string stringToDebug = string.Format("{0}, {2}", (KinectWrapper.NuiSkeletonPositionIndex)jc.joint, 
//				globalRotation.eulerAngles, localRotation.eulerAngles);
//			Debug.Log(stringToDebug);
//			
//			if(debugText != null)
//				debugText.guiText.text = stringToDebug;
			
        }
    }


	// Joint Constraint structure to hold the constraint axis, angle and cone direction and associated joint.
    private struct BoneOrientationConstraint
    {
		// skeleton joint
		public int joint;
		
		// the list of axis constraints for this bone
		public List<AxisOrientationConstraint> axisConstrainrs;
		
		
        public BoneOrientationConstraint(int joint)
        {
            this.joint = joint;
			axisConstrainrs = new List<AxisOrientationConstraint>();
        }
    }

    private struct AxisOrientationConstraint
	{
		// the axis to rotate around
		public int axis;
		public Vector3 rotateAround;
				
		// min, max and range of allowed angle
		public float angleMin;
		public float angleMax;
		
		public Quaternion minQuaternion;
		public Quaternion maxQuaternion;
		public float angleRange;
				
		
		public AxisOrientationConstraint(ConstraintAxis axis, float angleMin, float angleMax)
		{
			// Set the axis that we will rotate around
			this.axis = (int)axis;
            this.rotateAround = Vector3.zero;
			switch(axis)
			{
				case ConstraintAxis.X:
					this.rotateAround = Vector3.right;
					break;
				 
				case ConstraintAxis.Y:
					this.rotateAround = Vector3.up;
					break;
				 
				case ConstraintAxis.Z:
					this.rotateAround = Vector3.forward;
                    
					break;
			
				default:
					this.rotateAround = Vector3.zero;
					break;
			}
			
			// Set the min and max rotations in degrees
			this.angleMin = angleMin;
			this.angleMax = angleMax;

            // Set the min and max rotations in quaternion space
            this.minQuaternion = AngleAxis(angleMin, this.rotateAround);
            this.maxQuaternion = AngleAxis(angleMax, this.rotateAround);

            this.angleRange = angleMax - angleMin;
		}
	}

    private static Quaternion AngleAxis(float degress, Vector3 axis)
    {
         const float degToRad = (float)(Math.PI / 180.0);

          if (axis.sqrMagnitude == 0.0f)
              return Quaternion.identity;

          Quaternion result = Quaternion.identity;
          var radians = degress * degToRad;
          radians *= 0.5f;
          axis.Normalize();
          axis = axis * (float)System.Math.Sin(radians);
          result.x = axis.x;
          result.y = axis.y;
          result.z = axis.z;
          result.w = (float)System.Math.Cos(radians);

          return Normalize(result);
    }   

    public static Quaternion Normalize(Quaternion q)
    {
       float scale = 1.0f / Length(q);
       return vrniQuart(q.x * scale, q.y * scale, q.z * scale, q.w * scale);
    }

    public static float Length(Quaternion q)
    {
        return (float)System.Math.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
    }

    public static Quaternion vrniQuart(float x, float y, float z, float w)
    {
        Quaternion q = new Quaternion();
        q.x = x;
        q.y = y;
        q.z = z;
        q.w = w;
        return q;
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

    public static int GetSkeletonJointParent(int jointIndex)
    {
        switch (jointIndex)
        {
            case (int)NuiSkeletonPositionIndex.HipCenter:
                return (int)NuiSkeletonPositionIndex.HipCenter;
            case (int)NuiSkeletonPositionIndex.Spine:
                return (int)NuiSkeletonPositionIndex.HipCenter;
            case (int)NuiSkeletonPositionIndex.ShoulderCenter:
                return (int)NuiSkeletonPositionIndex.Spine;
            case (int)NuiSkeletonPositionIndex.Head:
                return (int)NuiSkeletonPositionIndex.ShoulderCenter;
            case (int)NuiSkeletonPositionIndex.ShoulderLeft:
                return (int)NuiSkeletonPositionIndex.ShoulderCenter;
            case (int)NuiSkeletonPositionIndex.ElbowLeft:
                return (int)NuiSkeletonPositionIndex.ShoulderLeft;
            case (int)NuiSkeletonPositionIndex.WristLeft:
                return (int)NuiSkeletonPositionIndex.ElbowLeft;
            case (int)NuiSkeletonPositionIndex.HandLeft:
                return (int)NuiSkeletonPositionIndex.WristLeft;
            case (int)NuiSkeletonPositionIndex.ShoulderRight:
                return (int)NuiSkeletonPositionIndex.ShoulderCenter;
            case (int)NuiSkeletonPositionIndex.ElbowRight:
                return (int)NuiSkeletonPositionIndex.ShoulderRight;
            case (int)NuiSkeletonPositionIndex.WristRight:
                return (int)NuiSkeletonPositionIndex.ElbowRight;
            case (int)NuiSkeletonPositionIndex.HandRight:
                return (int)NuiSkeletonPositionIndex.WristRight;
            case (int)NuiSkeletonPositionIndex.HipLeft:
                return (int)NuiSkeletonPositionIndex.HipCenter;
            case (int)NuiSkeletonPositionIndex.KneeLeft:
                return (int)NuiSkeletonPositionIndex.HipLeft;
            case (int)NuiSkeletonPositionIndex.AnkleLeft:
                return (int)NuiSkeletonPositionIndex.KneeLeft;
            case (int)NuiSkeletonPositionIndex.FootLeft:
                return (int)NuiSkeletonPositionIndex.AnkleLeft;
            case (int)NuiSkeletonPositionIndex.HipRight:
                return (int)NuiSkeletonPositionIndex.HipCenter;
            case (int)NuiSkeletonPositionIndex.KneeRight:
                return (int)NuiSkeletonPositionIndex.HipRight;
            case (int)NuiSkeletonPositionIndex.AnkleRight:
                return (int)NuiSkeletonPositionIndex.KneeRight;
            case (int)NuiSkeletonPositionIndex.FootRight:
                return (int)NuiSkeletonPositionIndex.AnkleRight;
        }

        return (int)NuiSkeletonPositionIndex.HipCenter;
    }

    public struct Quat
    {
        const float radToDeg = (float)(180.0 / Math.PI);
        const float degToRad = (float)(Math.PI / 180.0);

        public const float kEpsilon = 1E-06f; // should probably be used in the 0 tests in LookRotation or Slerp
        public Vector3 xyz
        {
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
            get
            {
                return new Vector3(x, y, z);
            }
        }
        public float x;
        public float y;
        public float z;
        public float w;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    case 3:
                        return this.w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }
        public static Quat identity
        {
            get
            {
                return new Quat(0f, 0f, 0f, 1f);
            }
        }
        public Vector3 eulerAngles
        {
            get
            {
                return Quat.Internal_ToEulerRad(this) * radToDeg;
            }
            set
            {
                this = Quat.Internal_FromEulerRad(value * degToRad);
            }
        }

        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(x * x + y * y + z * z + w * w);
            }
        }

        public float LengthSquared
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            xyz *= scale;
            w *= scale;
        }

        public static Quat Normalize(Quat q)
        {
            Quat result;
            Normalize(ref q, out result);
            return result;
        }

        public static void Normalize(ref Quat q, out Quat result)
        {
            float scale = 1.0f / q.Length;
            result = new Quat(q.xyz * scale, q.w * scale);
        }

        public Quat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quat(Vector3 v, float w)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = w;
        }

        public void Set(float new_x, float new_y, float new_z, float new_w)
        {
            this.x = new_x;
            this.y = new_y;
            this.z = new_z;
            this.w = new_w;
        }
        public static float Dot(Quat a, Quat b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static Quat AngleAxis(float angle, Vector3 axis)
        {
            return Quat.INTERNAL_CALL_AngleAxis(angle, ref axis);
        }
        private static Quat INTERNAL_CALL_AngleAxis(float degress, ref Vector3 axis)
        {
            if (axis.sqrMagnitude == 0.0f)
                return identity;

            Quat result = identity;
            var radians = degress * degToRad;
            radians *= 0.5f;
            axis.Normalize();
            axis = axis * (float)System.Math.Sin(radians);
            result.x = axis.x;
            result.y = axis.y;
            result.z = axis.z;
            result.w = (float)System.Math.Cos(radians);

            return Normalize(result);
        }
        public void ToAngleAxis(out float angle, out Vector3 axis)
        {
            Quat.Internal_ToAxisAngleRad(this, out axis, out angle);
            angle *= radToDeg;
        }

        public static Quat FromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            return RotateTowards(LookRotation(fromDirection), LookRotation(toDirection), float.MaxValue);
        }

        public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            this = Quat.FromToRotation(fromDirection, toDirection);
        }

        public static Quat LookRotation(Vector3 forward, Vector3 upwards)
        {
            return Quat.INTERNAL_CALL_LookRotation(ref forward, ref upwards);
        }

        public static Quat LookRotation(Vector3 forward)
        {
            Vector3 up = Vector3.up;
            return Quat.INTERNAL_CALL_LookRotation(ref forward, ref up);
        }

        private static Quat INTERNAL_CALL_LookRotation(ref Vector3 forward, ref Vector3 up)
        {

            forward = Vector3.Normalize(forward);
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
            up = Vector3.Cross(forward, right);
            var m00 = right.x;
            var m01 = right.y;
            var m02 = right.z;
            var m10 = up.x;
            var m11 = up.y;
            var m12 = up.z;
            var m20 = forward.x;
            var m21 = forward.y;
            var m22 = forward.z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quat();
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

        public void SetLookRotation(Vector3 view)
        {
            Vector3 up = Vector3.up;
            this.SetLookRotation(view, up);
        }

        public void SetLookRotation(Vector3 view, Vector3 up)
        {
            this = Quat.LookRotation(view, up);
        }

        public static Quat Slerp(Quat a, Quat b, float t)
        {
            return Quat.INTERNAL_CALL_Slerp(ref a, ref b, t);
        }

        private static Quat INTERNAL_CALL_Slerp(ref Quat a, ref Quat b, float t)
        {
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
        }

        public static Quat SlerpUnclamped(Quat a, Quat b, float t)
        {
            return Quat.INTERNAL_CALL_SlerpUnclamped(ref a, ref b, t);
        }

        private static Quat INTERNAL_CALL_SlerpUnclamped(ref Quat a, ref Quat b, float t)
        {
            // if either input is zero, return the other.
            if (a.LengthSquared == 0.0f)
            {
                if (b.LengthSquared == 0.0f)
                {
                    return identity;
                }
                return b;
            }
            else if (b.LengthSquared == 0.0f)
            {
                return a;
            }


            float cosHalfAngle = a.w * b.w + Vector3.Dot(a.xyz, b.xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return a;
            }
            else if (cosHalfAngle < 0.0f)
            {
                b.xyz = -b.xyz;
                b.w = -b.w;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - t;
                blendB = t;
            }

            Quat result = new Quat(blendA * a.xyz + blendB * b.xyz, blendA * a.w + blendB * b.w);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return identity;
        }
        /// <summary>
        ///   <para>Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is clamped to the range [0, 1].</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat Lerp(Quat a, Quat b, float t)
        {
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return INTERNAL_CALL_Slerp(ref a, ref b, t); // TODO: use lerp not slerp, "Because quaternion works in 4D. Rotation in 4D are linear" ???
        }
        /// <summary>
        ///   <para>Interpolates between /a/ and /b/ by /t/ and normalizes the result afterwards. The parameter /t/ is not clamped.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static Quat LerpUnclamped(Quat a, Quat b, float t)
        {
            return INTERNAL_CALL_Slerp(ref a, ref b, t);
        }
        /// <summary>
        ///   <para>Rotates a rotation /from/ towards /to/.</para>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxDegreesDelta"></param>
        public static Quat RotateTowards(Quat from, Quat to, float maxDegreesDelta)
        {
            float num = Quat.Angle(from, to);
            if (num == 0f)
            {
                return to;
            }
            float t = Mathf.Min(1f, maxDegreesDelta / num);
            return Quat.SlerpUnclamped(from, to, t);
        }

        public static Quat Inverse(Quat rotation)
        {
            float lengthSq = rotation.LengthSquared;
            if (lengthSq != 0.0)
            {
                float i = 1.0f / lengthSq;
                return new Quat(rotation.xyz * -i, rotation.w * i);
            }
            return rotation;
        }

        public override string ToString()
        {
            return string.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", new object[]
            {
                this.x,
                this.y,
                this.z,
                this.w
            });
        }
        /// <summary>
        ///   <para>Returns a nicely formatted string of the Quaternion.</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format)
        {
            return string.Format("({0}, {1}, {2}, {3})", new object[]
            {
                this.x.ToString(format),
                this.y.ToString(format),
                this.z.ToString(format),
                this.w.ToString(format)
            });
        }
        /// <summary>
        ///   <para>Returns the angle in degrees between two rotations /a/ and /b/.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Angle(Quat a, Quat b)
        {
            float f = Quat.Dot(a, b);
            return Mathf.Acos(Mathf.Min(Mathf.Abs(f), 1f)) * 2f * radToDeg;
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static Quat Euler(double x, double y, double z)
        {
            return Quat.Internal_FromEulerRad(new Vector3((float)x, (float)y, (float)z) * degToRad);
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static Quat Euler(float x, float y, float z)
        {
            return Quat.Internal_FromEulerRad(new Vector3(x, y, z) * degToRad);
        }
        /// <summary>
        ///   <para>Returns a rotation that rotates z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis (in that order).</para>
        /// </summary>
        /// <param name="euler"></param>
        public static Quat Euler(Vector3 euler)
        {
            return Quat.Internal_FromEulerRad(euler * degToRad);

        }

        // from http://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
        private static Vector3 Internal_ToEulerRad(Quat rotation)
        {
            float sqw = rotation.w * rotation.w;
            float sqx = rotation.x * rotation.x;
            float sqy = rotation.y * rotation.y;
            float sqz = rotation.z * rotation.z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = rotation.x * rotation.w - rotation.y * rotation.z;
            Vector3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.y = 2f * Mathf.Atan2(rotation.y, rotation.x);
                v.x = Mathf.PI / 2;
                v.z = 0;
                return NormalizeAngles(v * Mathf.Rad2Deg);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.y = -2f * Mathf.Atan2(rotation.y, rotation.x);
                v.x = -Mathf.PI / 2;
                v.z = 0;
                return NormalizeAngles(v * Mathf.Rad2Deg);
            }
            Quat q = new Quat(rotation.w, rotation.z, rotation.x, rotation.y);
            v.y = (float)Math.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z * q.z + q.w * q.w));     // Yaw
            v.x = (float)Math.Asin(2f * (q.x * q.z - q.w * q.y));                             // Pitch
            v.z = (float)Math.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y * q.y + q.z * q.z));      // Roll
            return NormalizeAngles(v * Mathf.Rad2Deg);
        }
        private static Vector3 NormalizeAngles(Vector3 angles)
        {
            angles.x = NormalizeAngle(angles.x);
            angles.y = NormalizeAngle(angles.y);
            angles.z = NormalizeAngle(angles.z);
            return angles;
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

        private static Quat Internal_FromEulerRad(Vector3 euler)
        {
            var yaw = euler.x;
            var pitch = euler.y;
            var roll = euler.z;
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)Math.Sin((double)rollOver2);
            float cosRollOver2 = (float)Math.Cos((double)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
            float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)Math.Sin((double)yawOver2);
            float cosYawOver2 = (float)Math.Cos((double)yawOver2);
            Quat result;
            result.x = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
            result.z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.w = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            return result;

        }
        private static void Internal_ToAxisAngleRad(Quat q, out Vector3 axis, out float angle)
        {
            if (Math.Abs(q.w) > 1.0f)
                q.Normalize();


            angle = 2.0f * (float)System.Math.Acos(q.w); // angle
            float den = (float)System.Math.Sqrt(1.0 - q.w * q.w);
            if (den > 0.0001f)
            {
                axis = q.xyz / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                axis = new Vector3(1, 0, 0);
            }
        }
    }

    private Quaternion quatToQuat(Quat q)
    {
        Quaternion qt = new Quaternion();
        qt.x = q.x;
        qt.y = q.y;
        qt.z = q.z;
        qt.w = q.w;
        return qt;
    }

    private Quat quatToQuat2(Quaternion q)
    {
        Quat qt = new Quat();
        qt.x = q.x;
        qt.y = q.y;
        qt.z = q.z;
        qt.w = q.w;
        return qt;
    }

    public static Matrix4x4 Invert(Matrix4x4 matrix)
    {
        Matrix4x4 result = new Matrix4x4(); 

        float a = matrix.m00, b = matrix.m01, c = matrix.m02, d = matrix.m03;
        float e = matrix.m10, f = matrix.m11, g = matrix.m12, h = matrix.m13;
        float i = matrix.m20, j = matrix.m21, k = matrix.m22, l = matrix.m23;
        float m = matrix.m30, n = matrix.m31, o = matrix.m32, p = matrix.m33;

        float kp_lo = k * p - l * o;
        float jp_ln = j * p - l * n;
        float jo_kn = j * o - k * n;
        float ip_lm = i * p - l * m;
        float io_km = i * o - k * m;
        float in_jm = i * n - j * m;

        float a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
        float a12 = -(e * kp_lo - g * ip_lm + h * io_km);
        float a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
        float a14 = -(e * jo_kn - f * io_km + g * in_jm);

        float det = a * a11 + b * a12 + c * a13 + d * a14;

        if (Math.Abs(det) < float.Epsilon)
        {
            return result;
        }

        float invDet = 1.0f / det;

        result.m00 = a11 * invDet;
        result.m10 = a12 * invDet;
        result.m20 = a13 * invDet;
        result.m30 = a14 * invDet;

        result.m01 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
        result.m11 = +(a * kp_lo - c * ip_lm + d * io_km) * invDet;
        result.m21 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
        result.m31 = +(a * jo_kn - b * io_km + c * in_jm) * invDet;

        float gp_ho = g * p - h * o;
        float fp_hn = f * p - h * n;
        float fo_gn = f * o - g * n;
        float ep_hm = e * p - h * m;
        float eo_gm = e * o - g * m;
        float en_fm = e * n - f * m;

        result.m02 = +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
        result.m12 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
        result.m22 = +(a * fp_hn - b * ep_hm + d * en_fm) * invDet;
        result.m32 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

        float gl_hk = g * l - h * k;
        float fl_hj = f * l - h * j;
        float fk_gj = f * k - g * j;
        float el_hi = e * l - h * i;
        float ek_gi = e * k - g * i;
        float ej_fi = e * j - f * i;

        result.m03 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
        result.m13 = +(a * gl_hk - c * el_hi + d * ek_gi) * invDet;
        result.m23 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
        result.m33 = +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet;

        return result;
    }

    public static Matrix4x4 Transform(Matrix4x4 value, Quaternion rotation)
    {
        // Compute rotation matrix.
        float x2 = rotation.x + rotation.x;
        float y2 = rotation.y + rotation.y;
        float z2 = rotation.z + rotation.z;

        float wx2 = rotation.w * x2;
        float wy2 = rotation.w * y2;
        float wz2 = rotation.w * z2;
        float xx2 = rotation.x * x2;
        float xy2 = rotation.x * y2;
        float xz2 = rotation.x * z2;
        float yy2 = rotation.y * y2;
        float yz2 = rotation.y * z2;
        float zz2 = rotation.z * z2;

        float q11 = 1.0f - yy2 - zz2;
        float q21 = xy2 - wz2;
        float q31 = xz2 + wy2;

        float q12 = xy2 + wz2;
        float q22 = 1.0f - xx2 - zz2;
        float q32 = yz2 - wx2;

        float q13 = xz2 - wy2;
        float q23 = yz2 + wx2;
        float q33 = 1.0f - xx2 - yy2;

        Matrix4x4 result;

        // First row
        result.m00 = value.m00 * q11 + value.m01 * q21 + value.m02 * q31;
        result.m01 = value.m00 * q12 + value.m01 * q22 + value.m02 * q32;
        result.m02 = value.m00 * q13 + value.m01 * q23 + value.m02 * q33;
        result.m03 = value.m03;

        // Second row
        result.m10 = value.m10 * q11 + value.m11 * q21 + value.m12 * q31;
        result.m11 = value.m10 * q12 + value.m11 * q22 + value.m12 * q32;
        result.m12 = value.m10 * q13 + value.m11 * q23 + value.m12 * q33;
        result.m13 = value.m13;

        // Third row
        result.m20 = value.m20 * q11 + value.m21 * q21 + value.m22 * q31;
        result.m21 = value.m20 * q12 + value.m21 * q22 + value.m22 * q32;
        result.m22 = value.m20 * q13 + value.m21 * q23 + value.m22 * q33;
        result.m23 = value.m23;

        // Fourth row
        result.m30 = value.m30 * q11 + value.m31 * q21 + value.m32 * q31;
        result.m31 = value.m30 * q12 + value.m31 * q22 + value.m32 * q32;
        result.m32 = value.m30 * q13 + value.m31 * q23 + value.m32 * q33;
        result.m33 = value.m33;

        return result;
    }
}