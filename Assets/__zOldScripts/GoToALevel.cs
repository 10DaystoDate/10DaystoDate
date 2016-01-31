using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToALevel : MonoBehaviour {

	public int levelNum;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void GoToTheLevel(int levelNum){
		SceneManager.LoadScene(levelNum);
	}
}
