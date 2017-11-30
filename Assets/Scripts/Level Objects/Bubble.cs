using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

	public char letter = ' ';

	public float radius = 0.5f;

	public float jiggleAmount = 0.075f;
	public float jiggleSpeed = 0.05f;
	public float rotateAmount = 3f;
	public float rotatespeed = 8f;
	Vector3 pos;

	private TextMesh textMesh;

	[HideInInspector]
	public int bubbleIndex;

	public void Initalize(){
		if (this.gameObject.tag == "Bubble") {
			bubbleIndex = LevelController.main.GetBubbleCount ();
			LevelController.main.AddBubbleToList (this);
		} else {
			Debug.LogWarning ("Warning: Instance of Bubble with no 'Bubble' tag.");
		}
		letter = SpellingManager.main.GrabRandomLetter ();
		if (GetText ())
			textMesh.text = letter.ToString ();

	}
	void Start(){
		pos = this.gameObject.transform.position;

		jiggleAmount = 0.075f + Random.Range(0f, 0.025f);
		jiggleSpeed = 0.05f + Random.Range(0f, 0.075f);
		rotateAmount = 3f + Random.Range(0f, 4f);
		rotatespeed = 8f+ Random.Range(0f, 4f);
	}
	void Update() {
		
		Vector3 newPos = pos;
		newPos.x += Mathf.PingPong (Time.time * jiggleSpeed, jiggleAmount);
		transform.localEulerAngles = new Vector3 (0, 0, rotateAmount - Mathf.PingPong (Time.time * rotatespeed, rotateAmount * 2f));
		transform.position = newPos;

	}

	private void OnDestroy(){
		if (this.gameObject.tag == "Bubble") {
			LevelController.main.RemoveBubbleFromList (bubbleIndex);
		}
	}

	private bool GetText(){
		if (textMesh == null) {
			textMesh = this.GetComponentInChildren<TextMesh> ();
			if (textMesh == null) {
				Debug.LogError ("Error: No TextMesh componenet in child Game Object of '" + this.gameObject.name + "' for Bubble component.");
				return false;
			}
			return true;
		}
		return true;
	}

}
