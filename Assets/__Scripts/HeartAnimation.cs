using UnityEngine;
using System.Collections;

public class HeartAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void ActivateHeart () {
		foreach (Transform child in this.transform) {
			child.gameObject.SetActive (true);
		}
		Invoke ("DeactivateHeart", 1.5f);
	}

	void DeactivateHeart () {
		foreach (Transform child in this.transform) {
			child.gameObject.SetActive (false);
		}
	}
}
