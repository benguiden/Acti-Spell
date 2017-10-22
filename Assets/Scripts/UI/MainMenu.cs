using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public GameObject startButton;
	public GameObject creditsButton;
	public GameObject backButton;
	public GameObject quitButton;

	AudioSource audio;
	public AudioClip EnterClick;
	public AudioClip BackClick;
	public AudioClip Mute;
	// Use this for initialization
	void Start () {
		audio = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startGame() {
		audio.PlayOneShot (EnterClick);
			EditorSceneManager.LoadScene (0);
	}

	public void home() {
		audio.PlayOneShot (BackClick);
		EditorSceneManager.LoadScene (1);
	}

	public void credits() {
		audio.PlayOneShot (EnterClick);
			startButton.SetActive (false);
			backButton.SetActive (true);
			quitButton.SetActive (false);
			creditsButton.SetActive (false);
	}

	public void back() {
		audio.PlayOneShot (BackClick);
			startButton.SetActive (true);
			creditsButton.SetActive (true);
			quitButton.SetActive (true);
			backButton.SetActive (false);
	}

	public void quit() {
		Application.Quit();
		print ("Quit");
	}

}
