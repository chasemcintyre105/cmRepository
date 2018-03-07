using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

	public Vector3 movePos;
	public float speed = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if(Input.GetMouseButton(0) ){

			if (Physics.Raycast (ray, out hit, Mathf.Infinity, ~(1 << 10))) {
				movePos = new Vector3 (hit.point.x, transform.position.y, hit.point.z);
			}

			speed = Mathf.Clamp(speed += 0.1f, 0,5);
			Vector3 direction = new Vector3(movePos.x - transform.position.x, 0f, movePos.z - transform.position.x);
			transform.rotation = Quaternion.LookRotation(direction);
		}else{
			if(isActiveAndEnabled.Equals(false)){
				speed = Mathf.Clamp(speed -= 0.1f, 0,5);
			}
		}

		transform.Translate(Vector3.forward*speed*Time.deltaTime);
		transform.Rotate(Vector3.forward*speed*Time.deltaTime);
	}
}
