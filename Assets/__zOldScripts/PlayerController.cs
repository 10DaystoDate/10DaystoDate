using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	//Variables that are the same for all ships (get from maincontroller)
	public float speed = 10;
	public float minSpeed = 5;
	private float origSpeed;
	private float origMinSpeed;
	public float turnSpeed = 160;
	public float weaponDelay = 0.8f; //Delay for weapons
	public Color shipColor;
	public float resetSpeedTime;
	public float resetStealthTime;
	public bool canTurn = true;
	public int killerTag;
	public AudioClip hitSound; // Add a AudioClip reference

	public GameObject shipExplosion;
	public GameObject shipExplosionBlast;
	public GameObject shipHit;


	public GameObject bulletContainer;

	public float lastFireTime; //Time last fired for each weapon


	//Player number, only thing different for each player
	public int playerNum = 0;
	//Default gun
	//Main controller
	public GameObject mainController;
	public GameObject[] weaponSet;
	public GameObject playerInput;
	public Player pIn;
	public Vector3[] weaponPos = new [] {new Vector3(0,0.5f,0), new Vector3(0,0,0), new Vector3(0,-0.5f,0)};
	
	public GameObject smokeTrail;
	public Ctrl ctrl;
	//Weapon slots
	public int[] wepNum;
	public GameObject[] wepSlot;

	public Vector4 heartShipPos;
	private int maxHealth = 0;
	public int startingHealth = 300;
	public int healthPerHeart = 100;

	private int currentHealth;

	public GUITexture heartGUI;
	public Texture[] images;

	private ArrayList hearts = new ArrayList();

	public int maxHeartsPerRow = 1;
	private float spacingX;
	private float spacingY;

	private float rotationDir = 0;
	public float rotationSpeed = 1;

	public int classicTurning;



	// Use this for initialization

	void Awake () {
		//Get ship variables from main controller
		mainController = GameObject.Find ("MainController");
		ctrl = mainController.GetComponent<Ctrl>();
		weaponSet = ctrl.weaponSet;
	}

	void Start () {
		pIn = playerInput.GetComponent<Player>();
		//Get/Set weapons from Main Ctrl on creation
		wepNum [0] = ctrl.wepNum1 [playerNum];
		
		//Set variables from mainController
		origSpeed = speed;
		origMinSpeed = minSpeed;
		bulletContainer = mainController;
		shipColor = ctrl.shipColor[playerNum];

		transform.GetChild(0).GetComponent<ParticleSystem> ().startColor = shipColor;

		//Instantiate Weapon guns
		for (int i = 0; i <3; i++) {
			GameObject wep = Instantiate (weaponSet [wepNum[i]], this.transform.position - ((transform.up*i/3f)-(transform.up*1/2.5f)), this.transform.rotation) as GameObject;

			wep.transform.parent = this.transform;
			wep.GetComponent<SpriteRenderer> ().color = shipColor;
			wepSlot[i] = wep;
		}
		//Health Heart Bar
		heartShipPos = ctrl.heartBarPos[playerNum];
		currentHealth = startingHealth;
		//spacingX = -heartGUI.pixelInset.width * 5 * Screen.width/960;
		//spacingY = -heartGUI.pixelInset.height * 1 * Screen.height/540;
		spacingX = -heartGUI.pixelInset.width;
		spacingY = -heartGUI.pixelInset.height;

		AddHearts (startingHealth/healthPerHeart);
	}
	
	// Update is called once per frame
	void Update () {
		if (currentHealth <= 0) {
			PlayerDeath ();
		}
		
		//Bool controls
		//bool up = pIn.Actions.Up;
		bool rTrig = pIn.Actions.RTrigger;
		//bool down = pIn.Actions.Down;
		bool left = pIn.Actions.Left;
		bool right = pIn.Actions.Right;
		bool fire1 = pIn.Actions.Blue.WasPressed;
		bool fire2 = pIn.Actions.Green.WasPressed;
		bool fire3 = pIn.Actions.Red.WasPressed;


		//Movement
			if (rTrig) {
				GetComponent<Rigidbody2D> ().velocity = transform.up * speed;		
			} else {
				GetComponent<Rigidbody2D> ().velocity = transform.up * minSpeed;
			}

		if (classicTurning == 1) {
			if (canTurn == true) {
				if (left) {
					transform.Rotate (Vector3.forward * turnSpeed * Time.deltaTime);
				}
				if (right) {
					transform.Rotate (Vector3.forward * -turnSpeed * Time.deltaTime);
				}
			}
		} else {
			Vector2 joystickPos = pIn.Actions.Rotate;
			if (joystickPos != Vector2.zero) {
				if (canTurn == true) {
					// Now add in our joysticks position
					rotationDir = Mathf.Atan2 (-joystickPos.x, joystickPos.y) * Mathf.Rad2Deg;
					transform.rotation = Quaternion.Slerp (transform.rotation, (Quaternion.Euler (0, 0, rotationDir)), Time.deltaTime * rotationSpeed);

				}
			}
		}
		//Weapon Fire
		if (Time.timeScale > 0) {
			if (fire1) {
				wepSlot [0].SendMessage ("Fire", playerNum);
			}
			if (fire2) {
				wepSlot [1].SendMessage ("Fire", playerNum);
			}
			if (fire3) {
				wepSlot [2].SendMessage ("Fire", playerNum);
			}
		}

		//Screen Wrapping
		if(transform.position.x > 21.5f) {
			transform.position = new Vector3(-21.5f,transform.position.y, transform.position.z);
		}
		else if(transform.position.x <-21.5f){
			transform.position = new Vector3(21.5f,transform.position.y, transform.position.z);
		}
		if(transform.position.y > 12f) {
			transform.position = new Vector3(transform.position.x, -12f, transform.position.z);
		}
		else if(transform.position.y < -12f) {
			transform.position = new Vector3(transform.position.x, 12f, transform.position.z);
		}

		if (Time.time >= resetSpeedTime) {
			speed = origSpeed;
			minSpeed = origMinSpeed;
			canTurn = true;
		}
		if (Time.time >= resetStealthTime) {
			transform.GetChild(0).GetComponent<ParticleSystem> ().startColor = shipColor;
			for (int i = 0; i < 3; i++) {
				wepSlot [i].GetComponent<SpriteRenderer> ().color = shipColor;
			}
		}

	}

	//Collision with player or objects/bullets
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Star") { //if tag is star
			ModifyHealth(-100);
			killerTag = 9;
		} else if (other.gameObject.tag != playerNum.ToString ()) { //If object tag is not equal to ships tag
			if (other.gameObject.name == "MachBullet(Clone)") {
				ModifyHealth(-50);
				//GetComponent<AudioSource> ().PlayOneShot (hitSound); // Play the AudioClip
				//GameObject wep = Instantiate (machBlastMark, this.transform.position, this.transform.rotation) as GameObject;
				//wep.GetComponent<SpriteRenderer> ().color = shipColor;
				//wep.transform.parent = this.transform;
			} else {
				ModifyHealth(-100); //If hit by own weapon i think
			}
			killerTag = int.Parse (other.gameObject.tag);
		} else if (other.gameObject.tag == playerNum.ToString ()) {
			if (other.gameObject.name == "RotaBlast(Clone)" || 
			    other.gameObject.name == "MineBlast(Clone)" ||
				other.gameObject.name == "TeslaCoil") {
				ModifyHealth(-100);
				killerTag = 9;
			}
		}
	}


	public void ChangeSpeed (float newSpeed, float newMinSpeed, float length, bool setTurn) {
		speed = newSpeed;
		minSpeed = newMinSpeed;
		resetSpeedTime = Time.time + length;
		canTurn = setTurn;
	}

	public void SetStealth (float length) {
		resetStealthTime = Time.time + length;
		transform.GetChild(0).GetComponent<ParticleSystem> ().startColor = new Color(0,0,0,0);
		for (int i = 0; i < 3; i++) {
			wepSlot[i].GetComponent<SpriteRenderer> ().color = new Color(0,0,0,0);
		}
	}

	void CreateExplosion () {
		GameObject shipEx = Instantiate (shipExplosion, this.transform.position, this.transform.rotation) as GameObject;
		shipEx.GetComponent<ParticleSystem>().startColor = shipColor;
		shipEx.transform.parent = ctrl.bulletContainer.transform;
		if (ctrl.lastTwoPlayers == true) {
			ctrl.TimeStopActivate (0.2f, 0.35f);
		}
		GameObject shipExB = Instantiate (shipExplosionBlast, this.transform.position, this.transform.rotation) as GameObject;
		shipExB.GetComponent<ParticleSystem>().startColor = shipColor;
		shipExB.transform.parent = ctrl.bulletContainer.transform;
	}
	void CreateHit () { //When ship is hit
		GameObject shipEx = Instantiate (shipHit, this.transform.position, this.transform.rotation) as GameObject;
		shipEx.GetComponent<ParticleSystem>().startColor = shipColor;
		shipEx.transform.parent = ctrl.bulletContainer.transform;
	}

	void PlayerDeath () {
		CreateExplosion ();

		if(killerTag == 9) {
			mainController.SendMessage ("Suicide", playerNum);
		} else {
			mainController.SendMessage ("AddScore", killerTag);
		}
		currentHealth = 100;
		Destroy (gameObject);
	}
	//Heartbar stuff
	public void AddHearts(int n){
		for (int i = 0; i < n; i ++) {
			Transform newHeart = ((GameObject)Instantiate(heartGUI.gameObject,bulletContainer.transform.position,Quaternion.identity)).transform; //gets transform of instantiated gameobject
			newHeart.parent = bulletContainer.transform; //makes the hearts children of game object
			newHeart.gameObject.layer = 8;

			//FloorToInt returns lowest integer of float (ie. if 5.8, returns 5)
			int y = Mathf.FloorToInt(hearts.Count / maxHeartsPerRow); // until hearts.count/maxhearts equals 1, int y will be equal to 0
			int x = hearts.Count - y * maxHeartsPerRow; //resets x position for new row of hearts
			//Uses vector 4 to determine if small subtractions should be done to heartbar position
			newHeart.GetComponent<GUITexture>().pixelInset = new Rect(heartShipPos.x - x*spacingX + spacingX*3*heartShipPos.z, heartShipPos.y + y*spacingY + spacingY*heartShipPos.w, -spacingX, -spacingY); //size and position of heartbar
			newHeart.GetComponent<GUITexture>().color = shipColor/2;
			hearts.Add (newHeart);
		}
		maxHealth += n * healthPerHeart; //so maxHealth increases with added hearts
		currentHealth = maxHealth; //Health gets refilled when gaining a new heart
		UpdateHearts();

	}

	public void ModifyHealth(int amount){
		currentHealth += amount;
		currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);//restricts the variable to a min and max (0 and maxHealth in this case)
		UpdateHearts();
		if (amount <= 0) {
			if (currentHealth >= 1) {
				CreateHit ();
			}
		}
		if (currentHealth <= 100) {
			GameObject wep = Instantiate (smokeTrail, this.transform.position, this.transform.rotation) as GameObject;
			wep.GetComponent<SpriteRenderer> ().color = shipColor;
			wep.transform.parent = this.transform;
		}
	}

	private void UpdateHearts () {
		bool restAreEmpty = false; //if we find one of the hearts is empty or partially empty, all the hearts after that will be empty
		int i = 0;
		foreach (Transform heart in  hearts) {
			if(restAreEmpty) {
				heart.GetComponent<GUITexture>().texture = images[0];
			} 
			else{
				i += 1;
				if (currentHealth >= i * healthPerHeart){ //detects if heart is full 
					heart.GetComponent<GUITexture>().texture = images[images.Length-1];
				}
				else{
					int currentHeartHealth = (int)(healthPerHeart - (healthPerHeart * i - currentHealth)); //calculates health in current heart
					int healthPerImage = healthPerHeart / images.Length; //how much health each image represents
					int imageIndex = currentHeartHealth / healthPerImage;

					if(imageIndex == 0 && currentHeartHealth > 0) {
						imageIndex = 1;
					}

					heart.GetComponent<GUITexture>().texture = images[imageIndex];
					restAreEmpty = true;
				}
			}
		}
	}

}
