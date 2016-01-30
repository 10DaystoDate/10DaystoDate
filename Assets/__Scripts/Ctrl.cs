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
	public AudioClip phoneVibrateSound;
	public AudioClip readySound;
	public AudioClip timerTone;

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
	public Text timerText;

	public GameObject gamedayPanel;
	public GameObject phonePanel;
	public GameObject[] phonePanes;
	public GameObject morn;
	public GameObject after;
	public GameObject night;
	public Text[] phoneText;
	public Sprite[] playerSprites;

	public bool questionSelect = false;
	public bool[] playerInQuestionSelect;

	public int[] playerScore;

	public int dayNumber = 0;
	public int backdrop;
	public int questionPhase = 0;
	public int dayLimit = 5;
	public float timerLength = 5;
	private float timer = 0;
	private float oldTime = 0;

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
		if (timer > Time.time) {
			float roundedTime = Mathf.Round (timer - Time.time);
			timerText.text = roundedTime.ToString ();
			if (oldTime != roundedTime) {
				GetComponent<AudioSource> ().PlayOneShot (timerTone);
				oldTime = roundedTime;
			}
		} else {
			timerText.text = "";
			if (questionSelect) { //If in question select mode
				for (int i = 0; i < playerCurrent.Length; i++) {
					if (playerInQuestionSelect [i]) { //For all players in question select
						playerCurrent [i].GetComponent<PlayerController> ().EndTimerChoose (); //Make them choose current choice
					}
				}
			}
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
		yield return new WaitForSeconds (1);
		dayNumber += 1;
		after.SetActive (false);
		night.SetActive (false);
		morn.SetActive(true);
		dayText.text = string.Format ("Day {0}",dayNumber);
		//Play day screen music
		yield return new WaitForSeconds (2);
		camScript.CameraChangePos(3);
		backdrop = 0;
		questionPhase = 0; //Reset question phase
		phonePanel.SetActive (false); //Disable phone panel
		StartCoroutine (StartQuestionPhase ());
	}

	IEnumerator StartQuestionPhase () { //Start day, reset all day variables
		if (backdrop == 1) {
			morn.SetActive (false);
			after.SetActive (true);
		} 
		else if (backdrop == 2) {
			after.SetActive (false);
			night.SetActive (true);
		}
		yield return new WaitForSeconds (gamedayStartupTime);
		gamedayPanel.SetActive (true);
		CreateQuestions ();
		timer = Time.time + timerLength; //Set timer

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
		while (statNums [tempDefNum] == 99) { //While it is doing a category that was chosen before
			tempDefNum = Random.Range(0,statNums.Count); //Rechoose a random number
		}
		questionScore.Add(gStats[tempDefNum]); //Add a score from gStats to the questionScore list
		statNums[tempDefNum] = 99; //Replace statNum with 99 to know to skip it

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
		backdrop += 1;
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
		GetComponent<AudioSource> ().PlayOneShot (phoneVibrateSound);
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
		yield return new WaitForSeconds (3);
		camScript.CameraChangePos(4);
	}


	void CreatePlayerCharacter (int playerNum) { //Instantiate players
		//Instantiate player ship at respective spawn
		playerSpawn [playerNum] = GameObject.Find ("P" + (playerNum+1) + "Spawn"); //Get spawn location
		GameObject playerX = Instantiate (playerChar, playerSpawn [playerNum].transform.position, playerSpawn [playerNum].transform.rotation) as GameObject;
		playerX.transform.parent = this.transform; //Make player ship child of this game object
		playerCurrent [playerNum] = playerX;
		PlayerController playerXScript = playerX.GetComponent<PlayerController>(); //Get script of player ship
		playerXScript.playerNum = playerNum; //Set ship player number
		playerXScript.playerInput = playerInput [playerNum]; //Set player input
		playerXScript.GetComponent<SpriteRenderer>().sprite = playerSprites[playerNum];

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

	public void PlayReadySound() {
		GetComponent<AudioSource> ().PlayOneShot (readySound);
	}
}
