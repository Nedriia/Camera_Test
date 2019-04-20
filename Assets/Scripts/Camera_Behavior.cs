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

    public float zoomForce;
    public float minFov, maxFov;

    public static float t = 0.0f;
    public float decreaseZoomForce;

    public void Start()
    {
        offsetRotation = transform.position;     
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player.GetComponent<Rigidbody>().velocity.magnitude > threesholdMovement && Camera.main.fieldOfView != 60)
        {
            t += decreaseZoomForce * Time.deltaTime;
            var fov = Mathf.Lerp(Camera.main.fieldOfView, 60, t);
            Camera.main.fieldOfView = fov;
        }

        transform.LookAt(new Vector3(player.position.x,player.position.y + offsetVerticalFocus, player.position.z));
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref velocity, smoothTime);

        RotationControl();

        Zoom();
    }

    public void Zoom()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            t = 0;
            var fov = Camera.main.fieldOfView;
            fov -= Input.GetAxis("Mouse ScrollWheel") * zoomForce;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }   

        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            t = 0;
            var fov = Camera.main.fieldOfView;
            fov -= Input.GetAxis("Mouse ScrollWheel") * zoomForce;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }
    }

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
            player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = player.transform;
            offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
            offsetVertical = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.left) * offsetVertical;
            
            transform.position = new Vector3(player.position.x + offsetRotation.x,player.position.y + offsetVertical.y,player.position.z + offsetRotation.z);
            transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
        }
        else
        {
            player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = Camera.main.transform;
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
