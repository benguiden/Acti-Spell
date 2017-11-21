using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayControls : MonoBehaviour {

	public GameObject[] pages;

	public void ChangeToPage(int pageIndex){
		for (int i = 0; i < pages.Length; i++) {
			if (i == pageIndex)
				pages [i].SetActive (true);
			else
				pages [i].SetActive (false);
		}
	}

}
