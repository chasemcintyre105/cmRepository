using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class chestDialogueManager : MonoBehaviour {

	public GameObject playerSword;
	public GameObject chestDialogueBox;
	public Text chestDialougueText;
	public RigidbodyFirstPersonController player;

	public bool dialogueActive;
	public bool talkingToChest;
	public int currentline;
	public string[] chestDialoguelines;


	// Use this for initialization
	void Start () {
		chestDialogueBox.SetActive (false);
		playerSword.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		chestDialougueText.text = chestDialoguelines [currentline];
		if (Input.GetKey ("e")) {
			playerSword.SetActive (true);
		}
	}

	void OnTriggerEnter(Collider other) {

		if(other.gameObject.tag == "DialogueTrigger1"){
			currentline = 0;
			dialogueActive = true;
			talkingToChest = true;
			chestDialogueBox.SetActive(true);
		}

	}

	void OnTriggerExit(Collider other){
		talkingToChest = false;
		chestDialogueBox.SetActive (false);
	}


}
