/**Note: This C# code is compatible with Unity Software, as well as any Inverse Kinematics System application, and it is a simple implentation of humanoid foot placement

For an implenetation on any humanoid characters, import your existing animation files, (Blender, Mixamo, etc.) or create them in the Unity Software itself; this script 
will automatically reference the Animator Component. 
In addition, for the FABRIK implementation, it is recommented to utilize the algorithm developed by DitzelGames; their technique is
shown and utilized in this video: https://www.youtube.com/watch?v=qqOAzn05fvk&t=1623s
*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootPosition : MonoBehaviour
{
    public Animator animator;
    public Transform center;

    [Header("Left Foot Parameters")]
    private Transform leftFoot;
    public Transform leftFootTarget;
    public Transform leftFootBase;

    [Header("Right Foot Parameters")]
    private Transform rightFoot;
    public Transform rightFootTarget;
    public Transform rightFootBase;

    public LayerMask groundLayer;

    public void Start()
    {
        animator = GetComponent<Animator>();

        leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = animator.GetBoneTransform (HumanBodyBones.RightFoot);
    }

    // Update is called once per frame
    void Update()
    {
        //Change the IKTarget for each foot to the position of their respective animation transforms
        leftFootTarget.position = new Vector3((animator.GetIKPosition(AvatarIKGoal.LeftFoot)).x, (animator.GetIKPosition(AvatarIKGoal.LeftFoot)).y, 
            (animator.GetIKPosition(AvatarIKGoal.LeftFoot)).z);
        rightFootTarget.position = new Vector3((animator.GetIKPosition(AvatarIKGoal.RightFoot)).x, (animator.GetIKPosition(AvatarIKGoal.RightFoot)).y, 
            (animator.GetIKPosition(AvatarIKGoal.RightFoot)).z);

        //IK Target Rotation
        leftFootTarget.rotation = animator.GetIKRotation(AvatarIKGoal.LeftFoot);
        rightFootTarget.rotation = animator.GetIKRotation(AvatarIKGoal.RightFoot);

        //Feet Base
        leftFootBase.position = new Vector3(leftFootTarget.position.x, leftFootTarget.position.y - 0.15f, leftFootTarget.position.z);
        rightFootBase.position = new Vector3(rightFootTarget.position.x, rightFootTarget.position.y - 0.15f, rightFootTarget.position.z);
        
        rayPosition();
    }

    public void rayPosition()
    {        
        //From pelvis to feet positions
        Vector3 leftPos = new Vector3((animator.GetIKPosition(AvatarIKGoal.LeftFoot)).x, center.position.y, 
            (animator.GetIKPosition(AvatarIKGoal.LeftFoot)).z);
        Vector3 rightPos = new Vector3((animator.GetIKPosition(AvatarIKGoal.RightFoot)).x, center.position.y, 
            (animator.GetIKPosition(AvatarIKGoal.RightFoot)).z);
                
        Ray leftRay = new Ray(leftPos, Vector3.down);
        Ray rightRay = new Ray(rightPos, Vector3.down);

        RaycastHit hitInfo;

        //Left Ray (Original distance 1.9f)
        if(Physics.Raycast(leftRay, out hitInfo, 1.75f, groundLayer))
        {
            //Transform the leftFoot base to ray information
            leftFootBase.position = new Vector3((animator.GetIKPosition(AvatarIKGoal.LeftFoot)).x, hitInfo.point.y, 
                (animator.GetIKPosition(AvatarIKGoal.LeftFoot)).z);
            
            //Rotate the leftFoot target parallel to the vector surface
            Quaternion leftFootTargetRot = Quaternion.FromToRotation(transform.up, hitInfo.normal);
            leftFootTarget.rotation = Quaternion.Lerp(leftFootTarget.rotation, leftFootTargetRot * transform.rotation, 5f);

            //Transform leftFoot target to align with animations **GetIKPosition
            leftFootTarget.position = new Vector3((animator.GetIKPosition(AvatarIKGoal.LeftFoot)).x, leftFootBase.position.y + 0.25f, 
                (animator.GetIKPosition(AvatarIKGoal.LeftFoot)).z);

            Debug.DrawLine(leftRay.origin, hitInfo.point, Color.green);
        }
        else
        {
            //Reset values
            leftFootBase.position = leftFootBase.position;
            //leftFootTarget.position = leftFootTarget.position;

            //IKTargets: LeftFoot
            leftFootTarget.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot);

            Debug.DrawLine(leftRay.origin, leftRay.origin + leftRay.direction * 1.5f, Color.red);
        }

        //Right Ray
        if (Physics.Raycast(rightRay, out hitInfo, 1.75f, groundLayer))
        {
            //Transform the rightFoot base to ray information
            rightFootBase.position = new Vector3((animator.GetIKPosition(AvatarIKGoal.RightFoot)).x, hitInfo.point.y, 
                (animator.GetIKPosition(AvatarIKGoal.RightFoot)).z);

            //Rotate the rightFoot target parallel to the vector surface
            Quaternion rightFootTargetRot = Quaternion.FromToRotation(transform.up, hitInfo.normal);
            rightFootTarget.rotation = Quaternion.Lerp(rightFootTarget.rotation, rightFootTargetRot * transform.rotation, 5f);
            
            //Transform rightFoot target to align with animations **GetIKPosition
            rightFootTarget.position = new Vector3((animator.GetIKPosition(AvatarIKGoal.RightFoot)).x, rightFootBase.position.y + 0.25f, 
                (animator.GetIKPosition(AvatarIKGoal.RightFoot)).z);
            
            Debug.DrawLine(rightRay.origin, hitInfo.point, Color.green);
        }
        else
        {
            //Reset values
            rightFootBase.position = rightFootBase.position;
            //rightFootTarget.position = rightFootTarget.position;

            //IKTargets: RightFoot
            rightFootTarget.position = animator.GetIKPosition(AvatarIKGoal.RightFoot);

            Debug.DrawLine(rightRay.origin, rightRay.origin + rightRay.direction * 1.5f, Color.red);
        }        
    }
}
