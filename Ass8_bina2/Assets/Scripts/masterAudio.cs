using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class masterAudio : MonoBehaviour {

	public Camera cam;
	private settingsManager sm;
	private Slider slider;
	void Start()
	{
		sm = cam.GetComponent<settingsManager>();
		slider = GetComponent<Slider>();
	}
	// Use this for initialization
	public void setVolume(float newVolume)
	{
		sm.setVolume(newVolume);
	}
	public void setNewVolume()
	{
		Debug.Log(slider.value);
		setVolume(slider.value);
	}

}
