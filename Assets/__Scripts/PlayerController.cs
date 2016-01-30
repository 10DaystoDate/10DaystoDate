using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	//Player number, only thing different for each player
	public int playerNum = 0;
	public GameObject mainController;
	public GameObject[] weaponSet;
	public GameObject playerInput;
	public Player pIn;

	public Ctrl ctrl;

	void Awake () {
		//Get ship variables from main controller
		mainController = GameObject.Find ("_MainController");
		ctrl = mainController.GetComponent<Ctrl>();
	}

	void Start () {
		pIn = playerInput.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		//Bool controls
		bool up = pIn.Actions.Up.WasPressed;
		bool down = pIn.Actions.Down.WasPressed;
		bool left = pIn.Actions.Left;
		bool right = pIn.Actions.Right;
		bool green = pIn.Actions.Green.WasPressed;


		//Movement
		if (up) {
			//move selector up
		} else if (down) {
			//yes
		}
		if (green) {
			//Accept question
		}

		/*
		if (Time.time >= resetSpeedTime) {
			speed = origSpeed;
			minSpeed = origMinSpeed;
			canTurn = true;
		}*/

	}
}
