using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MoveInput : MonoBehaviour
{
    private float speed = 7.0f;
    private float squeeze_cutoff = 0.9f;
    private float tilt_speed = 10.0f;

    private CharacterController controller;

    private SlotProjector slot_component;

    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction rotate;


    private Vector3 movement = new Vector3(0f,0f,0f);
    private float degspersec = 500f;

    private Quaternion target_rotation;

    public bool rotation_lock = false;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        /* disabled independent rotation of squad
        rotate = playerControls.Player.Rotate;
        rotate.Enable();
        rotate.performed += OnRotate;
        */
    }

 

    private void OnDisable()
    {
        
        move.Disable();
        
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        slot_component = GetComponent<SlotProjector>();
        target_rotation = transform.rotation;

    }

 

    void Update()
    {
     

        Vector2 movement_2D;
        movement_2D = move.ReadValue<Vector2>();
        movement = new Vector3(movement_2D.x, 0, movement_2D.y);


        if (movement.magnitude > 0 && controller.velocity.magnitude < (movement.magnitude * speed) * squeeze_cutoff)
        {
            slot_component.TiltProjectionPlane(movement, speed, tilt_speed);
        }
        else slot_component.Untilt_Projection_Plane(speed);

        //this allows independent squad rotation
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, target_rotation, degspersec * Time.deltaTime);

        //this make squad rotation follow movement
        if (movement.magnitude > 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), degspersec * Time.deltaTime);


        }
    }

    private void FixedUpdate()
    {
        controller.Move(movement * speed * Time.deltaTime);
    }


    private void OnRotate(InputAction.CallbackContext context)
    {
        if (!rotation_lock) 
        {
            Vector2 orient_vector2D;
            Vector3 orient_vector3D;
            orient_vector2D = rotate.ReadValue<Vector2>();
            orient_vector3D = new Vector3(orient_vector2D.x, 0f, orient_vector2D.y);

            if (orient_vector3D != Vector3.zero)
            {
                target_rotation = Quaternion.LookRotation(orient_vector3D);
                //lock rotation, yaw only
                target_rotation.eulerAngles = new Vector3(0, target_rotation.eulerAngles.y, 0);

            }

        }

        

        

       
        


    }
}


