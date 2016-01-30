using UnityEngine;
using System.Collections;
//Extends Gun
public class CrashGunScript : Gun {

	//public float rotaTorque = 150;
	public float destructTimer = 0.5f;
	public GameObject aResiBlast;
	public float crashSpeed = 20;
	public float crashLength = 0.2f;
	public bool canTurn = true;
	
	//private GameObject aTemp;

	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
			playerCtrl.lastFireTime = Time.time;
			Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position + (transform.up/2), playerShip.transform.rotation) as Rigidbody2D;
			//aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
			//aShot.GetComponent<Rigidbody2D>().AddTorque(rotaTorque);
			playerCtrl.ChangeSpeed(crashSpeed, crashSpeed, crashLength, canTurn);
			aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShot.transform.parent = playerShip.transform;
			aShot.tag = tagNum.ToString();
			GetComponent<AudioSource>().PlayOneShot(gunSound, volume);
			
			Destroy (aShot.gameObject, destructTimer);

			GameObject aResi = Instantiate (aResiBlast, playerShip.transform.position + (transform.up/2), playerShip.transform.rotation) as GameObject;
			aResi.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aResi.transform.parent = playerShip.transform;
			Destroy (aResi, 1);
		}
	}
}