using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using InControl;


// This example iterates on the basic multiplayer example by using action sets with
// bindings to support both joystick and keyboard players. It would be a good idea
// to understand the basic multiplayer example first before looking a this one.
//
public class PlayerManager : MonoBehaviour
{
	public GameObject playerPrefab;
	//Reference to Ctrl MainCttrlr
	public GameObject mainCtrl;
	public Ctrl ctrl;
	//Boolean array for player joined and ready
	public bool[] playerJoined = new bool[] {false,false,false,false};
	public bool[] playerReady = new bool[] {false,false,false,false};

	//public GameObject[] playerInputs;

	public AudioClip joinSound;
	public AudioClip leaveSound;
	private float volume;

	public bool readyToPlay = false; //All players are ready to play
	
	public int arrayMatches = 0;
	
	public Text pressStart;
	public GameObject startPanel;

	const int maxPlayers = 4;
	
	public List<Player> players = new List<Player>( maxPlayers );
	
	PlayerActions keyboardListener1;
	PlayerActions keyboardListener2;
	PlayerActions joystickListener;
	
	void Awake () {
		mainCtrl = GameObject.Find ("_MainController");
		ctrl = mainCtrl.GetComponent<Ctrl> ();
		volume = PlayerPrefs.GetFloat ("SndVol");
	}
	void OnEnable()
	{
		InputManager.OnDeviceDetached += OnDeviceDetached;

		keyboardListener1 = PlayerActions.CreateWithKeyboardBindings1();
		keyboardListener2 = PlayerActions.CreateWithKeyboardBindings2();

		joystickListener = PlayerActions.CreateWithJoystickBindings();
	}
	
	
	void OnDisable()
	{
		InputManager.OnDeviceDetached -= OnDeviceDetached;
		joystickListener.Destroy();
		keyboardListener1.Destroy();
		keyboardListener2.Destroy();
	}
	
	
	void Update()
	{
		if (ctrl.wepSelect) {
			if (JoinButtonWasPressedOnListener (joystickListener)) {
				var inputDevice = InputManager.ActiveDevice;

				if (ThereIsNoPlayerUsingJoystick (inputDevice)) {
					CreatePlayer (inputDevice, 0);
				}
			}
		
			if (JoinButtonWasPressedOnListener (keyboardListener1)) {
				if (ThereIsNoPlayerUsingKeyboard1 ()) {
					CreatePlayer (null, 1);
				}
			}
			if (JoinButtonWasPressedOnListener (keyboardListener2)) {
				if (ThereIsNoPlayerUsingKeyboard2 ()) {
					CreatePlayer (null, 2);
				}
			}
		}
		if (LeaveButtonWasPressedOnListener( joystickListener ))
		{
			var inputDevice = InputManager.ActiveDevice;
			
			var player = FindPlayerUsingJoystick( inputDevice );
			if (player != null) {
				if (ctrl.wepSelect) {
					if (player.choosing) {
						RemovePlayer (player);
					} else {
						player.Unready ();
						PlayerUnReady (player.playerNum);
					}
				} else {
					if (ctrl.selectionDone && ctrl.ready) {
						ctrl.selectionDone = false;
						ctrl.wepSelect = true;
					}
				}
			}
		}
		if (LeaveButtonWasPressedOnListener( keyboardListener1 ))
		{

			//var inputDevice = InputManager.ActiveDevice;

			var player = FindPlayerUsingKeyboard1();
			if (player != null) {
				if(ctrl.wepSelect) {
					if(player.choosing) {
						RemovePlayer( player );
					} else {
						player.Unready ();
						PlayerUnReady (player.playerNum);
					}
				}
			}
		}
		if (LeaveButtonWasPressedOnListener( keyboardListener2 ))
		{

			//var inputDevice = InputManager.ActiveDevice;

			var player = FindPlayerUsingKeyboard2();
			if (player != null) {
				if(ctrl.wepSelect) {
					if(player.choosing) {
						RemovePlayer( player );
					} else {
						player.Unready ();
						PlayerUnReady (player.playerNum);
					}
				}
			}
		}
		if (players.Count > 1) { //If 2 or more players joined
			arrayMatches = 0;
			for (int i = 0; i < 4; i++) {
				if (playerJoined [i] == playerReady [i]) {
					arrayMatches += 1;
				}
			}
			if (arrayMatches == 4) {
				pressStart.text = "Press Start!";
				readyToPlay = true;
			} else {
				pressStart.text = "Waiting for Players...";
				//startPanel.SetActive (true);
				readyToPlay = false;
			}
		}
	}
	
	
	bool JoinButtonWasPressedOnListener( PlayerActions actions )
	{
		return actions.Green.WasPressed /*|| actions.Blue.WasPressed || actions.Yellow.WasPressed*/;
	}
	
