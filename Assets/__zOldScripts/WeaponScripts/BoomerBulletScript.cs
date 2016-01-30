using UnityEngine;
using System.Collections;

public class BoomerBulletScript : MonoBehaviour {

	public float movespeed = 10;
	// Use this for initialization

	public GameObject playerShip;
	void Start () {
		//playerShip = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody2D>().velocity = (playerShip.transform.position - this.transform.position).normalized * movespeed;
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.name == "PlayerShip(Clone)") {
			Destroy (gameObject);
		} 
	}
}
