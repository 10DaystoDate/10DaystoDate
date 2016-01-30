using UnityEngine;
using System.Collections;

//CrossGunSCript extends Gun

public class VGunScript : Gun {
	
	// Use this for initialization

	//public float changeTime = 1;
	public float angleChange = 10;
	//public float dashTime = 0.3f;
	//public float dashCooldown = 1;

	public AudioClip changeSound;

	//private float resetDashTime = 0;

	private bool oddChange = false;
	private GameObject aTemp;
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Fire (int tagNum) {
		if (aTemp == false) {
			if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
				playerCtrl.lastFireTime = Time.time;
				/*Rigidbody2D aShotInstance = Instantiate (gunBullet, playerShip.transform.position, playerShip.transform.rotation * Quaternion.Euler (0, 0, 10)) as Rigidbody2D;
				aShotInstance.velocity = aShotInstance.transform.TransformDirection (Vector3.up * gunShotSpeed);
				aShotInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
				aShotInstance.transform.parent = playerCtrl.bulletContainer.transform;
				aShotInstance.tag = tagNum.ToString ();*/
				Rigidbody2D aShotInstance = Instantiate (gunBullet, playerShip.transform.position + transform.up, playerShip.transform.rotation * Quaternion.Euler (0, 0, -angleChange)) as Rigidbody2D;
				aShotInstance.velocity = aShotInstance.transform.TransformDirection (Vector3.up * gunShotSpeed);
				aShotInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
				aShotInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShotInstance.transform.parent = playerCtrl.bulletContainer.transform;
				aShotInstance.tag = tagNum.ToString ();
				GetComponent<AudioSource> ().PlayOneShot (gunSound, volume); // Play the AudioClip
				aTemp = aShotInstance.gameObject;
				aTemp.transform.GetChild(0).rotation = aTemp.transform.rotation * Quaternion.AngleAxis (angleChange*2, Vector3.forward);
				oddChange = true;
				//CancelInvoke ("DirectionChange");
				//InvokeRepeating ("DirectionChange", changeTime/2, changeTime);

				playerCtrl.ChangeSpeed (3, 3, 0.1f, true);
			}
		} else {
			//if (Time.time >= resetDashTime) {
				//aTemp.GetComponent<Rigidbody2D> ().velocity = aTemp.transform.TransformDirection (Vector3.up * gunShotSpeed * 3);
				//resetDashTime = Time.time + dashCooldown;
				//CancelInvoke ("DirectionChange");
				//Invoke ("SpeedReset", dashTime);
			DirectionChange ();
			//}

		}
	}

	void DirectionChange () {
		if (aTemp) {
			if (oddChange == true) {
				aTemp.transform.rotation = aTemp.transform.rotation * Quaternion.AngleAxis (angleChange*2, Vector3.forward); //Reverse direction
				oddChange = false;
				aTemp.transform.GetChild(0).rotation = aTemp.transform.rotation * Quaternion.AngleAxis (-angleChange*2, Vector3.forward);
			} else {
				aTemp.transform.rotation = aTemp.transform.rotation * Quaternion.AngleAxis (-angleChange*2, Vector3.forward); //Reverse direction
				oddChange = true;
				aTemp.transform.GetChild(0).rotation = aTemp.transform.rotation * Quaternion.AngleAxis (angleChange*2, Vector3.forward);
			}
			GetComponent<AudioSource> ().PlayOneShot (changeSound, volume); // Play the AudioClip
			aTemp.GetComponent<Rigidbody2D>().velocity = aTemp.transform.TransformDirection (Vector3.up * gunShotSpeed);
		}
	}

	/*void SpeedReset () {
		if (aTemp) {
			aTemp.GetComponent<Rigidbody2D> ().velocity = aTemp.transform.TransformDirection (Vector3.up * gunShotSpeed);
			InvokeRepeating ("DirectionChange", 0, changeTime);
		}
	}*/
}
