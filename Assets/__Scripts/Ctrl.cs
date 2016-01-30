using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using InControl;

public class Ctrl : MonoBehaviour {


	public Text[] playerSelectText;

	public int[] gstats;


	public int numOfPlayers = 2;
	public GameObject[] playerSpawn;
	public GameObject playerShip;
	public int[] wepNum1 = new int[] {0,0,0};
	public GameObject[] playerCurrent;
	public GameObject[] playerInput;

	//Variables same across ships
	public GameObject[] weaponSet;

	public List<int> shipColorNum = new List<int>();
	public Color[] shipColor;
	public Color[] shipColorSet;
	
	public GameObject scorePanel;
	public GameObject startPanel;
	public GameObject pausePanel;
	//public GameObject missionPanel;
	public Text[] scoreTitleText;
	public Text[] scoreText;
	public Text selectTitle;
	public Text announcerText;
	public bool announced = false;
	//private float aT = 0; //lerp float for announcer text color
	public Button rematchButton;

	public AudioClip mapSound;
	public AudioClip startSound;
	public AudioClip addScore;
	public AudioClip minusScore;
	private float sndVolume;
	public Color tempAnnouncerColor;

	public Vector4[] heartBarPos;
	
	public int[] playerScore;
	public int[] playerTempScore;
	public bool[] playerSuicided;
	public GameObject scorePlus;
	public GameObject scoreMinus;
	public float scoreAddTime = 1;
	
	public Text mapName;
	public string[] mapNameArray;
	public GameObject[] map;

	public GameObject starEmit;

	public GameObject camFocus;
	public GameObject bulletContainer;
	
	public GameObject selectCtrl;
	public PlayerManager plyrMan;
	public Camera mainCam;

	public bool submit = false;

	public bool ready = true; //map screen
	public bool roundEnd = false;
	public bool selectionDone = false;
	
	public float roundEndTimer = 0;
	public float roundEndTimeDelay = 1;
	
	public bool goBack = false;
	public bool wepSelect = true;
	public bool mapExists = false;
	private bool rematch;

	public bool lastTwoPlayers = false;

	private int tempLives = 300;
	private int classicTurn = 1;


