using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using InControl;

public class Player : MonoBehaviour
{
	public int playerNum;
	public int colorNum;
	public List<string> wepNameList;
	public List<string> infoList;
	public int[] wepNum = new int[] {0,0,0};
	public GameObject[] gunArray;
	public Vector3[] slotArray = new [] {new Vector3(0,0.5f,0), new Vector3(0,0,0), new Vector3(0,-0.5f,0)};

	public int darkAlpha = 18;
	public int brightAlpha = 55;

	public Text playerName;
	//Weapon text fields
	public Text[] wepText;
	public Text playerStatus;
	public Text infoText;
	public GameObject infoPanel;
	public PlayerActions Actions { get; set; }

	public GameObject mainCtrl;
	public Ctrl ctrl;
	public CharSelectCtrl sCtrl;

	public bool choosing = true;
	public bool second = false;
	public bool readyToPlay;

	private GameObject charSelectController;

	public int fieldNum = 0;
	public int[] currentPartNum;

	public GameObject highlight;
	private GameObject highlightArrow;
	public GameObject highlightFollow;
	private GameObject highlightFollowee;
	public GameObject highlightPart;
	private GameObject highlightPartArrow;
	public GameObject highlightPartFollow;
	private GameObject highlightPartFollowee;

	private Color noColor = new Color (255,255,255,255);

	public bool canPress;

	public float deadzone = 0.25f;
	
	void Awake() {
	}
	
	void OnDisable()
	{
		if (playerName) {
			playerName.text = "";
		}
		ctrl.shipColorSet[colorNum] = playerName.color;
		Destroy (highlightArrow);
		Destroy (highlightPartArrow);
		Destroy (highlightFollowee);
		Destroy (highlightPartFollowee);
		wepText[0].text = "";
		wepText[1].text = "";
		wepText[2].text = "";
		playerStatus.text = "Press X to Join";
		if (charSelectController) {
			charSelectController.SendMessage ("PlayerUnReady", playerNum);
		}
		sCtrl.activePanel[playerNum].GetComponent<Image>().color = new Color(255,255,255,darkAlpha)/255;
		sCtrl.shipBox[playerNum].GetComponent<Image>().color = new Color(255,255,255,darkAlpha)/255;
		sCtrl.deactivePanel[playerNum].GetComponent<Image>().color = new Color(255,255,255,brightAlpha)/255;
		if (Actions != null)
		{
			Actions.Destroy();
		}

	}
	
	void Start()
	{
		mainCtrl = GameObject.Find ("MainController");
		ctrl = mainCtrl.GetComponent<Ctrl> ();
		//Get Text boxes and add them to variables
		charSelectController = GameObject.Find ("SelectController");
		sCtrl = charSelectController.GetComponent<CharSelectCtrl>();

		//Set default weapons
		playerName = sCtrl.playerName [playerNum];
		wepText[0] = sCtrl.wepSlot1 [playerNum];
		wepText[1] = sCtrl.wepSlot2 [playerNum];
		wepText[2] = sCtrl.wepSlot3 [playerNum];
		infoText = sCtrl.infoText [playerNum];
		infoPanel = sCtrl.infoPanel [playerNum];
		//Activate player panel, deactivate press X to join panel
		sCtrl.activePanel[playerNum].GetComponent<Image>().color = new Color(255,255,255,brightAlpha)/255;
		sCtrl.deactivePanel[playerNum].GetComponent<Image>().color = new Color(0,0,0,0);

		playerStatus = sCtrl.playerStatus [playerNum];

		//Player is choosing weapons
		choosing = true;
		colorNum = playerNum;
		while (ctrl.shipColorNum.Contains (colorNum)){ //while color num is the same, add 1
			colorNum += 1;
			if (colorNum > ctrl.shipColorSet.Length-1) {
				colorNum = 0;
			}
		}
		ctrl.shipColorNum[playerNum] = colorNum; //Set ship color in ctrl after logic
		ctrl.shipColor[playerNum] = ctrl.shipColorSet[colorNum]; //Set ship color in ctrl after logic

		playerName.text = "Player "+(playerNum+1);
		playerName.color = ctrl.shipColorSet[colorNum];
		ctrl.shipColorSet [colorNum] = noColor;

		highlightFollowee = Instantiate (highlightFollow, playerName.transform.position + Vector3.up*0.2f, playerName.transform.rotation) as GameObject;
		highlightArrow = Instantiate (highlight, playerName.transform.position + Vector3.up*0.2f, playerName.transform.rotation) as GameObject;
		//highlightArrow.GetComponent<Obj2DFollow> ().target = highlightFollowee.transform;

		highlightPartFollowee = Instantiate (highlightPartFollow, this.transform.position + slotArray[fieldNum], this.transform.rotation) as GameObject;
		highlightPartArrow = Instantiate (highlightPart, this.transform.position + slotArray[fieldNum], this.transform.rotation) as GameObject;
		//highlightPartArrow.GetComponent<Obj2DFollow> ().target = highlightPartFollowee.transform;
		highlightPartArrow.SetActive (false);

		CreateAllParts ();
	}
	void ChangeFieldNum (int amount) {
		int newFieldNum = fieldNum + amount;
		if (newFieldNum >= -1 && newFieldNum < 3) {
			fieldNum = newFieldNum;
			if (fieldNum == -1) {
				highlightFollowee.transform.position = playerName.transform.position + Vector3.up*0.2f;
				highlightPartArrow.SetActive (false);
			} else {
				highlightPartArrow.SetActive (true);
				highlightFollowee.transform.position = wepText [fieldNum].transform.position;
				highlightPartFollowee.transform.position = this.transform.position + slotArray [fieldNum];
			}
		}
	}

