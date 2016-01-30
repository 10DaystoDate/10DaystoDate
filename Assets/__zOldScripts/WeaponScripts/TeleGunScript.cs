using UnityEngine;
using System.Collections;
//Extends Gun
public class TeleGunScript : Gun {

	private GameObject teleTemp;
	public GameObject teleOut;
	public GameObject teleIn;

	public GameObject aShotFX;

	private LineRenderer lineRend;
	// Update is called once per frame
	void Update () {
		
	}
	
	void Fire (int tagNum) {
		if(teleTemp == false){
			if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
				playerCtrl.lastFireTime = Time.time;
				Rigidbody2D aShot = Instantiate(gunBullet, playerShip.transform.position, playerShip.transform.rotation) as Rigidbody2D;
				aShot.velocity = transform.TransformDirection (Vector3.up * gunShotSpeed);
				aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShot.tag = tagNum.ToString();
				aShot.transform.parent = playerCtrl.bulletContainer.transform;
				teleTemp = aShot.gameObject;
				GetComponent<AudioSource>().PlayOneShot(gunSound2);

				playerCtrl.ChangeSpeed (3, 3, 0.1f, true);

				GameObject aShotFXI = Instantiate (aShotFX, playerShip.transform.position + transform.up*1.2f, playerShip.transform.rotation * Quaternion.Euler (-90, 0, 0)) as GameObject;
				aShotFXI.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShotFXI.transform.parent = playerCtrl.bulletContainer.transform;
				Destroy (aShotFXI, 1);
			}
		} else {
			if (teleTemp == true){
				GameObject teleOutFX = Instantiate(teleOut, playerShip.transform.position, playerShip.transform.rotation) as GameObject;
				teleOutFX.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				GameObject teleInFX = Instantiate(teleIn, teleTemp.transform.position, teleTemp.transform.rotation) as GameObject;
				teleInFX.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				SpawnLightning ();
				transform.parent.position = teleTemp.transform.position;
				GetComponent<AudioSource>().PlayOneShot(gunSound,0.5f);
				Destroy (teleTemp);
				Destroy (teleOutFX,1);
				Destroy (teleInFX,1);
			}
		}
	}

	void SpawnLightning () {
		lineRend = this.GetComponent<LineRenderer> ();
		lineRend.SetVertexCount (2);
		lineRend.SetPosition (0, playerShip.transform.position);
		lineRend.SetPosition (1, teleTemp.transform.position);
		lineRend.SetColors (playerCtrl.shipColor - new Color(0,0,0,1), playerCtrl.shipColor - new Color(0,0,0,0.5f));
		Invoke ("ResetLine", 0.1f);
	}

	void ResetLine () {
		lineRend.SetVertexCount (0);
	}
}
