using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

	public float destructTimer = 0.5f;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, destructTimer);
	}
}
