using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Thirdcamera : MonoBehaviour {

	public Transform target;
	public float dist = 5;

	public float xSpeed = 220.0f;
	public float ySpeed = 100.0f;

	private float x = 0.0f;
	private float y = 0.0f;

	public float yMinLimit = -22f;
	public float yMaxLimit = 25f;

	float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}

	private Transform cam;
	// Use this for initialization
	void Start () {
		cam = GetComponent<Transform> ();
		//Cursor.lockState = CursorLockMode.Locked;
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate()
	{
		if (target) {
			dist -= .5f * Input.mouseScrollDelta.y;

			if (dist < 0.5) {
				dist = 1;
			}
			if (dist >= 10) {
				dist = 10;
			}
			x += Input.GetAxis ("Mouse x") * xSpeed * 0.02f;
			y -= Input.GetAxis ("Mouse y") * ySpeed * 0.02f;


			y = ClampAngle (y, yMinLimit, yMaxLimit);


			Quaternion rotation = Quaternion.Euler (y, x, 0);
			Vector3 position = rotation * new Vector3 (0, 0.0f, -dist) + target.position + new Vector3 (0.0f, 0, 0.0f);

		}
	}

}
