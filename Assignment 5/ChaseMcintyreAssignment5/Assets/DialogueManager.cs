using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class DialogueManager : MonoBehaviour {


	public GameObject DialogueBox;
	public Text DialougueText;
	public RigidbodyFirstPersonController player;

	public bool dialogueActive;
	public bool talkingToSkeleton;
	public int currentline;
	public string[] dialoguelines;


	// Use this for initialization
	void Start () {
		DialogueBox.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		DialougueText.text = dialoguelines [currentline];
	}

	void OnTriggerEnter(Collider other) {

		if(other.gameObject.tag == "DialogueTrigger1"){
			currentline = 0;
			dialogueActive = true;
			talkingToSkeleton = true;
			DialogueBox.SetActive(true);
		}

	}

	void OnTriggerExit(Collider other){
		talkingToSkeleton = false;
		DialogueBox.SetActive (false);
	}


}
