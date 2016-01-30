using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	
	public Rigidbody2D gunBullet;
	public float gunShotSpeed = 10;
	public AudioClip gunSound; // Add a AudioClip reference
	public AudioClip gunSound2; // Add a AudioClip reference

	public GameObject playerShip;
	public PlayerController playerCtrl;
	
	public Transform thisGun;

	public float volume;

	// Use this for initialization
	void Start () {
		thisGun = this.transform;
		playerShip = transform.parent.gameObject;
		playerCtrl = playerShip.GetComponent<PlayerController>();
		volume = PlayerPrefs.GetFloat ("SndVol");
	}
	
	// Update is called once per frame
}
