using UnityEngine;
using InControl;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
	public Button focusedButton;
	public GameObject camFollow;
	public Camera mainCam;
	public bool notFocused = false;

	public AudioClip moveSound; // Add a AudioClip reference

	private bool kUp;
	private bool kRight;
	private bool kDown;
	private bool kLeft;

	public GameObject mainController;
	public Ctrl ctrl;

	TwoAxisInputControl filteredDirection;

	void Awake()
	{
		filteredDirection = new TwoAxisInputControl();
		filteredDirection.StateThreshold = 0.5f;
	}
	
	
	void Update()
	{
		// Use last device which provided input.
		var inputDevice = InputManager.ActiveDevice;
		filteredDirection.Filter( inputDevice.Direction, Time.deltaTime );
		bool green = inputDevice.Action1.WasPressed;
		bool greenK = Input.GetButtonDown ("p0fire1");
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

		if (filteredDirection.Up.WasPressed || kUp) {
			
		}

		if (green || greenK) {
		}

		if (red || redK || esc) {
			Press (focusedButton.gameObject.name);
		}

		CheckMovement();
	}
	
	
	void MoveFocusTo( Button newFocusedButton )
	{
		if (newFocusedButton != null)
		{
			focusedButton = newFocusedButton;
			//GetComponent<AudioSource>().PlayOneShot(moveSound, volume); // Play the AudioClip
		}
	}
	void Press (string textName) {
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