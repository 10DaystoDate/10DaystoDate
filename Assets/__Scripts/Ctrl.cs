using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using InControl;

public class Ctrl : MonoBehaviour {
	[System.Serializable]
	public class QuestionList
	{
		public string[] question;
	}
	public QuestionList[] questionlist;

	[System.Serializable]
	public class QuestionTextList
	{
		public Text[] questionText;
	}
	public QuestionTextList[] qTextList;

	public List<int> questionScore;
	public List<int> questionCat;

	public Text[] playerSelectText;

	public List<int> statNums;
	public List<int> defaultStats;
	public List<int> tempDefaultStats;
	public List<int> gStats;

	public AudioClip dayStartSound;

	public Camera mainCam;
	public CameraScript camScript;
	public PlayerManager plyrMan;
	public bool submit = false;

	public bool ready = true; //map screen
	public bool roundEnd = false;
	public bool selectionDone = false;

	public bool goBack = false;
	public bool wepSelect = true;
	private bool questionStart = false;

	public GameObject[] playerCurrent;
	public GameObject[] playerInput;

	public bool dayStart = false;
	public float gamedayStartupTime = 2;
	public int numOfPlayers;
	public GameObject[] playerSpawn;
	public GameObject playerChar;

	public Text dayText;

	public GameObject gamedayPanel;
	public GameObject phonePanel;
	public GameObject[] phonePanes;
	public Text[] phoneText;

	public bool questionSelect = false;
	public bool[] playerInQuestionSelect;

	public int[] playerScore;

	public int dayNumber = 0;
	public int questionPhase = 0;
	public int dayLimit = 5;

	// Use this for initialization
	void Awake() //INCONTROL
	{
		plyrMan = this.GetComponent<PlayerManager> ();
		camScript = mainCam.GetComponent<CameraScript> ();

	}
	// Update is called once per frame
	void Update () {
		if (selectionDone) { //Players have pressed start on character select screen
			//Reset stats
			if (ready) {
				SetupGame (numOfPlayers);
				ready = false;
			}
		}
		if (dayStart) { //At school screen
			//Go to day screen (Day 1)
			StartCoroutine (StartDay ());
			dayStart = false;
		}

	}
	void SetupGame (int numOfPlayers) { //Set up game scene (Create girl, players) //Reset game variables
		ResetStats(); //Reset player stats
		CreateGirl ();
		numOfPlayers = 4; //set num of players to 4
		for (int i = 0; i < 4; i++) { //Loops for however many number of players
			if (plyrMan.playerJoined[i]) {
				CreatePlayerCharacter(i); //Creates a player char
			} else {
				numOfPlayers -= 1; //Subtract from num of players to find total num of players
			}
		}
		dayStart = true;
		ready = false;
		submit = false;
	}

	void ResetStats () {
		for (int i = 0; i < playerScore.Length; i++) {
			playerScore [i] = 0;
		}
	}

	void CreateGirl() { //Set girl stats and instantiate her?
		if (gStats.Count > 1) {
			gStats.Clear();
		}
		for (int i = 0; i < defaultStats.Count; i++) { //Fill up temp stats with 2, 1, 0, -1s (To have even amount of numbers
			tempDefaultStats.Add(defaultStats[i]);
		}

		for (int i = 0; i < defaultStats.Count; i++) { //Fill up gStats with random stats based on defaultStats
			int tempDefNum = Random.Range(0,tempDefaultStats.Count); //Save a random number within tempDefaultStats count
			gStats.Add(tempDefaultStats[tempDefNum]); //Add a random element from tempDefaultStats based on length of tempDefaultStats to gStats
			tempDefaultStats.RemoveAt(tempDefNum); //Remove element from tempDefault stats
		}
	}

	IEnumerator StartDay () { //Start day, reset all day variables, add 1 to day number
		camScript.CameraChangePos(2);
		dayNumber += 1;
		dayText.text = dayNumber.ToString ();
		//Play day screen music
		yield return new WaitForSeconds (3);
		camScript.CameraChangePos(3);
		questionPhase = 0; //Reset question phase
		phonePanel.SetActive (false); //Disable phone panel
		StartCoroutine (StartQuestionPhase ());
	}

	IEnumerator StartQuestionPhase () { //Start day, reset all day variables
		yield return new WaitForSeconds (gamedayStartupTime);
		gamedayPanel.SetActive (true);
		CreateQuestions ();

	}

	void CreateQuestions() {
		questionSelect = true;
		if (statNums.Count > 1) { //If statNums is not empty, empty it
			statNums.Clear();
			questionScore.Clear ();
		}
		for (int i = 0; i < defaultStats.Count; i++) { //Fill statnums with whatever (we only use it for its places)
			statNums.Add(defaultStats[i]); //Fill up until as many elemnets as stats
		}
		for (int i = 0; i < 4; i++) { //Generate 4 questions and set players answering questions to true
			GenerateQuestion (i);
			if (plyrMan.playerJoined [i]) {
				playerInQuestionSelect [i] = true;
			}
		}
	}
	void GenerateQuestion(int questionNum) {
		//Randomly pick g stat category from list
		int tempDefNum = Random.Range(0,statNums.Count); //Save a random number within statNums count
		questionScore.Add(gStats[tempDefNum]); //Add a score from gStats to the questionScore list
		statNums.RemoveAt(tempDefNum); //Remove element from statNums

		//Add question text for all players
		int tempQuestNum = Random.Range(0,4);
		for (int i = 0; i < 4; i++) { //for each question box
			if (plyrMan.playerJoined[i]) { //If player is in the game
				qTextList [i].questionText[questionNum].text = questionlist [tempDefNum].question[tempQuestNum]; //Choose random question from the category for the player
			}
		}
	}

