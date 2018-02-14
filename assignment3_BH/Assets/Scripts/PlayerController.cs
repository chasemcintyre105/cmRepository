using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private float speed = 15f ;


	private Rigidbody rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();

	
	}

	void Update(){
		Vector3 mPosition = Input.mousePosition; // mouse position return
		Vector3 oPosition = transform.position; // obeject position return

		mPosition.z = oPosition.x - Camera.main.transform.position.z;

		Vector3 target = Camera.main.ScreenToWorldPoint (mPosition);

		float dy = target.y - oPosition.y;
		float dx = target.x - oPosition.x;

		float rotateDegree = Mathf.Atan2 (dy, dx) * Mathf.Rad2Deg;

		transform.rotation = Quaternion.Euler (0f, 0f, rotateDegree);

	}


	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		rb.AddForce (movement * speed);
	}




}