using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayControls : MonoBehaviour {

	public GameObject[] pages;

	private AudioSource audioSource;

	private void Awake(){
		audioSource = GetComponent<AudioSource> ();
	}

	public void ChangeToPage(int pageIndex){
		for (int i = 0; i < pages.Length; i++) {
			if (i == pageIndex)
				pages [i].SetActive (true);
			else
				pages [i].SetActive (false);
		}
		if ((audioSource != null) && (gameObject.activeSelf)) {
			audioSource.time = 0f;
			audioSource.Play ();
		}
	}

}
