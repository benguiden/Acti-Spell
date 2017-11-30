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
	AudioSource part5;
	int level;

	public static bool endGame;

	private bool[] isHeard;

	// Use this for initialization
	void Start () {
		endGame = false;
		music = gameObject.GetComponents<AudioSource> ();
		part1 = music [0];
		part2 = music [1];
		part3 = music [2];
		part4 = music [3];
		part5 = music [4];
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
		if (!endGame) {
			level = Score.main.level;
			if (Score.main.level >= Score.main.levelCap)
				level = 3;
			level = level % 4;

			if ((level >= 0) && (!isHeard [1])) {
				part2.clip = musicParts [1];
				StartCoroutine (RaiseVolume (1));
				isHeard [1] = true;
			} else if ((level <= 0) && (isHeard [1])) {
				StartCoroutine (LowerVolume (1));
				isHeard [1] = false;
			}

			if ((level > 0) && (!isHeard [2])) {
				part3.clip = musicParts [2];
				StartCoroutine (RaiseVolume (2));
				isHeard [2] = true;
			} else if ((level <= 1) &&  (isHeard [2])) {
				StartCoroutine (LowerVolume (2));
				isHeard [2] = false;
			}

			if ((level > 1) && (!isHeard [3])) {
				part4.clip = musicParts [3];
				StartCoroutine (RaiseVolume (3));
				isHeard [3] = true;
			} else if ((level <= 2) && (isHeard [3])) {
				StartCoroutine (LowerVolume (3));
				isHeard [3] = false;
			}
		}
		else if (endGame) {
			if (part1.isPlaying) {
				part1.Stop ();
			}
			if (part2.isPlaying) {
				part2.Stop ();
			}
			if (part3.isPlaying) {
				part3.Stop ();
			}
			if (part4.isPlaying) {
				part4.Stop ();
			}
			if (!part5.isPlaying) {
				part5.Play ();
			}
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

	private IEnumerator LowerVolume(int track){

		float time = fadeTime;
		while (time > 0f) {

			music [track].volume = 0.7f * time / fadeTime;

			time -= Time.deltaTime;
			yield return null;
		}

		music [track].volume = 0f;
	}

}
