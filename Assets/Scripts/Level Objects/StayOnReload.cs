using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayOnReload : MonoBehaviour {

	private static StayOnReload instance = null;
	public static StayOnReload Instance {
		get {return instance;}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Awake() {
		if (instance != null && instance != this){
			Destroy(this.gameObject);
		}
		else {
			instance = this;
		}

		DontDestroyOnLoad(this.gameObject);
	}
}
