using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    public Vector3 movement;
    public float speed, moveX, moveZ;

    float yRotation;

    public float jumpForce;
    Rigidbody rb_player;
    bool inAir;

    private void Awake()
    {
        rb_player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(moveX, moveZ);

        //Rotation of the pawn with the mouse
        if (!inAir) // Avoid Air control
            RotationControl();

        Jump();
    }

    void Move(float MoveX, float MoveZ)
    {
        //The move fonction let the player move with some inertia
        movement = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        movement.Normalize();
        movement *= speed * Time.fixedDeltaTime;
        transform.position += movement;
    }

    void RotationControl()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(Input.GetAxis("Mouse X")<0)
        {
            yRotation += Input.GetAxis("Mouse X");
            transform.eulerAngles = new Vector3(0, yRotation, 0);
        }
        else if(Input.GetAxis("Mouse X")>0)
        {
            yRotation += Input.GetAxis("Mouse X");
            transform.eulerAngles = new Vector3(0, yRotation, 0);
        }
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !inAir)
        {
            rb_player.velocity = new Vector3(0, jumpForce, 0);
            inAir = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        inAir = false;
    }
}
