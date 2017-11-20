using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioClip[] musicParts;
	public float fadeTime;
	AudioSource[] music;
	AudioSource part1;
	AudioSource part2;
	AudioSource part3;
	AudioSource part4;
	int level;

	private bool[] isHeard;

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

		isHeard = new bool[music.Length];
		isHeard [0] = true;
	}
	
	// Update is called once per frame
	void Update () {
		level = Score.main.level;
		if (((level > 0 && level < 4) || (level > 4 && level < 8) || (level > 8)) && (!isHeard [1])) {
			part2.clip = musicParts [1];
			StartCoroutine (RaiseVolume (1));
			isHeard [1] = true;
		}
		else {
			part2.volume = 0f;
			isHeard [1] = false;
		}

		if (((level > 1 && level < 4) || (level > 5 && level < 8) || (level > 9)) && (!isHeard [2])) {
			part3.clip = musicParts [2];
			StartCoroutine (RaiseVolume (2));
			isHeard [2] = true;
		} else {
			part3.volume = 0f;
			isHeard [2] = false;
		}

		if (((level > 2 && level < 4) || (level > 6 && level < 8) || (level > 10)) && (!isHeard [3])) {
			part4.clip = musicParts [3];
			StartCoroutine (RaiseVolume (3));
			isHeard [3] = true;
		} else {
			part4.volume = 0f;
			isHeard [3] = false;
		}
	}
		

	private IEnumerator RaiseVolume(int track){
		
		float time = 0f;
		while (time < fadeTime) {

			music [track].volume = 0.7f * time / fadeTime;

			time += Time.deltaTime;
			yield return null;
		}

		music [track].volume = 0.7f;
	}

}
