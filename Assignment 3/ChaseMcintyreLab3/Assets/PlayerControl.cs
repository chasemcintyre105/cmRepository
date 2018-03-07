using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    private CharacterController characterController;
    private Camera cam;

    private float speed = 6.0f;
    private float sensetivity = 120.0f;
    private float cameraHeight = 0.5f;
    private Vector3 direction = Vector3.zero;
    private float gravity = 20.0f;
    private float jumpSpeed = 10.0f;

	// Use this for initialization
	void Start () {
        characterController = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(0, cameraHeight, 0);
        cam.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 moveInput = (transform.forward * Input.GetAxis("Vertical") + 
                            transform.right * Input.GetAxis("Horizontal"))
                             * speed;
        direction.x = moveInput.x;
        direction.z = moveInput.z;
        //if(characterController.isGrounded)
        //{
            if(Input.GetKey("space"))
            {
                direction.y = jumpSpeed;
            }
            else if (Input.GetKey("left shift"))
            {
                direction.y -= jumpSpeed;
            }
            else
            {
                direction.y = 0;
            }
        //}
        characterController.Move(direction * Time.deltaTime);
        direction.y -= gravity * Time.deltaTime;

        float mouseX = Input.GetAxis ("Mouse X");
        float mouseY = -Input.GetAxis ("Mouse Y");
        transform.Rotate(0, mouseX * sensetivity * Time.deltaTime, 0);
        cam.transform.Rotate(mouseY * sensetivity * Time.deltaTime, 0, 0);
	}
}