	bool LeaveButtonWasPressedOnListener( PlayerActions actions )
	{
		return actions.Red.WasPressed;
	}
	
	Player FindPlayerUsingJoystick( InputDevice inputDevice )
	{
		var playerCount = players.Count;
		for (int i = 0; i < playerCount; i++)
		{
			var player = players[i];
			if (player.Actions.Device == inputDevice)
			{
				return player;
			}
		}
		
		return null;
	}
	
	
	bool ThereIsNoPlayerUsingJoystick( InputDevice inputDevice )
	{
		return FindPlayerUsingJoystick( inputDevice ) == null;
	}
	bool ThereIsAPlayerUsingJoystick( InputDevice inputDevice )
	{
		return FindPlayerUsingJoystick( inputDevice ) != null;
	}
	
	
	Player FindPlayerUsingKeyboard1()
	{
		var playerCount = players.Count;
		for (int i = 0; i < playerCount; i++)
		{
			var player = players[i];
			if (player.Actions == keyboardListener1) {
				return player;
			}
		}
		
		return null;
	}
	Player FindPlayerUsingKeyboard2()
	{
		var playerCount = players.Count;
		for (int i = 0; i < playerCount; i++)
		{
			var player = players[i];
			if (player.Actions == keyboardListener2) {
				return player;
			}
		}

		return null;
	}
	
	
	bool ThereIsNoPlayerUsingKeyboard1()
	{
		return FindPlayerUsingKeyboard1() == null;
	}
	bool ThereIsNoPlayerUsingKeyboard2()
	{
		return FindPlayerUsingKeyboard2() == null;
	}
	
	
	void OnDeviceDetached( InputDevice inputDevice )
	{
		var player = FindPlayerUsingJoystick( inputDevice );
		if (player != null)
		{
			RemovePlayer( player );
		}
	}
	
	
	Player CreatePlayer( InputDevice inputDevice, int keyboardNum )
	{
		if (players.Count < maxPlayers)
		{
			// Pop a position off the list. We'll add it back if the player is removed.
			
			var gameObject = (GameObject) Instantiate( playerPrefab, ctrl.playerSelectText[FindPlayer ()].transform.position - Vector3.forward - Vector3.up*1.5f, Quaternion.identity );
			var player = gameObject.GetComponent<Player>();

			int emptySlot = FindPlayer ();
			player.playerNum = emptySlot;
			playerJoined[emptySlot] = true;
			ctrl.playerInput[emptySlot] = gameObject;
			//PlayerPrefs.SetInt ("NumOfPlayers", (players.Count+1)); //Sets number of players for next scene
			//Debug.Log (players.Count+1);
			
			if (inputDevice == null)
			{
				// We could create a new instance, but might as well reuse the one we have
				// and it lets us easily find the keyboard player.
				if (keyboardNum == 1) {
					player.Actions = keyboardListener1;
					Debug.Log ("ONE");
				} else if (keyboardNum == 2){
					Debug.Log ("TWO");
					player.Actions = keyboardListener2;
				}
			}
			else
			{
				// Create a new instance and specifically set it to listen to the
				// given input device (joystick).
				var actions = PlayerActions.CreateWithJoystickBindings();
				actions.Device = inputDevice;
				
				player.Actions = actions;
			}
			
			players.Add( player );
			GetComponent<AudioSource>().PlayOneShot(joinSound, volume); // Play the AudioClip
			
			return player;
		}
		
		return null;
	}
	
	
	void RemovePlayer( Player player )
	{
		var playerRef = player.GetComponent<Player>();
		playerJoined[playerRef.playerNum] = false; //set player number to false
		PlayerPrefs.SetInt ("NumOfPlayers", (players.Count));
		players.Remove( player );
		player.Actions = null;
		Destroy( player.gameObject );
		GetComponent<AudioSource>().PlayOneShot(leaveSound, volume); // Play the AudioClip
	}

	public int FindPlayer () { //Finds empty player slot in bool array
		for (int i = 0; i < 4; i++) {
			if (playerJoined[i] == false) {
				return i;
			}
		}
		return 5;
	}
	
	public void PlayerReady (int playerNum) {
		playerReady [playerNum] = true;
	}
	public void PlayerUnReady (int playerNum) {
		playerReady [playerNum] = false;
	}
	
	
	/*void OnGUI()
	{
		const float h = 22.0f;
		var y = 10.0f;
		
		GUI.Label( new Rect( 10, y, 300, y + h ), "Active players: " + players.Count + "/" + maxPlayers );
		y += h;
		
		if (players.Count < maxPlayers)
		{
			GUI.Label( new Rect( 10, y, 300, y + h ), "Press a button or a/s/d/f key to join!" );
			y += h;
		}
	}*/
}