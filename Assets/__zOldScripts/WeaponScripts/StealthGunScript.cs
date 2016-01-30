using UnityEngine;
using System.Collections;
//Extends Gun
public class StealthGunScript : Gun {

	//public float rotaTorque = 150;
	//public float destructTimer = 0.5f;
	public float stealthTimer = 0.5f;

	public GameObject aResi;
	
	//private GameObject aTemp;

	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
			playerCtrl.lastFireTime = Time.time;
			//Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position + (transform.up/2), playerShip.transform.rotation) as Rigidbody2D;
			//aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
			//aShot.GetComponent<Rigidbody2D>().AddTorque(rotaTorque);

			GameObject aShot = Instantiate(aResi, playerShip.transform.position + transform.up, playerShip.transform.rotation) as GameObject;
			playerCtrl.SetStealth(stealthTimer);
			//aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			//aShot.transform.parent = playerShip.transform;
			//aShot.tag = tagNum.ToString();
			GetComponent<AudioSource>().PlayOneShot(gunSound, volume);
			Destroy (aShot.gameObject, 1f);
			
			//Destroy (aShot.gameObject, destructTimer);
		}
	}
}