	// Use this for initialization
	void Awake() //INCONTROL
	{
		plyrMan = this.GetComponent<PlayerManager> ();
		/*selectCtrl = GameObject.Find ("SelectController");
		wepSelect = true;
		sndVolume = PlayerPrefs.GetFloat ("SndVol");

		heartBarPos [0] = new Vector4 (0, Screen.height, -0.5f, 2);
		heartBarPos [1] = new Vector4 (Screen.width, Screen.height, 1.5f, 2);
		heartBarPos [2] = new Vector4 (0, 0, -0.5f, -1);
		heartBarPos [3] = new Vector4 (Screen.width, 0, 1.5f, -1);


		//mainCam.backgroundColor = PlayerPrefsX.GetColor ("BGColor");
		shipColorSet = PlayerPrefsX.GetColorArray ("Palette");
		tempLives = PlayerPrefs.GetInt ("LivesNum") * 100;
		tempLives = 200;
		classicTurn = PlayerPrefs.GetInt ("TurnClassic");*/

		CreateGirl ();

	}
	// Update is called once per frame
	void Update () {




		/*if (selectionDone) { //Players have ready selected their weapons
			if (ready) { //on map select screen
				if (submit) { //If submit is pressed start round
					selectTitle.text = "";
					numOfPlayers = 4;
					lastTwoPlayers = false;
					startPanel.SetActive (false); //Deactivates Start panel
					GameObject starX = Instantiate (starEmit, transform.position, transform.rotation) as GameObject;
					starX.transform.parent = this.transform;
					GetComponent<AudioSource> ().PlayOneShot (startSound, sndVolume);
					for (int i = 0; i < 4; i++) { //Loops for however many number of players
						if (plyrMan.playerJoined[i]) {
							CreatePlayerShip (i); //Creates a player ship
						} else {
							numOfPlayers -= 1;
						}
					}
					ready = false;
					submit = false;
				}
			} else { //If not on ready (map select) screen
				if (roundEnd) { //If round is ended
					if (submit) { //If submit is pressed
						scorePanel.SetActive (false); //Deactivate score panel
						startPanel.SetActive (true); //Activate Start panel
						ready = true;
						rematch = false;
						roundEnd = false;
						submit = false;
						GameObject.Find ("ArenaCanvas").SendMessage ("MapSelect");
					} else if (rematch) { //Else If rematch button is pressed
						scorePanel.SetActive (false); //Deactivate score panel
						startPanel.SetActive (true); //Activate Start panel
						ready = true;
						rematch = false;
						roundEnd = false;
					} else if(wepSelect) {
						selectTitle.text = "Prepare for Battle!";
						selectionDone = false;
					}
				}
			}
			camFocus.transform.position = new Vector3 (0, 0, -10);
		} else {
			camFocus.transform.position = new Vector3 (0, -25, -10);
		}

		if (playerDeaths >= numOfPlayers-1) { //If at most 1 player remains
			if (roundEndTimer == 0) {
				roundEndTimer = Time.time + roundEndTimeDelay;
			}
			if (roundEndTimer <= Time.time) {
				RoundEnd(); //End round
				playerDeaths = 0; //Reset playerDeaths counter
				roundEndTimer = 0;
			}
		}

		if (playerDeaths >= numOfPlayers - 2) {
			lastTwoPlayers = true;
		}

		if (announced) {
			announcerText.color = Color.Lerp(tempAnnouncerColor, Color.clear, aT);
			if (aT < 1) {
				aT += Time.deltaTime/3;
			} else {
				announced = false;
			}
		}

		submit = Input.GetButtonDown("Submit");

		if (shake > 0) { //If shake value is over 0
			mainCam.transform.position += new Vector3(Random.insideUnitCircle.x * shakeAmount,Random.insideUnitCircle.y * shakeAmount, 0); //shake camera and keep z at -10
			shake -= Time.deltaTime * decreaseFactor;
		} else {
			shake = 0.0f;
		}*/
	}

	void CreateGirl() {
		 
		for (int x = 0; x < gstats.Length; x++) {
			gstats [x] = Random.Range (-1, 3);
		}

	}


	void CreatePlayerShip (int playerNum) {
		//Instantiate player ship at respective spawn
		playerSpawn [playerNum] = GameObject.Find ("P" + (playerNum+1) + "Spawn");
		GameObject playerX = Instantiate (playerShip, playerSpawn [playerNum].transform.position, playerSpawn [playerNum].transform.rotation) as GameObject;
		playerX.transform.parent = this.transform; //Make player ship child of this game object
		PlayerController playerXScript = playerX.GetComponent<PlayerController>(); //Get script of player ship
		playerXScript.playerNum = playerNum; //Set ship player number
		playerXScript.playerInput = playerInput [playerNum];
		playerXScript.startingHealth = tempLives;
		playerXScript.classicTurning = classicTurn;

		playerX.tag = playerNum.ToString(); //Set tag to player number String
		playerCurrent[playerNum] = playerX; //Add ship gameobject to array

	}
	