	void ChooseColor (int amount) {

		if (playerName.color != noColor) {
			ctrl.shipColorSet[colorNum] = playerName.color; //Replace blank with the part back into name list
		}
		colorNum += amount;
		colorNum = CheckColorWrap (colorNum);
		while (ctrl.shipColorSet[colorNum] == noColor) { //As long as partnum is equal to a taken field, keep adding
			colorNum += amount;
			colorNum = CheckColorWrap (colorNum); //Check wrap for array
		}
		ctrl.shipColorNum[playerNum] = colorNum; //Set shipColorNum as colorNum
		ctrl.shipColor[playerNum] = ctrl.shipColorSet[colorNum]; //Set shipColor from colorSet
		playerName.color = ctrl.shipColorSet[colorNum];
		CreateAllParts ();
		ctrl.shipColorSet [colorNum] = noColor; //Replace color in color set with blank
		return;
	}

	void ChoosePart (int amount) {
		if (fieldNum == -1) {
			ChooseColor (amount);
			return;
		}
		int partNum = currentPartNum [fieldNum];
		if (wepText [fieldNum].text != "") {
			wepNameList[partNum] = wepText [fieldNum].text; //Replace blank with the part back into name list
		}
		partNum += amount; //Change partnumber
		partNum = CheckWepWrap (partNum);
		while (wepNameList [partNum] == "") { //As long as partnum is equal to a taken field, keep adding
			partNum += amount; //Change partnumber
			partNum = CheckWepWrap (partNum);
		}
		currentPartNum [fieldNum] = partNum; //setPartNum for field
		wepText[fieldNum].text = wepNameList [partNum]; //Set text for wep field

		wepNameList [partNum] = "";//Replace wepName in list with blank
		wepNum[fieldNum] = partNum; //Adds number to wepNum array
		CreatePart();
	}

	void CreatePart () {
		if (gunArray [fieldNum] != null) {
			Destroy (gunArray [fieldNum]);
		}
		GameObject wep = Instantiate (ctrl.weaponSet[currentPartNum[fieldNum]], this.transform.position + slotArray[fieldNum], this.transform.rotation) as GameObject;
		wep.transform.parent = this.transform;
		wep.GetComponent<SpriteRenderer> ().color = ctrl.shipColor[playerNum];
		wep.transform.localScale += new Vector3 (1.2f, 1.2f, 0);
		gunArray [fieldNum] = wep; //Set gunArray as wep
	}

	int CheckColorWrap (int thisNum) {
		if (thisNum < 0) { //if less than limit, wrap
			thisNum = ctrl.shipColorSet.Length - 1;
		} else if (thisNum > ctrl.shipColorSet.Length - 1) { //if more than limit, wrap
			thisNum = 0;
		}
		return thisNum;
	}

