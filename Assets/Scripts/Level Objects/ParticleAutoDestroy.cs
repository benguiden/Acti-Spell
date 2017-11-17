using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAutoDestroy : MonoBehaviour {

	private ParticleSystem partSys;

	private void Awake(){
		partSys = this.GetComponent<ParticleSystem> ();
	}

	private void Update(){
		if (!partSys.IsAlive()){
			this.gameObject.SetActive (false);
			LevelController.main.DestroyLater (this.gameObject);
		}
	}

}
