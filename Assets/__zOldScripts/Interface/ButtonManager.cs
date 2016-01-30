using UnityEngine;
using InControl;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
	public Button focusedButton;
	public Button lastMapButton;
	public Text fullScreenText;
	public Text turnText;
	public Text livesText;
	public Text paletteText;
	public GameObject camFollow;
	public Camera mainCam;
	public bool mainMenu;
	public bool arenaMenu;
	public bool notFocused = false;
	public Color[] defaultScheme;
	public Color[] solarisDarkScheme;
	public Color[] base16Scheme;
	public Color[] chalkScheme;
	public Color[] mochaScheme;
	public Color[] backColor;
	private Color[] currentColorScheme;
	private Color currentBackColor;
	public GameObject colorPanel;
	public GameObject panelPosition;

	public AudioClip moveSound; // Add a AudioClip reference

	private bool kUp;
	private bool kRight;
	private bool kDown;
	private bool kLeft;

	public Slider sndSlider;
	public Slider musSlider;

	private float tempSnd;
	private float tempMus;
	private bool fullScreenTemp = false;
	
	public GameObject mainController;
	public Ctrl ctrl;

	TwoAxisInputControl filteredDirection;

	public float volume;

	public GameObject loadingPanel;

	private int livesTemp = 3;
	private int turnClassicTemp = 1;

	void Awake()
	{
		filteredDirection = new TwoAxisInputControl();
		filteredDirection.StateThreshold = 0.5f;
		volume = PlayerPrefs.GetFloat ("SndVol");

		if (mainMenu) {
			if (Screen.fullScreen) {
				//Screen.fullScreen = !Screen.fullScreen;
				fullScreenText.text = "Full Screen";
			} else {
				fullScreenText.text = "Windowed";
			}
			GameObject mTemp = GameObject.Find("MusSlider"); //find music slider
			GameObject sTemp = GameObject.Find("SndSlider"); //find music slider
			
			if (mTemp != null) { //if slider was found
				musSlider = mTemp.GetComponent<Slider>();  //set slider to musSlider
			}
			if (sTemp != null) { //if slider was found
				sndSlider = sTemp.GetComponent<Slider>();  //set slider to musSlider
			}

			currentColorScheme = PlayerPrefsX.GetColorArray ("Palette");
			if (!PlayerPrefs.HasKey ("LivesNum")) {
				PlayerPrefs.SetInt ("LivesNum", 3);
			}

			if (!PlayerPrefs.HasKey ("TurnClassic")) {
				PlayerPrefs.SetInt ("TurnClassic", 1);

			}
		}
		if (arenaMenu) {
			mainController = GameObject.Find ("MainController");
			ctrl = mainController.GetComponent<Ctrl>();
		}
		mainCam.backgroundColor = PlayerPrefsX.GetColor ("BGColor");
	}
	
	
	void Update()
	{
		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;
		filteredDirection.Filter( inputDevice.Direction, Time.deltaTime );
		bool green = inputDevice.Action1.WasPressed;
		bool greenK = Input.GetButtonDown ("p0fire1");
		bool enter = Input.GetButtonDown ("Submit");
		bool red = inputDevice.Action2.WasPressed;
		bool redK = Input.GetButtonDown ("p0fire2");
		//bool yellow = inputDevice.Action4.WasPressed;
		bool esc = Input.GetButtonDown ("Cancel");
		
		kUp = Input.GetButtonDown ("p0u");
		kRight = Input.GetButtonDown ("p0r");
		kLeft = Input.GetButtonDown ("p0l");
		kDown = Input.GetButtonDown ("p0d");
		//filteredDirection = inputDevice.Direction;
		if (filteredDirection.Left.WasRepeated)
		{
			//Debug.Log( "!!!" );
		}

		// Move focus with directional inputs.
		if (arenaMenu) {
			if (red || redK || esc) {
				if(!ctrl.plyrMan.playerJoined[0] && !ctrl.plyrMan.playerJoined[1] && !ctrl.plyrMan.playerJoined[2] && !ctrl.plyrMan.playerJoined[3]) {

					if(!ctrl.goBack) {
						ctrl.goBack = true;
					} else {
						ctrl.GoToLevel(0);
					}
				}
			}

			if(ctrl.roundEnd && !ctrl.wepSelect) {
				CheckMovement();
				if (notFocused) {
					MoveFocusTo(GameObject.Find ("RematchText").GetComponent<Button>());
					notFocused = false;
				}
				if (green || greenK) {
					Press (focusedButton.gameObject.name);
					//Debug.Log (focusedButton.gameObject.name);
				}
			}
			if (ctrl.ready && ctrl.selectionDone) {
				notFocused = true;
				CheckMovement();
				if (green || greenK) {
					Press (focusedButton.gameObject.name);
					lastMapButton = focusedButton;
				}
			}
		}
		if (mainMenu) {
			if (focusedButton.gameObject.name == "SoundText") {
				if (filteredDirection.Left.WasPressed || kLeft) {
					sndSlider.value -= 0.1f;
				} else if (filteredDirection.Right.WasPressed || kRight) {
					sndSlider.value += 0.1f;
				}
			}
			if (focusedButton.gameObject.name == "MusicText") {
				if (filteredDirection.Left.WasPressed || kLeft) {
					musSlider.value -= 0.1f;
				} else if (filteredDirection.Right.WasPressed || kRight) {
					musSlider.value += 0.1f;
				}
			}
			CheckMovement();
			if (red || redK) {
				//camFollow.transform.position = new Vector3 (0, 25, -25);
				//MoveFocusTo (focusedButton.left);
			}
			if (green || greenK || enter) {
				Press (focusedButton.gameObject.name);
			}
		}
	}
	
	
	void MoveFocusTo( Button newFocusedButton )
	{
		if (newFocusedButton != null)
		{
			focusedButton = newFocusedButton;
			GetComponent<AudioSource>().PlayOneShot(moveSound, volume); // Play the AudioClip
		}
	}
	void Press (string textName) {
		if (mainMenu) {
			switch (textName) {
			case "PlayText":
				loadingPanel.GetComponent<Animator> ().SetBool ("DropDown", true);
				loadingPanel.GetComponent<Animator> ().SetBool ("LoadArena", true);
				break;
			case "OptionsText":
				camFollow.transform.position = new Vector3 (44, 25, -25);
				MoveFocusTo (focusedButton.link);
				tempSnd = sndSlider.value;
				tempMus = musSlider.value;
				if (Screen.fullScreen) {
					fullScreenTemp = true;
				} else {
					fullScreenTemp = false;
				}
				Color prefsColor = PlayerPrefsX.GetColor ("BGColor");
				currentBackColor = prefsColor;
				if (prefsColor == backColor [0]) {
					paletteText.text = "Palette: Default";
					paletteText.transform.name = "Default";
				} else if (prefsColor == backColor [1]) {
					paletteText.text = "Palette: Solarized Dark";
					paletteText.transform.name = "SolarizedDark";
				} else if (prefsColor == backColor [2]) {
					paletteText.text = "Palette: Base16";
					paletteText.transform.name = "Base16";
				} else if (prefsColor == backColor [3]) {
					paletteText.text = "Palette: Chalk";
					paletteText.transform.name = "Chalk";
				} else if (prefsColor == backColor [4]) {
					paletteText.text = "Palette: Mocha";
					paletteText.transform.name = "Mocha";
				}
				livesTemp = PlayerPrefs.GetInt ("LivesNum");
				livesText.text = "Lives: " + livesTemp;
				livesText.gameObject.name = "Lives" + livesTemp;
				turnClassicTemp = PlayerPrefs.GetInt ("TurnClassic");
				if (turnClassicTemp == 1) {
					turnText.text = "Turning: Classic";
					turnText.gameObject.name = "TurningClassic";
				} else {
					turnText.text = "Turning: Point";
					turnText.gameObject.name = "TurningPoint";
				}
				break;
			case "ControlsText":
				camFollow.transform.position = new Vector3 (0, 0, -25);
				MoveFocusTo (focusedButton.link);
				break;
			case "CreditsText":
				camFollow.transform.position = new Vector3 (-44, 25, -25);
				MoveFocusTo (focusedButton.link);
				break;
			case "BackCreditsText":
				camFollow.transform.position = new Vector3 (0, 25, -25);
				MoveFocusTo (focusedButton.link);
				break;
			case "QuitText":
				Application.Quit ();
				break;
			case "FullscreenText":
				if (Screen.fullScreen) {
					//Screen.fullScreen = !Screen.fullScreen;
					Screen.SetResolution (1280, 720, false);
					fullScreenText.text = "Full Screen";
				} else {
					Screen.fullScreen = !Screen.fullScreen;
					fullScreenText.text = "Windowed";
				}
				break;
			case "TurningClassic":
				turnText.text = "Turning: Point";
				turnText.transform.name = "TurningPoint";
				turnClassicTemp = 0;
				break;
			case "TurningPoint":
				turnText.text = "Turning: Classic";
				turnText.transform.name = "TurningClassic";
				turnClassicTemp = 1;
				break;
			case "Lives3":
				livesText.text = "Lives: 5";
				livesText.transform.name = "Lives5";
				livesTemp = 5;
				break;
			case "Lives5":
				livesText.text = "Lives: 1";
				livesText.transform.name = "Lives1";
				livesTemp = 1;
				break;
			case "Lives1":
				livesText.text = "Lives: 3";
				livesText.transform.name = "Lives3";
				livesTemp = 3;
				break;
			case "SaveText":
				PlayerPrefs.SetFloat ("MusVol", musSlider.value);
				PlayerPrefs.SetFloat ("SndVol", sndSlider.value);
				PlayerPrefsX.SetColorArray ("Palette", currentColorScheme);
				PlayerPrefsX.SetColor ("BGColor", currentBackColor);
				mainCam.backgroundColor = currentBackColor;
				PlayerPrefs.SetInt ("LivesNum", livesTemp);
				camFollow.transform.position = new Vector3 (0, 25, -25);
				PlayerPrefs.SetInt ("TurnClassic", turnClassicTemp);
				MoveFocusTo (focusedButton.link);
				break;
			case "BackText":
				camFollow.transform.position = new Vector3 (0, 25, -25);
				MoveFocusTo (focusedButton.link);
				sndSlider.value = tempSnd;
				musSlider.value = tempMus;
				if (Screen.fullScreen != fullScreenTemp) {
					if (Screen.fullScreen) {
						Screen.SetResolution (1280, 720, false);
						fullScreenText.text = "Full Screen";
					} else {
						Screen.fullScreen = !Screen.fullScreen;
						fullScreenText.text = "Windowed";
					}
				}
				mainCam.backgroundColor = PlayerPrefsX.GetColor ("BGColor");
				currentBackColor = mainCam.backgroundColor;
				currentColorScheme = PlayerPrefsX.GetColorArray ("Palette");
				break;
			case "Default":
				paletteText.text = "Palette: Solarized Dark";
				paletteText.transform.name = "SolarizedDark";
				currentColorScheme = solarisDarkScheme;
				currentBackColor = backColor [1];
				mainCam.backgroundColor = currentBackColor;
				break;
			case "SolarizedDark":
				paletteText.text = "Palette: Base16";
				paletteText.transform.name = "Base16";
				currentColorScheme = base16Scheme;
				currentBackColor = backColor [2];
				mainCam.backgroundColor = currentBackColor;
				break;
			case "Base16":
				paletteText.text = "Palette: Chalk";
				paletteText.transform.name = "Chalk";
				currentColorScheme = chalkScheme;
				currentBackColor = backColor [3];
				mainCam.backgroundColor = currentBackColor;
				break;
			case "Chalk":
				paletteText.text = "Palette: Mocha";
				paletteText.transform.name = "Mocha";
				currentColorScheme = mochaScheme;
				currentBackColor = backColor [4];
				mainCam.backgroundColor = currentBackColor;
				break;
			case "Mocha":
				paletteText.text = "Palette: Default";
				paletteText.transform.name = "Default";
				currentColorScheme = defaultScheme;
				currentBackColor = backColor [0];
				mainCam.backgroundColor = currentBackColor;
				break;
			}
		}

		if (arenaMenu) {
			if (textName == "RematchText") {
				ctrl.submit = true;
			} else if (textName == "WepSelectText") {
				ctrl.wepSelect = true;
			} else if (textName == "MainMenuText") {
				//SceneManager.LoadScene(0);
				loadingPanel.GetComponent<Animator> ().SetBool ("DropDown", true);
				loadingPanel.GetComponent<Animator> ().SetBool ("LoadMenu", true);
			}
		}
	}

	void MapSelect () {
		MoveFocusTo(lastMapButton);
	}

	void CheckMovement () {
		if (filteredDirection.Up.WasPressed || kUp)
		{
			MoveFocusTo( focusedButton.up );
		}
		
		if (filteredDirection.Down.WasPressed || kDown)
		{
			MoveFocusTo( focusedButton.down );
		}
		
		if (filteredDirection.Left.WasPressed || kLeft)
		{
			MoveFocusTo( focusedButton.left );
		}
		
		if (filteredDirection.Right.WasPressed || kRight)
		{
			MoveFocusTo( focusedButton.right );
		}
	}


}