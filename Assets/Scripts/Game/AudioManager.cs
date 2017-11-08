using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioClip[] musicParts;
	AudioSource[] music;
	AudioSource part1;
	AudioSource part2;
	AudioSource part3;
	AudioSource part4;
	int level;

	// Use this for initialization
	void Start () {
		music = gameObject.GetComponents<AudioSource> ();
		part1 = music [0];
		part2 = music [1];
		part3 = music [2];
		part4 = music [3];
		level = Score.main.level;
		part1.clip = musicParts[0];
		part1.volume = 0.8f;
		part2.volume = 0f;
		part3.volume = 0f;
		part4.volume = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		level = Score.main.level;
		if ((level > 0 && level < 4) || (level > 4 && level < 8) || (level > 8)) {
				part2.clip = musicParts [1];
				part2.volume = 0.8f;
		}
		else {
			part2.volume = 0f;
		}

		if ((level > 1 && level < 4) || (level > 5 && level < 8) || (level > 9)) {
				part3.clip = musicParts [2];
				part3.volume = 0.8f;
		}
		else {
			part3.volume = 0f;
		}
		if ((level > 2 && level < 4) || (level > 6 && level < 8) || (level > 10)) {
			part4.clip = musicParts [3];
			part4.volume = 0.8f;
		}
		else {
			part4.volume = 0f;
		}
	}

	/*
	void addTrack(){
		part2.clip = musicParts [1];
		if (!part2.isPlaying) {
			part2.Play ();
		}
	}
	*/
}
