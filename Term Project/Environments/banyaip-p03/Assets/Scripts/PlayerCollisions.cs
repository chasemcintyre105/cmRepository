using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour {

	// Logic for the Outpost Door
	private GameObject 		currentDoor;
	private bool 			doorIsOpen 		= false;
	private float 			doorTimer 		= 0.0f;
	public float 			doorOpenTime 	= 3.0f;

	// Door sounds
	private new AudioSource audio;
	public AudioClip doorOpenSound;
	public AudioClip doorShutSound;

	// Battery Collect Sound
	public AudioClip batteryCollectSound;

	// Use this for initialization
	void Start ()
	{
		audio = GetComponent<AudioSource>();	
	}
	// Update is called once per frame
	void Update ()
	{
		// Automatically shut door if it's left open
		if (doorIsOpen)
		{
			doorTimer += Time.deltaTime;
		}
		if (doorTimer > doorOpenTime)
		{
			doorTimer = 0.0f;
			ShutDoor ();
		}
	}

	// Shut door function
	void ShutDoor()
	{
		// Play audio
		audio.PlayOneShot (doorShutSound);
		// Set doorIsOpen to false
		doorIsOpen = false;
		// Declare objects that contain animation
		GameObject myOutpost = GameObject.Find ("outpost");
		// Play Animation
		myOutpost.GetComponent<Animation> ().Play ("doorshut");
	}

	// Collision detection
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if(hit.gameObject.tag == "outpostDoor" && !doorIsOpen && BatteryCollect.charge >=4)
		{
			OpenDoor ();
			BatteryCollect.batteryGui.enabled = false;
		}
		else if (hit.gameObject.tag == "outpostDoor" && !doorIsOpen && BatteryCollect.charge <4)
		{
			BatteryCollect.batteryGui.enabled = true;
			TextHints.message = "The door seems to need more power....";
			TextHints.textOn = true;
		}
	}

	// Open door function
	void OpenDoor ()
	{

		// Play audio
		audio.PlayOneShot(doorOpenSound);
		// Set doorIsOpen to true
		doorIsOpen = true;
		// Declare objects thar contain animation
		GameObject myOutpost = GameObject.Find("outpost");
		// Play animation
		myOutpost.GetComponent<Animation>().Play("dooropen");
	}

	//Battery collision
	void OnTriggerEnter (Collider coll)
	{
		if(coll.gameObject.tag == "battery")
		{
			BatteryCollect.charge++; //Increment charge variable by 1
			audio.PlayOneShot(batteryCollectSound);
			Destroy (coll.gameObject); // Delete battery
		}
	}






}