	int CheckWepWrap (int thisNum) {
		if (thisNum < 0) { //if less than limit, wrap
			thisNum = wepNameList.Count - 1;
		} else if (thisNum > wepNameList.Count - 1) { //if more than limit, wrap
			thisNum = 0;
		}
		return thisNum;
	}

	void CreateAllParts () {
		for (int i = 0; i < 3; i++) {
			fieldNum = i;
			ChoosePart (0);
		}
		fieldNum = -1; //Reset field num to color field
	}
	
	void Update()
	{
		if (choosing) {

			if (Actions.Left.WasPressed || Actions.Right.WasPressed || Actions.Up.WasPressed || Actions.Down.WasPressed) {
				canPress = true;
			}
			if (Actions.Left.WasReleased || Actions.Right.WasReleased || Actions.Up.WasReleased || Actions.Down.WasReleased) {
				canPress = false;
			}
			if (canPress) {
				if (Mathf.Abs (Actions.Left) > deadzone) {
					ChoosePart (-1);
					canPress = false;
				}
				if (Mathf.Abs (Actions.Right) > deadzone) {
					ChoosePart (1);
					canPress = false;
				}
				if (Mathf.Abs (Actions.Up) > deadzone) {
					ChangeFieldNum (-1);
					canPress = false;
				}
				if (Mathf.Abs (Actions.Down) > deadzone) {
					ChangeFieldNum (1);
					canPress = false;
				}
			}
			//}

			/*
			if (Actions.Left.WasPressed) {
				ChoosePart (-1);
			}
			if (Actions.Right.WasPressed) {
				ChoosePart (1);
			}
			if (Actions.Up.WasPressed) {
				ChangeFieldNum (-1);
			}
			if (Actions.Down.WasPressed) {
				ChangeFieldNum (+1);
			}*/

			/*
			if (Actions.Red) {
			}
			if (Actions.Yellow.WasPressed) {
			}
			if (Actions.Blue.WasPressed) {
			}*/
			if (Actions.Green.WasPressed) {
				if(second) {
					choosing = false;
					sCtrl.shipBox[playerNum].GetComponent<Image>().color = new Color(255,255,255,brightAlpha)/255;
					playerStatus.text = "Ready!";
					charSelectController.SendMessage ("PlayerReady", playerNum);
					ctrl.wepNum1[playerNum] = wepNum[0];
					ctrl.wepNum2[playerNum] = wepNum[1];
					ctrl.wepNum3[playerNum] = wepNum[2];
					highlightPartArrow.SetActive (false);
					highlightArrow.SetActive (false);
				}
			}
			if (choosing) {
				playerStatus.text = "Choosing...";
				second = true;
				ctrl.goBack = false;
			}
		}
		if (Actions.Start.WasPressed) {
			if (charSelectController.GetComponent<PlayerManager>().readyToPlay == true) {
				ctrl.selectionDone = true;
				if (ctrl.wepSelect) {
					ctrl.PlayStartSound ();
				}
				if (ctrl.mapExists) {
					if (Time.timeScale != 0) {
						Time.timeScale = 0;
						ctrl.pausePanel.SetActive (true);
					} else {
						Time.timeScale = 1;
						ctrl.pausePanel.SetActive (false);
					}
				}
				ctrl.wepSelect = false;
			} else {
				ctrl.selectionDone = false;
			}
		}
		if (Actions.LTrigger.IsPressed) {
			//Bring up box
			if (choosing == true && fieldNum >= 0) {
				infoText.text = infoList [wepNum [fieldNum]];
				infoPanel.GetComponent<Image> ().color = new Color (55, 55, 55, 255) / 255;
			}
		} else {
			//Close box
			infoText.text = "";
			infoPanel.GetComponent<Image>().color = new Color(0,0,0,0);
		}

	}
	
	public void Unready () {
		choosing = true;
		sCtrl.shipBox[playerNum].GetComponent<Image>().color = new Color(255,255,255,darkAlpha)/255;
		playerStatus.text = "Choosing...";
		if (fieldNum != -1) {
			highlightPartArrow.SetActive (true);
		}
		highlightArrow.SetActive (true);
	}
}

