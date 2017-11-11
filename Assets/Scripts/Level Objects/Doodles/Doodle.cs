using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doodle : MonoBehaviour {

	public bool specifyYPosition = false;

	public float yPosition;

	[HideInInspector]
	public Doodles parent;

	private GameObject graphic;

	private float k;

	private void Start(){
		//Set Parent
		if (parent == null) {
			parent = this.transform.parent.gameObject.GetComponent<Doodles> ();
			if (parent == null)
				Debug.LogError("Error: No parent for Doodle with Doodles componenet.");
		}

		//Make Graphic
		graphic = new GameObject("Graphic");
		graphic.transform.parent = this.transform;

		//Random Sprite
		graphic.AddComponent<SpriteRenderer> ().sprite = parent.GetSprite (true);

		//Random Scale
		float newScale = Random.Range(parent.scale.x, parent.scale.y);
		graphic.transform.localScale = new Vector3 (newScale, newScale, 0f);

		//Random Rotation
		float newRot = Random.Range(-parent.rotationAmount, parent.rotationAmount);
		graphic.transform.localEulerAngles = new Vector3 (0f, 0f, newRot);

		//Set Position
		k = parent.parallaxAmount;
		Vector3 thisPos = this.transform.parent.position;
		thisPos.x = Random.Range (-parent.spawnWidth, parent.spawnWidth);
		if (specifyYPosition)
			thisPos.y = yPosition;
		else
			thisPos.y = Camera.main.transform.position.y + parent.spawnOffset - Random.Range (0f, 2f);
		this.transform.position = thisPos;

		//Set Graphic Position
		float distance = this.transform.position.y - parent.reference.position.y;
		Vector3 newPos = graphic.transform.position;
		newPos.y = parent.reference.position.y + (distance / k);
		newPos.x = 0f;
		newPos.z = 0f;
		graphic.transform.localPosition = newPos;

	}

	private void Update(){
		//Set Y Position
		float distance = this.transform.position.y - parent.reference.position.y;
		Vector3 newPos = graphic.transform.position;
		newPos.y = parent.reference.position.y + (distance / k);
		graphic.transform.position = newPos;

		if ((distance <= parent.despawnOffset) && (gameObject.activeSelf)) {
			gameObject.SetActive (false);
			LevelController.main.DestroyLater (this.gameObject);
		}

	}

}
