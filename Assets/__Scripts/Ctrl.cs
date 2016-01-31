using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Xml;

public class Ctrl : MonoBehaviour {
	[System.Serializable]
	public class QuestionList
	{
		public List<string> question;
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

	public AudioClip startSound;
	public AudioClip dayStartSound;
	public AudioClip phoneVibrateSound;
	public AudioClip readySound;
	public AudioClip timerTone;
	public AudioClip selectTone;

	public Camera mainCam;
	public CameraScript camScript;
	public PlayerManager plyrMan;
	public bool submit = false;

	public bool ready = true; //map screen
	public bool roundEnd = false;
	public bool selectionDone = false;

	public bool goBack = false;
	public bool wepSelect = true;
	//private bool questionStart = false;

	public GameObject[] playerCurrent;
	public GameObject[] playerInput;

	public float gamedayStartupTime = 2;
	public int numOfPlayers;
	public GameObject[] playerSpawn;
	public GameObject playerChar;

	public Text dayText;
	public Text timerText;

	public GameObject gamedayPanel;
	public GameObject[] questionPanes;
	public GameObject phonePanel;
	public GameObject[] phonePanes;
	public GameObject[] backdropImg;
	public Text[] phoneText;
	public Sprite[] playerSprites;
	public GameObject winnerPlace;

	public GameObject[] cutsceneObj;
	public GameObject cutsceneGirl;

	public bool questionSelect = false;
	public bool[] playerInQuestionSelect;
    public bool firstPerson = false;
	public bool mainMenu = true;
	public bool winScreen = false;

	public int[] playerScore;
    public int[] playerDayScore;

	public int dayNumber = 0;
	public int backdrop;
	public int questionPhase = 0;
	public int dayLimit = 5;
    public int firstPersonNum;
    public int firstPersonPoints;
    public float timerLength = 5;
	private float timer = 0;
	private float oldTime = 0;
	private bool backChanging = false;
	private float aT = 0; //lerp float for announcer text color

	public TextAsset GameAsset;
	private void XMLtoQuestions()
	{
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.LoadXml(GameAsset.text); // load the file.
		XmlNodeList levelsList = xmlDoc.GetElementsByTagName("Question"); // array of the level nodes.
		foreach (XmlNode levelInfo in levelsList)
		{
			XmlNodeList levelcontent = levelInfo.ChildNodes;
			int questionsize = 0;
			int questionID = 0;
			foreach (XmlNode levelsItens in levelcontent)
			{
				if (levelsItens.Name == "id") {
					questionID = int.Parse(levelsItens.InnerText);
				}
				if (levelsItens.Name == "size") {
					questionsize = int.Parse(levelsItens.InnerText);
				}
				for (int i = 0; i < questionsize; i++) {
					if (levelsItens.Name == "object" + i)
					{
						questionlist[questionID].question.Add(levelsItens.InnerText);
					}
				}
			}
		}
	}

	// Use this for initialization
	void Awake() 
	{
		plyrMan = this.GetComponent<PlayerManager> ();
		camScript = mainCam.GetComponent<CameraScript> ();
		XMLtoQuestions();
	}

