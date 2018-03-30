using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouseover : MonoBehaviour {

	public Text ESCtext;

	void Start(){
		
		ESCtext.text = "";
	}

	void Update(){
		if (Input.GetMouseButtonDown(0)) {
			ESCtext.text = "Press the ESC key to press the button";
		}
	}

	void OnTriggerEnter(Collider other){

		if(other.gameObject.CompareTag("Astronaut")){
			ESCtext.text = "Press the ESC key to press the button";
		}
	}
}
