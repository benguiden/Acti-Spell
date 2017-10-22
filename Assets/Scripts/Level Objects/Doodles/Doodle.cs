using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doodle : MonoBehaviour {

	[HideInInspector]
	public float originalYPosition;

	[HideInInspector]
	public Doodles parent;

	private bool started = false;

	private void Start(){
		//Random Sprite
		this.GetComponent<SpriteRenderer> ().sprite = parent.GetSprite (true);
		this.GetComponent<SpriteRenderer> ().enabled = false;

		//Random Scale
		float newScale = Random.Range(parent.scale.x, parent.scale.y);
		this.transform.localScale = new Vector3 (newScale, newScale, 0f);

		//Random Rotation
		float newRot = Random.Range(-parent.rotationAmount, parent.rotationAmount);
		this.transform.localEulerAngles = new Vector3 (0f, 0f, newRot);

		//Set Position
		float newPos = Random.Range(-parent.spawnWidth/2f, parent.spawnWidth/2f);
		Vector3 localPos = this.transform.localPosition;
		this.transform.localPosition = new Vector3 (newPos, parent.spawnOffset + Random.Range(0f, 3f), localPos.z);
		originalYPosition = this.transform.position.y;
	}

	private void Update(){
		//Set Y Position
		float newPos = (originalYPosition - parent.reference.position.y) * parent.offsetAmount;
		Vector3 localPos = this.transform.localPosition;
		this.transform.localPosition = new Vector3 (localPos.x, newPos, localPos.z);
		if (this.transform.localPosition.y <= parent.despawnOffset) {
			parent.SpawnDoodle();
			Destroy (this.gameObject);
		}
		if (started == false) {
			this.GetComponent<SpriteRenderer> ().enabled = true;
			started = true;
		}
	}

}
