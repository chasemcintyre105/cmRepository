using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI Classes

public class TextHints : MonoBehaviour {

	// Text Hint Logic
	public static string message; // Message content
	static Text textHint; // Onscreen text
	public static bool textOn = false; // Checks to see if text is on
	private float timer = 0.0f; // Default timer time
	public float textOnTime = 5.0f; // Time that text stays onscreen

	// Use this for initialization
	void Start () 
	{
		textHint = GetComponent<UnityEngine.UI.Text> ();
		timer = 0.0f;
		textOn=false;
		textHint.text =" ";
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (textOn) {
			textHint.enabled = true;
			textHint.text = message;
			timer += Time.deltaTime;
		}
		if (timer >= textOnTime) {
			textOn = false;
			textHint.enabled = false;
			timer = 0.0f;
		}
	}
}
