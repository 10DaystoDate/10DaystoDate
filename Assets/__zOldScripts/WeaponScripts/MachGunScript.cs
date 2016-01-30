using UnityEngine;
using System.Collections;
//Extends Gun
public class MachGunScript : Gun {

	public float destructTimer = 0.1f;
	public GameObject aShotFX;

	private int tagNumX;

	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
			Shoot (tagNum);
			StartCoroutine (TimeStop (0.4f));
		}
	}

	IEnumerator TimeStop (float length) {
		yield return new WaitForSeconds (length);
		Shoot (tagNumX);
	}

	void Shoot (int tagNum) {
		tagNumX = tagNum;
		playerCtrl.lastFireTime = Time.time;
		Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
		aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
		aShot.transform.parent = playerCtrl.bulletContainer.transform;
		aShot.tag = tagNum.ToString();
		GetComponent<AudioSource>().PlayOneShot(gunSound, 0.4f*volume); // Play the AudioClip

		playerCtrl.ChangeSpeed (3, 3, 0.05f, true);

		Destroy (aShot.gameObject, destructTimer);
		GameObject aShotFXI = Instantiate (aShotFX, playerShip.transform.position + transform.up*1.2f, playerShip.transform.rotation * Quaternion.Euler (-90, 0, 0)) as GameObject;
		aShotFXI.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
		aShotFXI.transform.parent = playerCtrl.bulletContainer.transform;
		Destroy (aShotFXI, 1);

	}

}