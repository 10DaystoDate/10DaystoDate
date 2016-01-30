using UnityEngine;
using System.Collections;

public class GridGunScript : Gun {

	// Use this for initialization
	// Update is called once per frame
	void Update () {
	
	}

	void Fire (int tagNum) {
		if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
			playerCtrl.lastFireTime = Time.time;
			Rigidbody2D aShot = Instantiate (gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D; //Instantiate bullet
			aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed); //Add velocity to bullet
			aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShot.transform.parent = playerCtrl.bulletContainer.transform;
			aShot.tag = tagNum.ToString (); //Tag bullet with player number
			GetComponent<AudioSource>().PlayOneShot(gunSound); // Play the AudioClip
		}
	}
}
