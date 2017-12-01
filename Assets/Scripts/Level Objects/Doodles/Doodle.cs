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

	private bool isAnimated = false;
	private Sprite[] animatedList;
	private float animateFPS = 15f;
	private float animatedIndex = 0f;
	private SpriteRenderer sprRen;

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
		sprRen = graphic.AddComponent<SpriteRenderer> ();

		if (Random.value <= 0.15f) {
			//Animated Sprite
			Doodles.AnimatedDoodle animatedDoodlesData = parent.GetAnimated();
			animatedList = animatedDoodlesData.sprites;
			animateFPS = animatedDoodlesData.fps;
			animatedIndex = (float)Random.Range (0, animatedList.Length);
			sprRen.sprite = animatedList [(int)animatedIndex];
			isAnimated = true;
		} else {
			//Random Sprite
			sprRen.sprite = parent.GetSprite (true);
		}

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

		//Test Position Against Other Doodles
		Vector2[] previousPositions = parent.GetDoodlePositions();
		Vector2[] previousSizes = parent.GetDoodleSizes ();
		Vector2 size = (Vector2)sprRen.bounds.extents;

		bool noCollide = false;
		while (!noCollide) {
			noCollide = true;
			for (int i = 0; i < Mathf.Min (previousPositions.Length, previousSizes.Length); i++) {
				if (Mathf.Abs (thisPos.y - previousPositions [i].y) < size.y + previousSizes [i].y) {
					if (Mathf.Abs (thisPos.x - previousPositions [i].x) < size.x + previousSizes [i].x) {
						noCollide = false;
						thisPos.y = previousPositions [i].y + (size.y * 2f) + (previousSizes [i].y * 2f) + Random.Range (0f, 0.5f);
					}
				}
			}
		}


		parent.AddDoodlePosition ((Vector2)thisPos);
		parent.AddDoodleSize (size);
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

		//Animated
		if (isAnimated) {
			animatedIndex += Time.deltaTime * animateFPS;
			animatedIndex = animatedIndex % animatedList.Length;
			int sprIndex = (int)Mathf.Floor (animatedIndex);
			if (animatedList [sprIndex] != sprRen.sprite) {
				sprRen.sprite = animatedList [sprIndex];
			}
		}

	}

}
