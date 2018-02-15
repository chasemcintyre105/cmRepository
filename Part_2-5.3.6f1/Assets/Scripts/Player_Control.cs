using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour {

	private CharacterController controller;
	private Camera cam;

	private float speed = 6.0f; //move speed
	private float sensitivity = 120.0f; //look spees
	private float cameraHeight = 0.5f; // distance above center of player
	private Vector3 direction = Vector3.zero; //move direction
	private float gravity = 20.0f; //accel from gravity
	private float jumpSpeed = 10.0f; //initial vertical speed on jump

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
		cam = GetComponentInChildren<Camera>();
		cam.transform.localPosition = new Vector3 (0, cameraHeight, 0);
		cam.transform.rotation = Quaternion.LookRotation (transform.forward, transform.up);	
	}
	
	// Update is called once per frame
	void Update () {
		//move
		Vector3 moveInput = (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis ("Horizontal")) * speed;
		direction.x = moveInput.x;
		direction.z = moveInput.z;
		if (controller.isGrounded) {
			if (Input.GetKey ("space")) {
				direction.y = jumpSpeed;
			} else {
				direction.y = 0;
			}
		}
		controller.Move (direction * Time.deltaTime);
		direction.y -=gravity * Time.deltaTime;

		//Look
		float mouseX = Input.GetAxis ("Mouse X");
		float mouseY = -Input.GetAxis ("Mouse Y");
		transform.Rotate (0, mouseX * sensitivity * Time.deltaTime, 0);
		cam.transform.Rotate (mouseY * sensitivity * Time.deltaTime, 0, 0);
	}
}
