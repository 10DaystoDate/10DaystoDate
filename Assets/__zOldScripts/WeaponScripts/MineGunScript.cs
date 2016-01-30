using UnityEngine;
using System.Collections;
//Extends Gun
public class MineGunScript : Gun {
	
	public float aTorque = 150;
	public Rigidbody2D aShotBlast;
	public float destructTimer = 0.5f;
	public float destructBlastTimer = 0.5f;
	public GameObject aResiBlast;
	
	
	private GameObject aTemp;
	
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if(aTemp == false){
			if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
				playerCtrl.lastFireTime = Time.time;
				Rigidbody2D aShot;
				aShot = Instantiate(gunBullet, playerShip.transform.position + (-transform.up/2), playerShip.transform.rotation) as Rigidbody2D;
				aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
				aShot.GetComponent<Rigidbody2D>().AddTorque(aTorque);
				playerCtrl.ChangeSpeed (2, 2, 0.2f, true);
				aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
				aShot.transform.parent = playerCtrl.bulletContainer.transform;
				aShot.tag = tagNum.ToString();
				GetComponent<AudioSource>().PlayOneShot(gunSound, volume); // Play the AudioClip
				aTemp = aShot.gameObject;
				CancelInvoke ("AutoExplode");
				Invoke("AutoExplode", destructTimer);
				/*
				//GameObject rotaShotSoundInstance;
				//rotaShotSoundInstance = Instantiate(rotaShotSound, shieldEmitter.position, shieldEmitter.rotation) as GameObject;
				rotaShot.tag = tagNum.ToString();
				rotaPointerTemp = rotaShot.gameObject;*/
			}
		} else {
			//GameObject rotaShotSoundInstance2;
			//rotaShotSoundInstance2 = Instantiate(rotaShotSound2, shieldEmitter.position, shieldEmitter.rotation) as GameObject;
			
			Rigidbody2D aShotBlastInstance = Instantiate (aShotBlast, aTemp.transform.position, aTemp.transform.rotation) as Rigidbody2D;
			//aShotBlastInstance.velocity = aTemp.transform.TransformDirection (Vector3.up * aBlastSpeed);
			//Debug.Log (rotaShotObj.transform.rotation);
			//aShotBlastInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShotBlastInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aShotBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
			aShotBlastInstance.tag = tagNum.ToString();
			GetComponent<AudioSource>().PlayOneShot(gunSound2, volume); // Play the AudioClip
			Destroy (aShotBlastInstance.gameObject, destructBlastTimer);
			Destroy (aTemp);

			GameObject aResi = Instantiate (aResiBlast, aTemp.transform.position, aTemp.transform.rotation) as GameObject;
			aResi.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aResi.transform.parent = playerShip.transform;
			Destroy (aResi, 1);
		}
	}

	void AutoExplode () {
		if (aTemp) {
			Rigidbody2D aShotBlastInstance = Instantiate (aShotBlast, aTemp.transform.position, aTemp.transform.rotation) as Rigidbody2D;
			//aShotBlastInstance.velocity = aTemp.transform.TransformDirection (Vector3.up * aBlastSpeed);
			//Debug.Log (rotaShotObj.transform.rotation);
			//aShotBlastInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShotBlastInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aShotBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
			aShotBlastInstance.tag = aTemp.tag;
			GetComponent<AudioSource> ().PlayOneShot (gunSound2, volume); // Play the AudioClip
			Destroy (aShotBlastInstance.gameObject, destructBlastTimer);
			Destroy (aTemp);

			GameObject aResi = Instantiate (aResiBlast, aTemp.transform.position, aTemp.transform.rotation) as GameObject;
			aResi.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aResi.transform.parent = playerShip.transform;
			Destroy (aResi, 1);
		}
	}
}