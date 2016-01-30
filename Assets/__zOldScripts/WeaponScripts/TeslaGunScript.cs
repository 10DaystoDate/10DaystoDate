using UnityEngine;
using System.Collections;
//Extends Gun
public class TeslaGunScript : Gun {

	public AudioClip zapSound;
	public float aTorque = 10;
	public Rigidbody2D bShot;
	public float bShotSpeed = 30;
	public float destructTime = 1;
	public float chargeTime = 1;

	public float shockWidth = 0.1f;

	private LineRenderer lineRend;
	public GameObject aShotFX;

	//public float aBlastDestructTime = 1;

	private GameObject aTemp;
	private GameObject bTemp;
	private GameObject aBlast;
	private GameObject bBlast;

	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if (bTemp == false) {
			if (aTemp == false) {
				if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay) && !aBlast) {
					playerCtrl.lastFireTime = Time.time;
					Rigidbody2D aShot;
					aShot = Instantiate (gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
					aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
					aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
					aShot.transform.parent = playerCtrl.bulletContainer.transform;
					aShot.tag = tagNum.ToString ();
					GetComponent<AudioSource> ().PlayOneShot (gunSound, volume); // Play the AudioClip
					aTemp = aShot.gameObject;
					playerCtrl.ChangeSpeed (3, 3, 0.1f, true);

					GameObject aShotFXI = Instantiate (aShotFX, playerShip.transform.position + transform.up*1.2f, playerShip.transform.rotation * Quaternion.Euler (-90, 0, 0)) as GameObject;
					aShotFXI.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
					aShotFXI.transform.parent = playerCtrl.bulletContainer.transform;
					Destroy (aShotFXI, 1);
				}
			} else {
				Rigidbody2D bShot = Instantiate (gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
				bShot.velocity = transform.TransformDirection (Vector3.up * bShotSpeed);
				bShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				//aShotBlastInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				bShot.transform.parent = playerCtrl.bulletContainer.transform;
				bShot.tag = tagNum.ToString ();
				GetComponent<AudioSource> ().PlayOneShot (gunSound, volume); // Play the AudioClip
				bTemp = bShot.gameObject;
				playerCtrl.ChangeSpeed (3, 3, 0.1f, true);

				GameObject aShotFXI = Instantiate (aShotFX, playerShip.transform.position + transform.up*1.2f, playerShip.transform.rotation * Quaternion.Euler (-90, 0, 0)) as GameObject;
				aShotFXI.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShotFXI.transform.parent = playerCtrl.bulletContainer.transform;
				Destroy (aShotFXI, 1);
			}
		} else {
			if (aTemp == true) { //If bTemp and atemp exist
				Rigidbody2D aBlastInstance = Instantiate (bShot, aTemp.transform.position, aTemp.transform.rotation) as Rigidbody2D;
				aBlastInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
				aBlast = aBlastInstance.gameObject;
				Rigidbody2D bBlastInstance = Instantiate (bShot, bTemp.transform.position, bTemp.transform.rotation) as Rigidbody2D;
				bBlastInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				bBlastInstance.tag = tagNum.ToString ();
				bBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
				bBlast = bBlastInstance.gameObject;
				GetComponent<AudioSource> ().PlayOneShot (gunSound2, volume); // Play the AudioClip
				Destroy (aTemp);
				Destroy (bTemp);
				Invoke ("SpawnLightning", chargeTime);
				//Invoke ("KillBug", chargeTime);

			} else {
				Rigidbody2D aShot;
				aShot = Instantiate (gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
				aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
				aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShot.transform.parent = playerCtrl.bulletContainer.transform;
				aShot.tag = tagNum.ToString ();
				GetComponent<AudioSource> ().PlayOneShot (gunSound, volume); // Play the AudioClip
				aTemp = aShot.gameObject;
				playerCtrl.ChangeSpeed (3, 3, 0.1f, true);

				GameObject aShotFXI = Instantiate (aShotFX, playerShip.transform.position + transform.up*1.2f, playerShip.transform.rotation * Quaternion.Euler (-90, 0, 0)) as GameObject;
				aShotFXI.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShotFXI.transform.parent = playerCtrl.bulletContainer.transform;
				Destroy (aShotFXI, 1);
			}
		}
	}
	void SpawnLightning () {
		lineRend = bBlast.GetComponent<LineRenderer> ();
		lineRend.SetVertexCount (2);
		lineRend.SetPosition (0, aBlast.transform.position);
		lineRend.SetPosition (1, bBlast.transform.position);
		lineRend.SetColors (playerCtrl.shipColor, playerCtrl.shipColor);
		addColliderToLine ();
		GetComponent<AudioSource> ().PlayOneShot (zapSound, volume); // Play the AudioClip
		Destroy (aBlast, destructTime);
		Destroy (bBlast, destructTime);
	}
	private void addColliderToLine()
	{
		BoxCollider2D col = new GameObject("TeslaCoil").AddComponent<BoxCollider2D> ();
		col.transform.parent = bBlast.transform; // Collider is added as child object of line
		float lineLength = Vector3.Distance (aBlast.transform.position, bBlast.transform.position); // length of line
		col.size = new Vector3 (lineLength, shockWidth, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
		Vector3 midPoint = (aBlast.transform.position + bBlast.transform.position)/2;
		col.transform.position = midPoint; // setting position of collider object
		// Following lines calculate the angle between startPos and endPos
		float angle = (Mathf.Abs (aBlast.transform.position.y - bBlast.transform.position.y) / Mathf.Abs (aBlast.transform.position.x - bBlast.transform.position.x));
		if((aBlast.transform.position.y<bBlast.transform.position.y && aBlast.transform.position.x>bBlast.transform.position.x) || bBlast.transform.position.y<aBlast.transform.position.y && bBlast.transform.position.x>aBlast.transform.position.x) {
			angle*=-1;
		}
		angle = Mathf.Rad2Deg * Mathf.Atan (angle);
		col.transform.Rotate (0, 0, angle);
		col.tag = bBlast.tag;
	}
	//www.theappguruz.com/blog/add-collider-to-line-renderer-unity#sthash.9dXcA5Ky.dpuf

	void KillBug () {
		if (!aBlast && bBlast) {
			Destroy (bBlast);
		}
		if (!bBlast && aBlast) {
			Destroy (aBlast);
		}
	}
}