	// Update is called once per frame
	void Update () {
		var inputDevice = InputManager.ActiveDevice;
		bool green = inputDevice.Action1.WasPressed;
		bool greenK = Input.GetButtonDown ("Fire1");
		bool red = inputDevice.Action2.WasPressed;
		bool redK = Input.GetButtonDown ("Fire2");

		if (mainMenu) {
			if (mainCam.transform.position != new Vector3 (0, 0, mainCam.transform.position.z)) {
				camScript.CameraChangePos (0);
			}
			if (green || greenK) {
				mainMenu = false;
				camScript.CameraChangePos (1);
			}
		} else {
			if (selectionDone) { //Players have pressed start on character select screen
				//Reset stats
				if (ready) {
					StartCoroutine (SetupGame (numOfPlayers));
					ready = false;
				}
			} else { 
				if (red || redK) {
					int joinTest = 0;
					for (int i = 0; i < 4; i++) { //Generate 4 questions and set players answering questions to true
						if (plyrMan.playerJoined [i]) {
							joinTest += 1;
						}
					}
					if (joinTest == 0) {
						mainMenu = true;
					}
				}
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
			if (winScreen) {
				if (green || greenK) {
					camScript.CameraChangePos(1);
					ready = true;
					wepSelect = true;
					selectionDone = false;
					winScreen = false;
					mainCam.GetComponent<MusicSelector> ().RandomSong ();
				}
			}
		}
		if (backChanging) { //if backdrop is changing
			backdropImg [backdrop].GetComponent<Image> ().color = Color.Lerp (Color.clear, Color.white, aT); //lerp backdrop img
			if (aT < 1) {
				aT += Time.deltaTime / 1;
			} else {
				backChanging = false;
			}
		}
	}
	IEnumerator SetupGame (int numOfPlayers) { //Set up game scene (Create girl, players) //Reset game variables
		GetComponent<AudioSource> ().PlayOneShot (startSound);
		yield return new WaitForSeconds (0.3f);
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
		StartCoroutine (CutsceneIntro());
		ready = false;
		submit = false;
	}

	void ResetStats () {
		for (int i = 0; i < playerScore.Length; i++) {
			playerScore [i] = 0;
		}
        for (int i = 0; i < playerDayScore.Length; i++) {
            playerDayScore[i] = 0;
        }
		dayNumber = 0;
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
	IEnumerator CutsceneIntro () { //Start day, reset all day variables, add 1 to day number
		camScript.CameraChangePos(2);
		//StartAnimations
		for (int i = 0; i < cutsceneObj.Length; i++) {
			if (plyrMan.playerJoined [i]) { //If player is in the game
				cutsceneObj [i].GetComponent<Animator> ().SetTrigger ("IntroCutStart");
			} else {
				cutsceneObj [i].GetComponent<SpriteRenderer> ().sprite = null;
			}
		}
		cutsceneGirl.GetComponent<Animator> ().SetTrigger ("IntroCutStart");
		yield return new WaitForSeconds (3.5f);
		StartCoroutine (StartDay ());
	}

	IEnumerator StartDay () { //Start day, reset all day variables, add 1 to day number
		ResetAllPhonePos ();
		camScript.CameraChangePos(3);
		yield return new WaitForSeconds (1);
		GetComponent<AudioSource> ().PlayOneShot (dayStartSound);
		dayNumber += 1;
		dayText.text = string.Format ("Day {0}",dayNumber);
		//Play day screen music
		yield return new WaitForSeconds (1.5f);
		camScript.CameraChangePos(4);
		ChangeBackdrop (0);
		questionPhase = 0; //Reset question phase
		StartCoroutine (StartQuestionPhase ());
	}

	IEnumerator StartQuestionPhase () { //Start day, reset all day variables
		
		yield return new WaitForSeconds (1.2f);
		gamedayPanel.SetActive (true);
		for (int i = 0; i < playerScore.Length; i++) {
			if (plyrMan.playerJoined [i]) { //If player is in the game
				questionPanes [i].SetActive (true);
			} else {
				questionPanes [i].SetActive (false);
			}
		}
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
				qTextList [i].questionText[questionNum].color = Color.white;
			}
		}
	}
	void ChangeBackdrop (int backdropNum) {
		aT = 0;
		if (backdropNum < backdropImg.Length) {
			backdrop = backdropNum;
			backChanging = true;
			for (int i = 0; i < backdropImg.Length; i++) { //set other backdrops to clear
				if (i != backdrop) {
					backdropImg [i].GetComponent<Image> ().color = Color.clear;
				}
			}
		}
	}

	IEnumerator ClearQuestions () {
		questionSelect = false;
		timer = Time.time;
        yield return new WaitForSeconds (2);
		questionPhase += 1;
		ChangeBackdrop (questionPhase);
		gamedayPanel.SetActive (false);

		// display first person's win/loss
		yield return new WaitForSeconds (0.2f);
		if (firstPerson) {
			phonePanel.SetActive (true);
			//phonePanes [firstPersonNum].SetActive (true);
			for (int i = 0; i < playerScore.Length; i++) {
				if (plyrMan.playerJoined [i] && i == firstPersonNum) { //If player is in the game
					//phonePanes [i].SetActive (true);
					MovePhone (i, 1);
				} else {
					//phonePanes [i].SetActive (false);
				}
			}
			yield return new WaitForSeconds (0.5f);
			switch (firstPersonPoints) {
			case 2:
				phoneText [firstPersonNum].text = "Her: That was amazing! <3";
				break;
			case 1:
				phoneText [firstPersonNum].text = "Her: That was fun! :)";
				break;
			case 0:
				phoneText [firstPersonNum].text = "Her: That was kind of boring...";
				break;
			case -1:
				phoneText [firstPersonNum].text = "Her: I'm never doing that again! :(";
				break;
			}
			GetComponent<AudioSource> ().PlayOneShot (phoneVibrateSound);
			yield return new WaitForSeconds(3);
			ResetAllPhonePos ();
	        //phonePanes[firstPersonNum].SetActive(false);
			//phonePanel.SetActive(false);
		}
		firstPerson = false;

        if (questionPhase <= 2) {
			StartCoroutine (StartQuestionPhase ());
		} else {
			StartCoroutine (EndDay ());
		}
	}

