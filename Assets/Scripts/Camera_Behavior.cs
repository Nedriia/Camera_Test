using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Behavior : MonoBehaviour
{
    public Transform target;
    public Vector3 velocity;
    public float smoothTime;
    public Vector3 offset;

    public Transform player;
    public float turnSpeed;
    public Vector3 offsetRotation;

    public float offsetVerticalFocus;
    public Vector3 offsetVertical;

    float yRotation;
    public float threesholdMovement;

    public void Start()
    {
        offsetRotation = transform.position;     
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(new Vector3(player.position.x,player.position.y + offsetVerticalFocus, player.position.z));
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref velocity, smoothTime);

        RotationControl();
    }

    /*public void Zoom()
    {
        if(Input.GetAxis)
    }*/

    public void RotationControl()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            offsetVertical = new Vector3(0, transform.position.y, 0);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
            offsetVertical = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.left) * offsetVertical;
            
            transform.position = new Vector3(player.position.x + offsetRotation.x,player.position.y + offsetVertical.y,player.position.z + offsetRotation.z);
            transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
        }
        else
        {
            if (player.GetComponent<Rigidbody>().velocity.magnitude <= threesholdMovement)
            {
                transform.position = player.position + offsetRotation;
                transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
                if (Input.GetAxis("Mouse X") < 0)
                {
                    yRotation += Input.GetAxis("Mouse X");
                    player.transform.eulerAngles = new Vector3(0, yRotation, 0);
                }
                else if (Input.GetAxis("Mouse X") > 0)
                {
                    yRotation += Input.GetAxis("Mouse X");
                    player.transform.eulerAngles = new Vector3(0, yRotation, 0);
                }
            }
            else
            {
                offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
                transform.position = player.position + offsetRotation;
                transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
            }
        }
    }
}
