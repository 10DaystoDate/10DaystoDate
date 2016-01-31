using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	//Player number, only thing different for each player
	public int playerNum = 0;
	public GameObject mainController;
	public Text[] question;
	public GameObject playerInput;
	public Player pIn;

	public GameObject chooser;
	public GameObject curChooser;
	//public GameObject curQuestion;

	public int curQuestionNum = 0;

	public Ctrl ctrl;

	void Awake () {
		//Get ship variables from main controller
		mainController = GameObject.Find ("_MainController");
		ctrl = mainController.GetComponent<Ctrl>();
	}

	void Start () {
		pIn = playerInput.GetComponent<Player>();
		curChooser = Instantiate (chooser, question [playerNum].transform.position, question [playerNum].transform.rotation) as GameObject;
		curChooser.transform.parent = ctrl.transform;
		curChooser.SetActive (false);
		curQuestionNum = playerNum;


	}
	
	// Update is called once per frame
	void Update () {
		//Bool controls
		bool up = pIn.Actions.Up.WasPressed;
		bool down = pIn.Actions.Down.WasPressed;
		bool green = pIn.Actions.Green.WasPressed;


		if (ctrl.questionSelect) { //If in question selection phase
            // force cursor to move down one if question is already picked
			if (question[curQuestionNum].color == new Color(255,255,255,0.3f)) {
                ChangeQuestion(1);
            }
            //Movement
            if (ctrl.playerInQuestionSelect [playerNum]) { //If player is still choosing a question

				curChooser.SetActive (true);

				if (up) {
					//move selector up
					ChangeQuestion (-1);
				} else if (down) {
					//yes
					ChangeQuestion (1);
				}
				if (green) {
					//Accept question, deactivate question chooser
					ctrl.ChooseQuestion (playerNum, curQuestionNum); //Run ChooseQuestion on Ctrl script
					curChooser.SetActive (false); //Disable the cursor chooser for now
				}
			}
		}
	}

	void ChangeQuestion (int amount) {
		curQuestionNum += amount;
		if (curQuestionNum < 0) { //If it goes below array
			curQuestionNum = question.Length - 1;
		} else if (curQuestionNum > question.Length - 1) { //If goes above array
			curQuestionNum = 0; //Wrap around array
		}
		curChooser.transform.position = question[curQuestionNum].transform.position;
        //logic to skip questions that are already chosen using recursion
		if (question[curQuestionNum].color == new Color(255,255,255,0.3f)) {
            ChangeQuestion(amount);
        }
    }

	public void EndTimerChoose () {if (ctrl.questionSelect) { //If in question selection phase
			//Movement
			if (ctrl.playerInQuestionSelect [playerNum]) {
				//Accept question, deactivate question chooser
				ctrl.ChooseQuestion (playerNum, curQuestionNum); //Run ChooseQuestion on Ctrl script
				curChooser.SetActive (false); //Disable the cursor chooser for now
			}
		}
	}
}
