﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public GameObject startButton;
	public GameObject creditsButton;
	public GameObject backButton;
	public GameObject quitButton;

	AudioSource audioSource;
	public AudioClip EnterClick;
	public AudioClip BackClick;
	public AudioClip Mute;
	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startGame() {
		audioSource.PlayOneShot (EnterClick);
		//StartCoroutine (LoadSceneAsync (1));
		SceneManager.LoadScene (1);
	}

	private IEnumerator LoadSceneAsync(int sceneIndex){
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneIndex);
		while (!operation.isDone) {
			Debug.Log (operation.progress / 0.9f);
			yield return null;
		}
	}

	public void home() {
		audioSource.PlayOneShot (BackClick);
		SceneManager.LoadScene (0);
	}

	public void credits() {
		audioSource.PlayOneShot (EnterClick);
			startButton.SetActive (false);
			backButton.SetActive (true);
			quitButton.SetActive (false);
			creditsButton.SetActive (false);
	}

	public void back() {
		audioSource.PlayOneShot (BackClick);
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
