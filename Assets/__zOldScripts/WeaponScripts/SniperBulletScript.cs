using UnityEngine;
using System.Collections;

public class SniperBulletScript : MonoBehaviour {

	public float movespeed = 10;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.up * movespeed * Time.deltaTime);
	}
}
