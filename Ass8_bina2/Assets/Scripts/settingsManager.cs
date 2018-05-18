using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsManager : MonoBehaviour {

	// Use this for initialization
	public void setVolume(float newVolume)
	{
		AudioListener.volume = newVolume;
	}
}