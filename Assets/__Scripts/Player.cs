using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using InControl;

public class Player : MonoBehaviour
{
	public int playerNum;
	public int colorNum;

	public int darkAlpha = 18;
	public int brightAlpha = 55;

	public Text playerStatus;
	//Weapon text fields
	public PlayerActions Actions { get; set; }

	public Text[] question;

	public GameObject mainCtrl;
	public Ctrl ctrl;
	public CharSelectCtrl sCtrl;

	public bool choosing = true;
	public bool second = false;
	public bool readyToPlay;

	public int fieldNum = 0;
	public int[] currentPartNum;
	
	void Awake() {
	}
	
	void OnDisable()
	{
		playerStatus.text = "Press X to Join";
		if (ctrl) {
			ctrl.SendMessage ("PlayerUnReady", playerNum);
		}
		if (Actions != null)
		{
			Actions.Destroy();
		}

	}
	
	void Start()
	{
		ctrl = GameObject.Find ("_MainController").GetComponent<Ctrl> ();

		playerStatus = ctrl.playerSelectText [playerNum];
		GetComponent<SpriteRenderer> ().sprite = ctrl.playerSprites [playerNum];

		//Player is choosing weapons
		choosing = true;

	}

	void Update()
	{
		if (choosing) {
			if (Actions.Green.WasPressed) {
				if(second) {
					choosing = false;
					playerStatus.text = string.Format ("Player {0} Ready!", playerNum+1);
					ctrl.SendMessage ("PlayerReady", playerNum);
					ctrl.PlayReadySound ();
				}
			}
			if (choosing) {
				playerStatus.text = "Press X again!";
				second = true;
				ctrl.goBack = false;
			}
		}
		if (Actions.Start.WasPressed) {
			if (ctrl.GetComponent<PlayerManager>().readyToPlay == true) {
				ctrl.selectionDone = true;
				if (ctrl.wepSelect) {
					//ctrl.PlayStartSound ();
				}
				ctrl.wepSelect = false;
			} else {
				ctrl.selectionDone = false;
			}
		}

	}
	
	public void Unready () {
		choosing = true;
		playerStatus.text = "Press X to Ready";
	}
}

