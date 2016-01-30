using UnityEngine;
using System.Collections;
//Extends Gun
public class SniperGunScript : Gun {

	public Rigidbody2D aShotBlast;
	public float destructTimer = 1;
	//public float aBlastSpeed = 30;
	
	private GameObject aTemp;
	private LineRenderer lineRend;
	private GameObject aShotTemp;

	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
	}
	
	void Fire (int tagNum) {
		if(aTemp == false){
			if (Time.time > (playerCtrl.lastFireTime + playerCtrl.weaponDelay)) {
				playerCtrl.lastFireTime = Time.time;
				Rigidbody2D aShot;
				aShot = Instantiate(gunBullet, playerShip.transform.position , playerShip.transform.rotation) as Rigidbody2D;

				aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				//aShot.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
				//aShot.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
				aShot.transform.parent = playerShip.transform;
				//playerCtrl.ChangeSpeed(5, 0, 1f, true);
				aShot.tag = tagNum.ToString();
				aTemp = aShot.gameObject;
				GetComponent<AudioSource>().PlayOneShot(gunSound, volume); // Play the AudioClip
			}
		} else {
			Rigidbody2D aShotBlastInstance = Instantiate (aShotBlast, aTemp.transform.position, aTemp.transform.rotation) as Rigidbody2D;
			aShotBlastInstance.GetComponent<SpriteRenderer> ().color = playerCtrl.shipColor;
			aShotBlastInstance.GetComponent<ParticleSystem> ().startColor = playerCtrl.shipColor;
			aShotBlastInstance.transform.parent = playerCtrl.bulletContainer.transform;
			aShotBlastInstance.tag = tagNum.ToString();
			playerCtrl.ChangeSpeed(10, 5, 0f, true);
			GetComponent<AudioSource>().PlayOneShot(gunSound2, volume); // Play the AudioClip
			aShotTemp = aShotBlastInstance.gameObject;
			Destroy (aTemp);
			SpawnLightning ();
			Destroy (aShotBlastInstance.gameObject, destructTimer);
		}
	}

	void SpawnLightning () {
		lineRend = aShotTemp.GetComponent<LineRenderer> ();
		lineRend.SetVertexCount (2);
		lineRend.SetPosition (0, aShotTemp.transform.position);
		lineRend.SetPosition (1, playerShip.transform.position);
		lineRend.SetColors (playerCtrl.shipColor - new Color(0,0,0,1), playerCtrl.shipColor - new Color(0,0,0,0.5f));
	}
}