using UnityEngine;
using System.Collections;

public class StarScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.name != "PlayerShip(Clone)") {
			//Destroy (other.gameObject);
			//Debug.Log (other);
		}
	}
}
