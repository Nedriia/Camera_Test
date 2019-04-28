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

    public bool isMoving;
    public float timer, timerBeforeTrigger;

    public void Start()
    {
        playerRb = player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
        offsetRotation = transform.position;
        transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
        distance = Vector3.Distance(player.position, transform.position);
        //TODO: Change that
        startY = transform.position.y;
    }

    public void Update()
    {
        Debug.DrawRay(new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z), player.transform.TransformDirection(Vector3.forward) *  playerRb.m_Rigidbody.velocity.magnitude, Color.yellow);

        float playerAngle = AngleOnXZPlane(player);
        float cameraAngle = AngleOnXZPlane(transform);
        rotationDiff = Mathf.DeltaAngle(cameraAngle, playerAngle);
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

        //TODO: Can move to the vertical
        //TODO: Don't use the clamp method anymore, but use collision detection of the floor
        //TODO: Does Get Component are really needed
        player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = Camera.main.transform;

        if (player.GetComponent<Rigidbody>().velocity.magnitude <= threesholdMovement)
        {
            /*player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().m_Cam = player.transform;
            offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;
            offsetVertical = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.left) * offsetVertical;
            offsetVertical.y = Mathf.Clamp(offsetVertical.y, 0.1f, 15);

            var newPosition = new Vector3(player.position.x + offsetRotation.x, offsetVertical.y, player.position.z + offsetRotation.z);
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
            transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));*/
        }
        else
        {
            if (Input.GetAxis("Mouse X") == 0)
            {
                var positionCamera = player.position - (player.forward * distance);
                positionCamera = new Vector3(positionCamera.x, positionCamera.y + startY, positionCamera.z);
                transform.position = Vector3.SmoothDamp(transform.position, positionCamera, ref velocity, smoothTime);
                transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));

                timer += Time.deltaTime;
                if (timer > timerBeforeTrigger)
                {
                    isMoving = true;
                    timer = timerBeforeTrigger;
                }
                    
            }
                
            else
            {
                if(isMoving)
                {
                    var positionCamera = player.position - (player.forward * distance);
                    positionCamera = new Vector3(positionCamera.x, positionCamera.y + startY, positionCamera.z);
                    transform.position = Vector3.SmoothDamp(transform.position, positionCamera, ref velocity, smoothTime);
                    transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));

                    offsetRotation = positionCamera;

                    isMoving = false;
                    timer = 0;
                }
                var newPosition = player.position + (offsetRotation.normalized * distance);
                transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
                offsetRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetRotation;                
                transform.LookAt(new Vector3(player.position.x, player.position.y + offsetVerticalFocus, player.position.z));
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