	/*void AddScore (int playerNum) {
		//playerScore[playerNum] += 1; //Add a score to player who killed some1
		playerDeaths += 1; //Add 1 to player death counter
		playerTempScore [playerNum] += 1;
		announcerText.text = "";
		aT = 0;
		announced = true;
		shake += shakeAmount;
		for (int i = 0; i < 4; i++) {
			
			if(playerTempScore[i] == 1) {
				if (!firstBloodHappened) {
					announcerText.text = "SINGLE KILL";
					//GetComponent<AudioSource>().PlayOneShot (firstBlood, sndVolume);
					tempAnnouncerColor = shipColor[i];
					firstBloodHappened = true;
				}
			}
			if(playerTempScore[i] == 2) {
				announcerText.text = "DOUBLE KILL";
				//GetComponent<AudioSource>().PlayOneShot (doubleKill, sndVolume);
				tempAnnouncerColor = shipColor[i];
			} else if(playerTempScore[i] == 3) {
				announcerText.text = "TRIPLE KILL";
				//GetComponent<AudioSource>().PlayOneShot (tripleKill, sndVolume);
				tempAnnouncerColor = shipColor[i];
			}
		}
	}

	void Suicide (int playerNum) {
		//playerScore[playerNum] -= 1; //Add a score to player who killed some1
		playerDeaths += 1; //Add 1 to player death counter
		//playerTempScore [playerNum] -= 1;
		playerSuicided [playerNum] = true;
		shake += shakeAmount;
		announcerText.text = "";
		aT = 0;
		announced = true;
		announcerText.text = "SELF KILL";
		//GetComponent<AudioSource>().PlayOneShot (firstBlood, sndVolume);
		tempAnnouncerColor = shipColor[playerNum];
	}

	void RoundEnd (){
		//Set Score when round ends for all players texts
		for (int i = 0; i < 4; i++) {
			if (plyrMan.playerJoined[i]) { //if the player number is in the game
				if (playerScore [i] == 0) {
					scoreText[i].text = "0";
				}
				float suiTimer = 0.5f; //Set base suicide popup timer
				scoreTitleText [i].color = shipColor [i];
				for (int x = 0; x < playerTempScore [i]; x++) { //For every kill
					StartCoroutine (ScoreScreenPlus (i, x, playerTempScore [i], scoreAddTime * x + 0.5f));
					suiTimer += 1; //Add one delay to suicide timer
				}
				if (playerSuicided [i]) { //If player had a suicide
					StartCoroutine (ScoreScreenSuicide (i, scoreAddTime * suiTimer));
				}
			} else {
				scoreTitleText[i].color = new Color(0,0,0,0);
				scoreText[i].text = "";
			}
			playerTempScore[i] = 0;
			playerSuicided [i] = false;
		}
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
		roundEnd = true;
		mapExists = false;
		firstBloodHappened = false;
		scorePanel.SetActive (true);
	}*/

	public void Rematch () { //Rematch button is pressed
		rematch = true;
	}

	public void GoToLevel(int levelNum){
		GameObject loadingPanel = GameObject.Find ("LoadingPanel");
		loadingPanel.GetComponent<Animator> ().SetBool ("FromSelect", true);
	}

	public void PlayStartSound () {
		GetComponent<AudioSource> ().PlayOneShot (mapSound, sndVolume);
	}

	IEnumerator ScoreScreenPlus (int playerNum, int killNum, int tempScore, float delayTime) {
		yield return new WaitForSeconds (delayTime);
		GetComponent<AudioSource> ().PlayOneShot (addScore, sndVolume);
		playerScore [playerNum] += 1;
		GameObject scorePlusOne = Instantiate (scorePlus, scoreText [playerNum].transform.position, scoreText [playerNum].transform.rotation) as GameObject;
		scoreText [playerNum].text = string.Format ("{0}", playerScore [playerNum]);
		Destroy (scorePlusOne, 2);
	}

	IEnumerator ScoreScreenSuicide (int playerNum, float delayTime) {
		yield return new WaitForSeconds (delayTime);
		GetComponent<AudioSource> ().PlayOneShot (minusScore, sndVolume);
		GameObject scoreMinusOne = Instantiate (scoreMinus, scoreText [playerNum].transform.position, scoreText [playerNum].transform.rotation) as GameObject;
		playerScore [playerNum] -= 1;
		scoreText [playerNum].text = string.Format ("{0}", playerScore [playerNum]);
		Destroy (scoreMinusOne, 2);
	}

	public void TimeStopActivate (float timeSpeed, float timeLength) {
		StartCoroutine (TimeStop (timeSpeed, timeLength));
	}

	IEnumerator TimeStop (float speed, float length) {
		Time.timeScale = speed;
		yield return new WaitForSeconds (length);
		Time.timeScale = 1.0f;
	}

}
