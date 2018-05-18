using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsToggle : MonoBehaviour {

	public GameObject go;

	public void setDisabled()
	{
		if(go.activeSelf)
		{
			go.SetActive(false);
		}
		else
		{
			go.SetActive(true);
		}
	}

}
