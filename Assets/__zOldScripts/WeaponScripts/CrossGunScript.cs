using UnityEngine;
using System.Collections;

//CrossGunSCript extens Gun

public class CrossGunScript : Gun {
	
	public Rigidbody2D crossShotBlastL;
	public Rigidbody2D crossShotBlastR;
	public float crossBlastSpeed = 10;
	public GameObject aResiBlast;

	private GameObject aTemp;

	public GameObject aShotFX;
	
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
		if (aTemp == true) {
			aTemp.transform.rotation = playerCtrl.transform.rotation;
		}
	}
	
	public void Fire (int tagNum) {
		if(aTemp == false){
			if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
				playerCtrl.lastFireTime = Time.time;
				Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
				aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
				aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
				aShot.transform.parent = playerCtrl.bulletContainer.transform;
				aShot.tag = tagNum.ToString();
				aTemp = aShot.gameObject;
				GetComponent<AudioSource>().PlayOneShot(gunSound, volume); // Play the AudioClip

				playerCtrl.ChangeSpeed (3, 3, 0.2f, true);

				GameObject aShotFXI = Instantiate (aShotFX, playerShip.transform.position + transform.up*1.2f, playerShip.transform.rotation * Quaternion.Euler (-90, 0, 0)) as GameObject;
				aShotFXI.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShotFXI.transform.parent = playerCtrl.bulletContainer.transform;
				Destroy (aShotFXI, 1);
			}
		} else {
			Rigidbody2D aShotBlastInstance = Instantiate (crossShotBlastL, aTemp.transform.position, aTemp.transform.rotation) as Rigidbody2D;
			aShotBlastInstance.velocity = aTemp.transform.TransformDirection (Vector3.left * crossBlastSpeed);
			aShotBlastInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShotBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
			aShotBlastInstance.tag = tagNum.ToString();
			aShotBlastInstance = Instantiate (crossShotBlastR, aTemp.transform.position, aTemp.transform.rotation) as Rigidbody2D;
			aShotBlastInstance.velocity = aTemp.transform.TransformDirection (Vector3.right * crossBlastSpeed);
			aShotBlastInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShotBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
			aShotBlastInstance.tag = tagNum.ToString();
			GetComponent<AudioSource>().PlayOneShot(gunSound2, volume); // Play the AudioClip
			//GameObject teleShotInInstance = Instantiate(teleShotIn, shieldEmitter.position, shieldEmitter.rotation) as GameObject;
			Destroy (aTemp);

			GameObject aResi = Instantiate (aResiBlast, aTemp.transform.position, aTemp.transform.rotation * Quaternion.AngleAxis (90, Vector3.forward)) as GameObject;
			aResi.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			GameObject bResi = Instantiate (aResiBlast, aTemp.transform.position, aTemp.transform.rotation * Quaternion.AngleAxis (-90, Vector3.forward)) as GameObject;
			bResi.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			Destroy (aResi, 1);
			Destroy (bResi, 1);
		}
	}
}