	IEnumerator ClearQuestions () {
		yield return new WaitForSeconds (gamedayStartupTime);
		questionPhase += 1;
		gamedayPanel.SetActive (false);
		if (questionPhase <= 2) {
			StartCoroutine (StartQuestionPhase ());
		} else {
			Debug.Log ("DAY FINISHED SHSOW SCORES");
			StartCoroutine (EndDay ());
		}
	}

	IEnumerator EndDay () { //Start day, reset all day variables
		yield return new WaitForSeconds (1);
		phonePanel.SetActive(true);
		for (int i = 0; i < playerScore.Length; i++) {
			if (plyrMan.playerJoined [i]) { //If player is in the game
				phonePanes [i].SetActive (true);
			} else {
				phonePanes [i].SetActive (false);
			}
		}
		for (int i = 0; i < phoneText.Length; i++) {
			phoneText[i].text = string.Format ("I heart you times {0}!", playerScore[i]);
		}
		yield return new WaitForSeconds (4);
		if (dayNumber < dayLimit) { //If there are still days to go
			StartCoroutine (StartDay ());
		} else { //If all days are over
			//Go to game over screen
			StartCoroutine (EndGame ());
		}
	}

	IEnumerator EndGame () { //Start day, reset all day variables, add 1 to day number
		//Play day screen music
		yield return new WaitForSeconds (3);
		int highestScore = 0;
		int winningPlayer = 0;
		for (int i = 0; i < playerScore.Length; i++) {
			if (plyrMan.playerJoined [i]) { //If player is in the game
				if (playerScore [i] >= highestScore) {
					highestScore = playerScore [i];
					winningPlayer = i;
				}
			}
		}

		phonePanel.SetActive(true);
		for (int i = 0; i < phoneText.Length; i++) {
			phoneText [i].text = "You: Will you be my girlfriend forever?";
			//phoneText[i].text = string.Format ("I heart you times {0}!", playerScore[i]);
		}
		yield return new WaitForSeconds (3);
		for (int i = 0; i < phoneText.Length; i++) {
			phoneText [i].text = "Girl is typing...";
			//phoneText[i].text = string.Format ("I heart you times {0}!", playerScore[i]);
		}
		yield return new WaitForSeconds (3);
		for (int i = 0; i < phoneText.Length; i++) {
			if (i == winningPlayer) {
				phoneText [i].text = "Her: YES! <3";
			} else {
				phoneText [i].text = "";
			}
			//phoneText[i].text = string.Format ("I heart you times {0}!", playerScore[i]);
		}

	}


	void CreatePlayerCharacter (int playerNum) { //Instantiate players
		//Instantiate player ship at respective spawn
		playerSpawn [playerNum] = GameObject.Find ("P" + (playerNum+1) + "Spawn"); //Get spawn location
		GameObject playerX = Instantiate (playerChar, playerSpawn [playerNum].transform.position, playerSpawn [playerNum].transform.rotation) as GameObject;
		playerX.transform.parent = this.transform; //Make player ship child of this game object
		PlayerController playerXScript = playerX.GetComponent<PlayerController>(); //Get script of player ship
		playerXScript.playerNum = playerNum; //Set ship player number
		playerXScript.playerInput = playerInput [playerNum]; //Set player input
		for (int i = 0; i <4; i++) {
			playerXScript.question[i] = qTextList [playerNum].questionText[i];
		}
	}

	public void ChooseQuestion (int playerNum, int questionNum) {
		playerScore[playerNum] += questionScore [questionNum]; //Add question score to player's score

		playerInQuestionSelect [playerNum] = false; //Set player to finished selecting question

		for (int i = 0; i < 4; i++) { //for each question
			if ( i != questionNum) { //if question is not the one player chose
				qTextList [playerNum].questionText[i].text = ""; //Remove the question text
			}
		}

		int boolTest = 0;
		for (int i = 0; i < playerInQuestionSelect.Length; i++) {
			if (playerInQuestionSelect [i]) {
				boolTest += 1;
			}
		}
		if (boolTest == 0) {
			StartCoroutine(ClearQuestions ());
		}
	}

	public void GoToLevel(int levelNum){
		GameObject loadingPanel = GameObject.Find ("LoadingPanel");
		loadingPanel.GetComponent<Animator> ().SetBool ("FromSelect", true);
	}

	/*void CreatePlayerShip (int playerNum) {
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

	}*/
	
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

	/*public void Rematch () { //Rematch button is pressed
		rematch = true;
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
	}*/





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
