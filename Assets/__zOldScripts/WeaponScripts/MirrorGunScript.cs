using UnityEngine;
using System.Collections;
//Extends Gun
public class MirrorGunScript : Gun {

	//public float rotaTorque = 150;
	public float destructTimer = 0.5f;
	
	//private GameObject aTemp;

	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
			playerCtrl.lastFireTime = Time.time;
			Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
			//aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
			//aShot.GetComponent<Rigidbody2D>().AddTorque(rotaTorque);
			playerCtrl.ChangeSpeed(0, 0, 0.2f, true); //New speed, new min speed, length, set turn
			//aShot.GetComponent<SpriteRenderer> ().color = (playerCtrl.shipColor - new Color (0,0,0,0.5f));
			aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aShot.transform.parent = playerCtrl.bulletContainer.transform;
			aShot.tag = tagNum.ToString();
			GetComponent<AudioSource>().PlayOneShot(gunSound, volume);

			playerCtrl.transform.rotation = transform.rotation * Quaternion.AngleAxis (180, Vector3.forward); //Reverse direction
			Destroy (aShot.gameObject, destructTimer);
		}
	}
}