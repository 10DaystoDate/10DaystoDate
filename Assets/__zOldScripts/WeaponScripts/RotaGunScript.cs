using UnityEngine;
using System.Collections;
//Extends Gun
public class RotaGunScript : Gun {

	public float aTorque = 10;
	public Rigidbody2D aShotBlast;
	public float aBlastSpeed = 30;
	public GameObject aResiBlast;

	public GameObject aShotFX;
	
	private GameObject aTemp;

	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if(aTemp == false){
			if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
				playerCtrl.lastFireTime = Time.time;
				Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
				aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
				aShot.GetComponent<Rigidbody2D>().AddTorque(aTorque);
				aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
				aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShot.transform.parent = playerCtrl.bulletContainer.transform;
				aShot.tag = tagNum.ToString();
				GetComponent<AudioSource>().PlayOneShot(gunSound, volume); // Play the AudioClip
				aTemp = aShot.gameObject;

				playerCtrl.ChangeSpeed (3, 3, 0.14f, true);

				GameObject aShotFXI = Instantiate (aShotFX, playerShip.transform.position + transform.up*1.2f, playerShip.transform.rotation * Quaternion.Euler (-90, 0, 0)) as GameObject;
				aShotFXI.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShotFXI.transform.parent = playerCtrl.bulletContainer.transform;
				Destroy (aShotFXI, 1);
			}
		} else {
			Rigidbody2D aShotBlastInstance = Instantiate (aShotBlast, aTemp.transform.position, aTemp.transform.rotation) as Rigidbody2D;
			aShotBlastInstance.velocity = aTemp.transform.TransformDirection (Vector3.up * aBlastSpeed);
			aShotBlastInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShotBlastInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aShotBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
			aShotBlastInstance.tag = tagNum.ToString();

			GetComponent<AudioSource>().PlayOneShot(gunSound2, volume); // Play the AudioClip
			Destroy (aTemp);

			GameObject aResi = Instantiate (aResiBlast, aTemp.transform.position, aTemp.transform.rotation) as GameObject;
			aResi.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			Destroy (aResi, 1);
		}
	}
}