	IEnumerator EndDay () { //Start day, reset all day variables
		yield return new WaitForSeconds (1);
		phonePanel.SetActive(true);
		for (int i = 0; i < playerScore.Length; i++) {
			if (plyrMan.playerJoined [i]) { //If player is in the game
				MovePhone(i,1);
				//phonePanes [i].SetActive (true);
			} else {
				//phonePanes [i].SetActive (false);
			}
		}
		yield return new WaitForSeconds (0.5f);
		GetComponent<AudioSource> ().PlayOneShot (phoneVibrateSound);
		for (int i = 0; i < phoneText.Length; i++) {
			phoneText [i].text = GenerateHeartText (playerDayScore [i]);
		}
        for (int i = 0; i < playerDayScore.Length; i++) {
            playerDayScore[i] = 0;
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
			phoneText [i].text = "You: Will you be my girlfriend?";
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
				if (playerScore [i] < 0) {
					phoneText [i].text = "Her: I'm calling the cops.";
				} else {
					phoneText [i].text = "Her: I'm not interested... sorry.";
				}
			}
			//phoneText[i].text = string.Format ("I heart you times {0}!", playerScore[i]);
		}
		yield return new WaitForSeconds (3);
		winnerPlace.GetComponent<Image>().sprite = playerSprites [winningPlayer];
		camScript.CameraChangePos(5);
		foreach (Transform child in transform) {//Destroy playercontrollers
			GameObject.Destroy(child.gameObject);
		}
		yield return new WaitForSeconds (3);
		winScreen = true;
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
		GetComponent<AudioSource> ().PlayOneShot (selectTone);
        // save the first person who chose an answer that round
		if (questionNum == -1) {
			playerInQuestionSelect [playerNum] = false; //Set player to finished selecting question
			for (int i = 0; i < 4; i++) { //for each question
				qTextList [playerNum].questionText [i].color = new Color (255, 255, 255, 0.3f); //Remove the question text
			}	
		} else {
			if (firstPerson == false) {
				firstPersonNum = playerNum;
				firstPersonPoints = questionScore [questionNum];
				firstPerson = true;
			}
			playerScore [playerNum] += questionScore [questionNum]; //Add question score to player's score
			playerDayScore [playerNum] += questionScore [questionNum]; //Add question score to player's score
			playerInQuestionSelect [playerNum] = false; //Set player to finished selecting question
			for (int i = 0; i < 4; i++) { //for each question
				if (i != questionNum) { //if question is not the one player chose
					qTextList [playerNum].questionText [i].color = new Color (255, 255, 255, 0.3f); //Remove the question text
				}
			}		
			//erase that choice from other player's choices
			for (int x = 0; x < 4; x++) {
				if (x != playerNum) {
					for (int i = 0; i < 4; i++) { //for each question
						if (i == questionNum) { //if question is the one the first player chose
							qTextList [x].questionText [i].color = new Color (255, 255, 255, 0.3f); //Remove the question text
						}
					}		
				}
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

	void MovePhone (int playerNum, int posNum) {
		//Change phone pos, 0 = offscreen, 1 = onscreen
		phoneText [playerNum].text = "";
		phonePanes [playerNum].GetComponent<FollowScript> ().ChangePos (posNum);
	}

	void ResetAllPhonePos () {
		for (int i = 0; i < playerScore.Length; i++) {
			MovePhone (i, 0);
		}
	}

	string GenerateHeartText (int numOfHearts) {
		string heartString = "";
		if (numOfHearts > 0) {
			for (int i = 0; i < numOfHearts; i++) {
				heartString += "<3";
			}
		} else if (numOfHearts < 0) {
            numOfHearts *= -1;
			for (int i = 0; i < numOfHearts; i++) {
				heartString += "</3";
			}
		}
		return heartString;
	}

	public void GoToLevel(int levelNum){
		GameObject loadingPanel = GameObject.Find ("LoadingPanel");
		loadingPanel.GetComponent<Animator> ().SetBool ("FromSelect", true);
	}

	public void PlayReadySound() {
		GetComponent<AudioSource> ().PlayOneShot (readySound);
	}
}
