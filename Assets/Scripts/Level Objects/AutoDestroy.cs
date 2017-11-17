using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

	public float delay;

	private float time = 0f;

	private void OnValidate(){
		if (delay < 0f)
			delay = 0f;
	}

	private void Update(){
		if (time < delay)
			time += Time.deltaTime;
		else {
			this.gameObject.SetActive (false);
			LevelController.main.DestroyLater (this.gameObject);
		}
			
					
	}

}
