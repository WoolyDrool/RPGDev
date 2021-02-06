using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFPSController : MonoBehaviour
{
    public float moveSpeed = 5;
    public float mouseSensitivity = 3;
    public float jumpSpeed = 7;
    public float clampAngle = 80;
    Vector3 velocity = Vector3.zero;
    float verticalRotation = 0;
    float verticalVelocity = 0;
    public Transform camHolder;

    CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float rotLeftright = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, rotLeftright, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);
        camHolder.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        
        // Movement
        float forwardSpeed = Input.GetAxisRaw("Vertical");
        float sideSpeed = Input.GetAxisRaw("Horizontal");

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        if(cc.isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = jumpSpeed;
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);

        speed = transform.rotation * speed;


        Vector3 forwardMovement = transform.forward * forwardSpeed;
        Vector3 sideMovement = transform.right * sideSpeed;
        cc.SimpleMove(Vector3.ClampMagnitude(forwardMovement+sideMovement, 1.0f) * moveSpeed);
    }
}
