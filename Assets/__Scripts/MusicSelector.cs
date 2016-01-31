using UnityEngine;
using System.Collections;

public class MusicSelector : MonoBehaviour {
	public AudioClip[] bgMusic; 

	// Use this for initialization
	void Start () {

		GetComponent<AudioSource>().clip = bgMusic[Random.Range(0,bgMusic.Length)];
		GetComponent<AudioSource>().Play();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
