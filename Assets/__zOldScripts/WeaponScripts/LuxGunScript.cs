using UnityEngine;
using System.Collections;
//Extends Gun
public class LuxGunScript : Gun {

	public Rigidbody2D luxBlast;

	//public float rotaTorque = 150;
	public float destructTimer = 0.5f;
	public float blastDestructTimer = 0.5f;

	
	public Transform luxTemp;

	public GameObject luxCharge;

	public Rigidbody2D luxResi;

	public bool fireLux = false;
	public float fireLuxTime;

	// Update is called once per frame
	void Update () {
		if (fireLux) {
			if (Time.time >= fireLuxTime + destructTimer - 0.05f) {
				fireLux = false;
				Rigidbody2D bShot = Instantiate(luxBlast, luxTemp.transform.position - transform.up, luxTemp.transform.rotation) as Rigidbody2D;
				bShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				bShot.transform.parent = playerCtrl.bulletContainer.transform;
				GetComponent<AudioSource>().PlayOneShot(gunSound2, volume); // Play the AudioClip
				bShot.tag = playerCtrl.playerNum.ToString();

				Invoke("LuxResi", blastDestructTimer);
				Destroy (bShot.gameObject, blastDestructTimer);
			}
		}
	}
	
	void Fire (int tagNum) {
		if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
			playerCtrl.lastFireTime = Time.time;
			Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position + transform.up, playerShip.transform.rotation) as Rigidbody2D;
			//aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aShot.transform.parent = playerCtrl.bulletContainer.transform;
			playerCtrl.ChangeSpeed (0, 0, destructTimer+blastDestructTimer, false);
			fireLux = true;
			fireLuxTime = Time.time;
			aShot.tag = tagNum.ToString();
			luxTemp = aShot.transform;
			GetComponent<AudioSource>().PlayOneShot(gunSound, volume); // Play the AudioClip
			Destroy (aShot.gameObject, destructTimer);

			GameObject luxChargeFX = Instantiate (luxCharge, playerShip.transform.position + transform.up, playerShip.transform.rotation) as GameObject;
			luxChargeFX.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			luxChargeFX.transform.parent = playerCtrl.bulletContainer.transform;
			Destroy (luxChargeFX, 1);
		}
	}

	void LuxResi () {
		Rigidbody2D aResi = Instantiate(luxResi, playerShip.transform.position + transform.up*26, playerShip.transform.rotation) as Rigidbody2D;
		aResi.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
		aResi.transform.parent = playerCtrl.bulletContainer.transform;
		Destroy (aResi, destructTimer);
	}
}