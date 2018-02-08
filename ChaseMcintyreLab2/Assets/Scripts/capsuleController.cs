using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class capsuleController : MonoBehaviour {


    private CharacterController con;
    private Camera cam;

    public float speed;
    public float jumpSpeed;
    public float sensetivity;
    public float cameraHeight;
    public float gravity;

    private Vector3 direction = Vector3.zero; //new Vector3(0,0,0)

	// Use this for initialization
	void Start ()
    {
        con = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(0, cameraHeight, 0);
        cam.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up); //sets the camera to look forward out of the players face
	}
	
	// Update is called once per frame
	void Update ()
    {
        //get player input movement
        Vector3 playerMovementInput = (transform.forward * Input.GetAxis("Vertical")
            + transform.right * Input.GetAxis("Horozontal")) *  speed;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

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
        cam.transform.Rotate(mouseY * sensetivity * Time.deltaTime, 0, 0);
		
	}
}
