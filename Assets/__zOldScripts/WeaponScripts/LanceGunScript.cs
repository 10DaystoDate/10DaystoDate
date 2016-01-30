using UnityEngine;
using System.Collections;
//Extends Gun
public class LanceGunScript : Gun {

	//public float rotaTorque = 150;
	public float destructTimer = 0.5f;
	
	//private GameObject aTemp;

	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
			playerCtrl.lastFireTime = Time.time;
			Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position + (transform.up*3), playerShip.transform.rotation) as Rigidbody2D;
			//aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
			//aShot.GetComponent<Rigidbody2D>().AddTorque(rotaTorque);
			//playerCtrl.ChangeSpeed(20, 20, 0.2f, true);
			aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aShot.transform.parent = playerShip.transform;
			aShot.tag = tagNum.ToString();
			GetComponent<AudioSource>().PlayOneShot(gunSound, volume);
			
			Destroy (aShot.gameObject, destructTimer);
		}
	}
}