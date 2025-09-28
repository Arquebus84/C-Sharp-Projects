using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Title: KinematicCtrl
 *  Description: Utilizing kinematic equations from Physics, create a procedurally animated leg system for a multi-legged creature *
 */

public class KinematicCtrl : MonoBehaviour
{
    /**
            Kinematic Equations:
    x_f = v_avg*t + x_0
    x_f = x_0 + v_0t + 0.5*a*t^2
    v = v_0 + at
    v^2 = (v_0)^2 + 2a(x - x_0) ===>    v = sqrt[ (v_0)^2 + 2a(x - x_0) ]
     
     */

    //  Parameters
    public LayerMask groundLayer;

    //  Hips (Base) will act as x_0; futureFootPosition will be x (final position)
    public Transform FootHipBase, footBase, footIKPos, futureFootPosition;

    [Header("Calculated Parameters")]
    //  Distance (targetPosVector - footPosVector) will be deltaX
    public float distance_FT;
    public float angle_FT;

    //  Velocity at t = 0 will be 0
    public float velocity;

    public bool isMoving;
    public float x = 0;

    public float highestPoint = 0;

    [Header("Editable Parameters")]
    public float acceleration;
    public float maxDistance = 0.2f;
    public float minDistance = 0.05f;
    private float maxAngle = 1.55f;     //Angle isn't editable
    private float minAngle = 0.2f;

    // Update is called once per frame
    void Update()
    {
        //  Calculate distance, angle, and velocity
        distance_FT = Mathf.Abs(Vector3.Distance(footIKPos.position, futureFootPosition.position));
        angle_FT = Mathf.Abs(Mathf.DeltaAngle(footIKPos.eulerAngles.y, futureFootPosition.eulerAngles.y)) * Mathf.Deg2Rad;  //Angle in Radians is more efficient

        //  I_Velocity is 0, and time will not be specified, therefore
        //  Velocity = sqrt[(I_velocity)^2 + (acceleration * distance_FT)]
        velocity = (angle_FT < maxAngle)? Mathf.Sqrt(acceleration * distance_FT) : Mathf.Sqrt(acceleration * distance_FT * angle_FT);

        checkDistance(footBase, footIKPos);
    }

    public void checkDistance(Transform foot, Transform pos)
    {
        foot.position = pos.position;
        foot.rotation = pos.rotation;

        if (distance_FT > maxDistance || angle_FT > maxAngle)       //  If either the distance or the angle between foot and futurePos exceeds max, then it's moving
        {
            isMoving = true;
        }
        else if (distance_FT < minDistance && angle_FT < minAngle)  //  For clarity, both position and angle have to be minimized in order for foot to completely rest
        {
            isMoving = false;
        }

        groundCheck();
    }

    public void groundCheck()
    {
        Ray ray = new Ray(FootHipBase.position, -transform.up);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 3.0f, groundLayer))
        {
            futureFootPosition.position = hitInfo.point;
            translateFoot();

            Debug.DrawLine(ray.origin, hitInfo.point, Color.green);
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2.0f, Color.red);
        }
    }

    public void translateFoot()
    {
        if (isMoving)
        {
            footIKPos.position = Vector3.Lerp(footIKPos.position, futureFootPosition.position, velocity * Time.deltaTime);
            footIKPos.rotation = Quaternion.Lerp(footIKPos.rotation, futureFootPosition.rotation, velocity * Time.deltaTime);
            footVerticalPos();
        }
        else
        {
            x = 0;
        }
    }

    public void footVerticalPos()
    {
        float height = (velocity * 0.095f);

        if (x < Mathf.PI)
        {
            x += velocity * Time.deltaTime;     //If isMoving is false, x will be 0 (Look in translateFoot())
        }
        else
        {
            x = 0;
        }

        highestPoint = (highestPoint < height) ? height : highestPoint;

        //  Y position modifies the vertical position of the foot
        //  Using a*Sin(n*x) + h, modify the amplitude (a) and the duration (n) using velocity;
        //      use target vertical position to adjust the height (h) of the footstep
        Vector3 y_Pos = new Vector3(footIKPos.position.x,
            futureFootPosition.position.y + (height * Mathf.Sin(x)),
            footIKPos.position.z);
        footIKPos.position = y_Pos;
    }
}
