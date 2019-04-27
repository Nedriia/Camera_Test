using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Behavior : MonoBehaviour
{
    public Vector3 velocity;
    public float smoothTime;

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

    UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter playerRb;

    private struct PointInSpace
    {
        public Vector3 Position;
        public float Time;
    }

    public float delay;
    private Queue<PointInSpace> pointsInSpace = new Queue<PointInSpace>();

    //Choose the camera we want
    public bool cameraModeDelay;

    public float rotationDiff;
    public float distance;
    public float startY;

    public void Start()
    {
        playerRb = player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
        offsetRotation = transform.position;
        transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
        distance = Vector3.Distance(player.position, transform.position);
        startY = transform.position.y;
    }

    public void Update()
    {
        Debug.DrawRay(new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z), player.transform.TransformDirection(Vector3.forward) *  playerRb.m_Rigidbody.velocity.magnitude, Color.yellow);
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

        if (cameraModeDelay)
        {
            offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
            //Add the current target position to the list of positions
            pointsInSpace.Enqueue(new PointInSpace() { Position = player.transform.position + offsetRotation, Time = Time.time });
            //Debug.Log(pointsInSpace.Dequeue().Position);
            //Move the camera to the position to the list of positions
            while (pointsInSpace.Count > 0 && pointsInSpace.Peek().Time <= Time.time - delay + Mathf.Epsilon)
            {
                transform.position = Vector3.SmoothDamp(transform.position, pointsInSpace.Dequeue().Position, ref velocity, smoothTime);
                transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
            }
        }
        else
        {
            if (Input.GetAxis("Mouse X") == 0 && player.GetComponent<Rigidbody>().velocity.magnitude >= threesholdMovement)
            {
                float playerAngle = AngleOnXZPlane(player);
                float cameraAngle = AngleOnXZPlane(transform);
                rotationDiff = Mathf.DeltaAngle(cameraAngle, playerAngle);

                if (rotationDiff > -40 || rotationDiff <40)
                {
                    player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = player.transform;
                    var positionCamera = player.position - (player.forward * distance);
                    positionCamera = new Vector3(positionCamera.x, positionCamera.y + startY, positionCamera.z);                  
                    transform.position = Vector3.SmoothDamp(transform.position, positionCamera, ref velocity, smoothTime);
                    transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
                } 
                else
                {
                    //We need to rotate the camera here not before
                    //Before : Follow the play but with a latence in the rotation
                }
            }
            else
            {
                player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = Camera.main.transform;
                transform.position = Vector3.SmoothDamp(transform.position, player.position + offsetRotation, ref velocity, smoothTime);
                transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
            }
        }
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
            offsetVertical.y = Mathf.Clamp(offsetVertical.y, 0.1f, 15);
            
            var newPosition = new Vector3(player.position.x + offsetRotation.x,offsetVertical.y,player.position.z + offsetRotation.z);
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
            transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
        }
        else
        {
            player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = Camera.main.transform;
            if (player.GetComponent<Rigidbody>().velocity.magnitude <= threesholdMovement)
            {
                transform.position = Vector3.SmoothDamp(transform.position, player.position + offsetRotation, ref velocity, smoothTime * 2);
                transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));

                if (Input.GetAxis("Mouse X") < 0)
                {
                    offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
                    transform.position = Vector3.SmoothDamp(transform.position, player.position + offsetRotation, ref velocity, smoothTime * 4);
                    transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
                }
                else if (Input.GetAxis("Mouse X") > 0)
                {
                    offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
                    transform.position = Vector3.SmoothDamp(transform.position, player.position + offsetRotation, ref velocity, smoothTime * 4);
                    transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
                }
            }
            else
            {
                if(!cameraModeDelay)
                {
                    offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
                }
            }
        }
    }

    private float AngleOnXZPlane(Transform item)
    {
        // get rotation as vector (relative to parent)
        Vector3 direction = item.rotation * item.forward;

        // return angle in degrees when projected onto xz plane
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}
