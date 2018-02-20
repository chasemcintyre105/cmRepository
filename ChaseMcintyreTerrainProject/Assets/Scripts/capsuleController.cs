using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class capsuleController : MonoBehaviour {


    private CharacterController con;
    private Camera cam;
    private Vector3 offset;
    public GameObject PlayerObject;


    private float speed = 6.0f;
    private float sensetivity = 120.0f;
    private float cameraHeight = 0.5f;
    private float gravity = 20.0f;
    private float jumpSpeed = 10.0f;
    private float rotationZ = 0f;
    private float sensetivityZ = 2f;

    private Vector3 direction = Vector3.zero; //new Vector3(0,0,0)

	// Use this for initialization
	void Start ()
    { 
        //offset = transform.position - PlayerObject.transform.position;
        con = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(0, (float) (cameraHeight + 1), (float)-1.5);
        cam.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up); //sets the camera to look forward out of the players face
	}
	
	// Update is called once per frame
	void Update ()
    {
        //get player input movement
        Vector3 playerMovementInput = (transform.forward * Input.GetAxis("Vertical")
                                     + transform.right * Input.GetAxis("Horizontal"))
                                       *  speed;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        float rotX = cam.transform.eulerAngles.x;
        if (rotX > 180)
        {
            rotX -= 360;
        }
        Debug.Log(-90 - rotX);
        //Debug.Log(90 - rotX);
        mouseY = mouseY * sensetivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -90-rotX, 90 - rotX);
        //move the player
        direction.x = playerMovementInput.x;
        direction.z = playerMovementInput.z;
        

        if (con.isGrounded)
        {
            if (Input.GetKey("space"))
            {
                direction.y = jumpSpeed;
            }
            else
            {
                direction.y = 0;
            }
        }
        con.Move(direction * Time.deltaTime);
        direction.y -= gravity * Time.deltaTime;

        //Look

        transform.Rotate(0, mouseX * sensetivity * Time.deltaTime, 0);
        cam.transform.Rotate(mouseY, 0, 0);

        //cam.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
    }
}
