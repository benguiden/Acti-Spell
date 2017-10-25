using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteSound : MonoBehaviour {

	public GameObject muteButton;
	public GameObject unMuteButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (AudioListener.volume < 1) {
			muteButton.SetActive (false);
			unMuteButton.SetActive (true);
		}
		if (AudioListener.volume > 0) {
			muteButton.SetActive (true);
			unMuteButton.SetActive (false);
		}
	}

	public void mute() {
		AudioListener.volume = 0f;
	}
	public void unmute() {
		AudioListener.volume = 1f;
	}
}
