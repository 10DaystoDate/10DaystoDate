using UnityEngine;
using System.Collections;

public class SetAudioVolume : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat ("SndVol");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
