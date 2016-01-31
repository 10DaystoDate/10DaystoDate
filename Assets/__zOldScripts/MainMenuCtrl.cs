using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuCtrl : MonoBehaviour {

	public AudioClip menuBG;
	public AudioSource mAudio;
	public float mVol = 0;
	public float sVol = 0;
	
	Slider musSlider;
	Slider sndSlider;

	// Use this for initialization
	void Awake () {

		GameObject loadingPanel = GameObject.Find ("LoadingPanel");
		loadingPanel.GetComponent<Animator> ().SetBool ("ArenaLiftUp", false);

		mAudio = GetComponent<AudioSource> ();

		
		GameObject mTemp = GameObject.Find("MusSlider"); //find music slider
		GameObject sTemp = GameObject.Find("SndSlider"); //find music slider

		if (mTemp != null) { //if slider was found
			musSlider = mTemp.GetComponent<Slider>();  //set slider to musSlider
		}
		if (sTemp != null) { //if slider was found
			sndSlider = sTemp.GetComponent<Slider>();  //set slider to musSlider
		}
		if (PlayerPrefs.HasKey ("MusVol")) { //if music volume was set
			musSlider.value = PlayerPrefs.GetFloat ("MusVol");
		} else {
			musSlider.value = 0.8f; //if music vol was not set, default to 1
		}

		if (PlayerPrefs.HasKey ("SndVol")) { //if sound volume was set
			sndSlider.value = PlayerPrefs.GetFloat ("SndVol");
		} else {
			sndSlider.value = 1; //if sound vol was not set, default to 1
		}
		
		mAudio.clip = menuBG; //set music clip
		mAudio.Play(); //play music

		//print(Mathf.Round((musSlider.value)*10)/10);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MusicSlider () {
		//print(Mathf.Round((musSlider.value)*10)/10);
		mVol = Mathf.Round((musSlider.value)*10)/10;
		mAudio.volume = mVol;
	}

	public void SndSlider () {
		//print(Mathf.Round((sndSlider.value)*10)/10);
		sVol = Mathf.Round((sndSlider.value)*10)/10;
	}
}
