using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;

public class Trigger : MonoBehaviour {

	public Text ESCtext;

	void Start(){

		ESCtext.text = "";
	}


/*	void OnTriggerEnter(Collider other){

		if(other.gameObject.CompareTag("Astronaut")){
			ESCtext.text = "Press the ESC key to press the button";
		}
	}*/


	void OnTriggerEnter(Collider other) {

		if(other.gameObject.tag == "Astronaut"){
			ESCtext.text = "Press the ESC key to press the button";
		}

	}

	void OnTriggerExit(Collider other){
		ESCtext.text = "";
	}
}