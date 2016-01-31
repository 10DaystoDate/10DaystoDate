using System;
using UnityEngine;
using InControl;
using UnityEngine.EventSystems; 
using UnityEngine.UI;


public class Button : MonoBehaviour
{
	//Renderer cachedRenderer;

	public Button up = null;
	public Button down = null;
	public Button left = null;
	public Button right = null;
	public Button link = null;
	public Text thisText;
	public RectTransform thisRect;
	public GameObject buttonMan;
	public bool arenaMenu;


	void Start()
	{
		//cachedRenderer = GetComponent<Renderer>();
		if (arenaMenu && transform.parent.name != "ArenaCanvas") {
			thisRect = GetComponent<RectTransform>();
		} else {
			thisText = GetComponent<Text> ();
		}
		//buttonMan = GameObject.Find ("CanvasMainMenu");

	}


	void Update()
	{
		// Find out if we're the focused button.
		bool hasFocus = buttonMan.GetComponent<ButtonManager>().focusedButton == this;

		if (arenaMenu && transform.parent.name != "ArenaCanvas") {
			var sizeX = thisRect;
			sizeX.localScale = new Vector3(Mathf.MoveTowards (sizeX.localScale.x, hasFocus ? 1.3f : 1f, Time.deltaTime * 3.0f),Mathf.MoveTowards (sizeX.localScale.y, hasFocus ? 1.3f : 1f, Time.deltaTime * 3.0f),sizeX.localScale.z);
			thisRect.localScale = sizeX.localScale;
		} else {
			// Fade alpha in and out depending on focus.
			var colorX = thisText.color;
			colorX.a = Mathf.MoveTowards (colorX.a, hasFocus ? 1.0f : 0.5f, Time.deltaTime * 3.0f);
			//cachedRenderer.material.color = color;

		
			thisText.color = colorX;
		}
	}
}