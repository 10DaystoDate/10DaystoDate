using UnityEngine;
using System.Collections;

public class MirrorBulletScript : MonoBehaviour {

	public GameObject shieldHit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		other.gameObject.tag = this.gameObject.tag;
		other.GetComponent<Rigidbody2D>().velocity = (other.GetComponent<Rigidbody2D>().velocity*-1.6f);
		GameObject aShot = Instantiate(shieldHit, transform.position, transform.rotation) as GameObject;
		aShot.transform.parent = GameObject.Find ("MainController").transform;
		Destroy (aShot, 1);
	}
}
