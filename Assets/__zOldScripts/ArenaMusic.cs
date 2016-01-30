using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaMusic : MonoBehaviour {

	public List<AudioClip> arenaBGPerm = new List<AudioClip>();
	public List<AudioClip> arenaBG = new List<AudioClip>();
	public AudioSource mAudio;

	// Use this for initialization
	void Start () {
		mAudio = GetComponent<AudioSource> (); //get audiosource component

		if (PlayerPrefs.HasKey ("MusVol")) { //if music vol was set
			mAudio.volume = PlayerPrefs.GetFloat ("MusVol"); //set volume of audiosource
		} else {
			mAudio.volume = 1;
		}

		mAudio.clip = arenaBG [3];
		arenaBG.RemoveAt (3); //Remove from list
		mAudio.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!mAudio.isPlaying) {
			if (arenaBG.Count == 0) {
				for (int i = 0; i < arenaBGPerm.Count; i++) { //Repopulate list
					arenaBG.Add (arenaBGPerm [i]);
				}
			} else {
				PlaySong ();
			}
		}
	}

	void PlaySong () {
		int musicNum = Random.Range (0, arenaBG.Count);
		mAudio.clip = arenaBG [musicNum];
		arenaBG.RemoveAt (musicNum); //Remove from list
		mAudio.Play ();
	}
}
