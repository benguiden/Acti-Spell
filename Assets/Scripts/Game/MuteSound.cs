using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void mute() {
		AudioListener.volume = 0f;
	}
	public void unmute() {
		AudioListener.volume = 1f;
	}
}
