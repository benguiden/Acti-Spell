using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOffset : MonoBehaviour {

	public Transform reference;

	public float offsetAmount;

	private Material gridMat;

	private void Start(){
		gridMat = this.GetComponent<Renderer> ().material;
	}

	private void Update(){
		gridMat.mainTextureOffset = new Vector2 (0f, reference.position.y * offsetAmount);
	}

}
