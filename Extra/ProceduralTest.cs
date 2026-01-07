using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Title: ProceduralTest
 * Description: The scope of the code is to create a dynamic way to moving an object's 
 *  foot position towards a target. Using vectors and kinematic equations, the speed (velocity)
 *  will change depending on the total distance between the foot position and the target position
 * 
 */
public class ProceduralTest : MonoBehaviour
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

    //  Hips (Base) will act as x_0; targetPosition will be x (final position)
    public Transform Base, footBase, footPosition, targetPosition;

    [Header("Calculated Parameters")]
    //  Distance (targetPosVector - footPosVector) will be deltaX
    public float distance_FT;

    //  Velocity at t = 0 will be 0
    public float velocity;

    public bool isMoving;
    public float x = 0;

    public float highestPoint = 0;

    [Header("Editable Parameters")]
    //private const float acceleration = 2.0f;
    private float acceleration = 12.0f;
    private float maxDistance = 1.5f;
    private float minDistance = 0.15f;

    // Update is called once per frame
    void Update()
    {
        //  Calculate distance and velocity
        distance_FT = Mathf.Abs(Vector3.Distance(footPosition.position, targetPosition.position));
        //  I_Velocity is 0, and time will not be specified, therefore
        //  Velocity = sqrt[(I_velocity)^2 + (acceleration * distance_FT)]
        velocity = Mathf.Sqrt(acceleration * distance_FT);

        checkDistance();
    }

    public void checkDistance()
    {
        footBase.position = footPosition.position;

        if (distance_FT > maxDistance)
        {
            isMoving = true;
        }
        else if(distance_FT < minDistance)
        {
            isMoving = false;
        }

        groundCheck();
    }

    public void groundCheck()
    {
        Ray ray = new Ray(Base.position, -transform.up);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, 3.0f, groundLayer))
        {
            targetPosition.position = hitInfo.point;
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
            footPosition.position = Vector3.Lerp(footPosition.position, targetPosition.position, velocity * Time.deltaTime);
            footVerticalPos();
        }
    }

    public void footVerticalPos()
    {
        float height = (velocity * 0.15f);

        if (x < (Mathf.PI))
        {
            x += velocity * Time.deltaTime;
        }
        else
        {
            x = 0;
        }
        highestPoint = (highestPoint < height) ? height : highestPoint;

        //  Y position modifies the vertical position of the foot
        //  Using a*Sin(n*x) + h, modify the amplitude (a) and the duration (n) using velocity;
        //      use target vertical position to adjust the height (h) of the footstep
        Vector3 y_Pos = new Vector3(footPosition.position.x, 
            targetPosition.position.y + (height * Mathf.Sin(x)), 
            footPosition.position.z);
        footPosition.position = y_Pos;
    }

}
