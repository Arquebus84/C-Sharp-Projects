using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransformTest : MonoBehaviour
{
    /**
     * Scope: create a transformation system with previous, current, and new position targets to move the legs
        
    **Use a swaping procedure to change the position of the current position to the new position, the and the previous position
    *       to the current position; this will allow the transformations to move more organically:
    *       Transform temp = currentPos;
    *       currentPos = newPos;
    *       prevPos = temp;

    **Note: prevPosition will be the currentPosition in the Update
     */


    //The target that the current pos is supposed to land
    public Transform[] newPos = new Transform[2];
    public Transform[] previousPos = new Transform[2];
    //The current pos that the foot must follow
    public Transform[] currentPos = new Transform[2];   //Current Position is the foot position
    public Transform[] foot = new Transform[2];

    public Vector3 distance;    //Distance between the currentPos and the newPos
    public float rotation;      //Rotation between the currentPos and the newPos

    public LayerMask groundLayer;

    GameObject[] hip = new GameObject[2];

    public float moveSpeed, sinTime;

    private void Start()
    {
        hip[0] = new GameObject("RightHip");
        hip[1] = new GameObject("LeftHip");

        hip[0].transform.SetParent(this.transform);
        hip[1].transform.SetParent(this.transform);

        hip[0].transform.localPosition = new Vector3(0.5f, 0, 0);
        hip[1].transform.localPosition = new Vector3(-0.5f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 2; i++)
        {
            distance = newPos[i].position - currentPos[i].position;

            rotation = Mathf.DeltaAngle(newPos[i].eulerAngles.y, currentPos[i].eulerAngles.y);
        }
        changePos();
    }

    public void changePos()
    {
        //NOTE: Current Position is the foot position
        //SetPositionAndRotation(pos1, pos2) method Takes care of the currentPos.transform.position and the currentPos.transform.rotation

        for (int i = 0; i < 2; i++)
        {
            //Set position and rotation for RightFoot and LeftFoot
            foot[i].SetPositionAndRotation(currentPos[i].position, currentPos[i].rotation);

            Ray[] ray = new Ray[2];
            ray[0] = new Ray(hip[0].transform.position, Vector3.down);
            ray[1] = new Ray(hip[1].transform.position, Vector3.down);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray[i], out hitInfo, 4, groundLayer))
            {
                //Change the target positions
                newPos[i].position = new Vector3(hitInfo.point.x, hitInfo.point.y + 0.1f, hitInfo.point.z);

                //RightFoot and LeftFoot
                if (Mathf.Abs(distance.x) > 1f || Mathf.Abs(distance.z) > 1f || Mathf.Abs(rotation) > 45 || currentPos[i].position != newPos[i].position)
                {
                    //currentPos[i].SetPositionAndRotation(newPos[i].position, newPos[i].rotation);

                    sinTime += moveSpeed * Time.deltaTime;

                    currentPos[i].position = Vector3.Lerp(previousPos[i].position, newPos[i].position, sinTime);
                    currentPos[i].rotation = Quaternion.Lerp(previousPos[i].rotation, newPos[i].rotation, 1);
                }
                else
                {
                    previousPos[i].position = currentPos[i].position;
                    sinTime = 0;
                }

                Debug.DrawLine(ray[i].origin, foot[i].position, Color.blue);
            }
            else
            {
                currentPos[i].SetPositionAndRotation(newPos[i].position, newPos[i].rotation);
                Debug.DrawLine(ray[i].origin, foot[i].position, Color.red);
            }
        }
    }
    public float customSine(float x)
    {
        return Mathf.Sin(x - Mathf.PI/2) + 1.0f;
    }
}
