using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour {

	public float rotationSpeed = 200.0f;

	// Update is called once per frame
	void Update () 
	{
		// Rotate any object on the Y-axis * rotationSpeed
		transform.Rotate (new Vector3(0,1,0), rotationSpeed * Time.deltaTime);		
	}
}
