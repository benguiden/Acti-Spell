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
	}
	
	// Update is called once per frame
	void Update () {
		level = Score.main.level;
		if ((level > 0 && level < 4) || (level > 4 && level < 8) || (level > 8)) {
			if (part1.time > part1.clip.length - 0.1f) {
				part2.clip = musicParts [1];
				if (!part2.isPlaying) {
					part2.Play ();
				}
			}
		}
		else {
			if (part2.time > part2.clip.length - 0.1f) {
				part2.Stop ();
			}
		}

		if ((level > 1 && level < 4) || (level > 5 && level < 8) || (level > 9)) {
			if (part1.time > part1.clip.length -0.1f) {
				part3.clip = musicParts [2];
				if (!part3.isPlaying) {
					part3.Play ();
				}
			}
		}
		else {
				if (part3.time > part3.clip.length - 0.1f) {
					part3.Stop ();
				}
		}
		if ((level > 2 && level < 4) || (level > 6 && level < 8) || (level > 10)) {
			if (part1.time > part1.clip.length -0.1f) {
				part4.clip = musicParts [3];
				if (!part4.isPlaying) {
					part4.Play ();
				}
			}
		}
		else {
				if (part4.time > part4.clip.length - 0.1f) {
					part4.Stop ();
				}
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
