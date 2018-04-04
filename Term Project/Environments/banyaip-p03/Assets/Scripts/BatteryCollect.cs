using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryCollect : MonoBehaviour {

	public static int charge = 0; // Charge will be incremented when batteries are collected

	// Holds the images
	public static Image batteryGui;

	// Battery Sprites
	public Sprite charge1tex;
	public Sprite charge2tex;
	public Sprite charge3tex;
	public Sprite charge4tex;
	public Sprite charge0tex;

	// Use this for initialization
	void Start () 
	{
		// Find the Image GameObject
		batteryGui = gameObject.GetComponentInChildren<Image>();
		// Hide Battery GUI on game start
		batteryGui.enabled = false;
		// Set the initial charge amount
		charge = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(charge==1)
		{
			batteryGui.sprite = charge1tex; // Change Sprite
			batteryGui.enabled = true; // Make Battery GUI visible
		} else if (charge==2)
		{
			batteryGui.sprite = charge2tex;
		} else if (charge==3)
		{
			batteryGui.sprite = charge3tex;
		} else if (charge==4)
		{
			batteryGui.sprite = charge4tex;
		} else
		{
			batteryGui.sprite = charge0tex;
		}
	}
}
