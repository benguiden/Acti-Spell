using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Font3D : MonoBehaviour {

	#region Variables & Objects
	//Visible
	public bool setStatic;
	public Sprite[] _letterSprites;
	public float _letterScale;
	public float _letterGap;
	public float _k;
	public Transform _reference;
	public Text _debugText;

	//Static
	public static Sprite[] letterSprites;
	public static float letterScale;
	public static float letterGap;
	public static float k;
	public static Transform reference;
	public static Text debugText;

	//Private
	private string text;
	#endregion

	#region Mono Methods
	private void Awake(){
		if (setStatic) {
			letterSprites = _letterSprites;
			if (letterSprites.Length != 26)
				Debug.LogError ("Error: Letter Sprites array length is not 26.");
			letterScale = _letterScale;
			letterGap = _letterGap;
			k = _k;
			reference = _reference;
			debugText = _debugText;
			Destroy (this.gameObject);
		}
	}

	private void Update(){
		//Set Y Position
		float distance = this.transform.parent.position.y - reference.position.y;
		Vector3 newPos = this.transform.position;
		newPos.y = reference.position.y + (distance / k);
		this.transform.position = newPos;
	}
	#endregion

	#region Text Methods
	public void SetText(string newString){
		text = newString;
		//Delete Previous Children
		foreach (Transform child in transform){
			Destroy (child.gameObject);
		}
			
		newString = newString.ToLower ();


		char[] letters = newString.ToCharArray ();

		for (int i = letters.Length - 1; i >= 0; i--) {
			if (((int)letters [i] < 97) || ((int)letters [i] > 124)) {
				Debug.LogError ("Error: Unidentified character in string. Unidentified character: " + letters [i].ToString () + "\n In word: " + newString + ". Please report to Ben!");
				if (debugText != null){
					debugText.text = "Error: Unidentified character in string. Unidentified character: " + letters [i].ToString () + "\n In word: " + newString + ". Please report to Ben!";
				}
				Debug.Break ();
			}
		}


		int[] spriteIndexes = new int[letters.Length];
		float canvasWidth = 0f;
		for (int i = 0; i < letters.Length; i++) {
			spriteIndexes [i] = GetLetterIndex (letters [i]);
			if (i != letters.Length - 1)
				canvasWidth += (letterSprites [spriteIndexes[i]].bounds.size.x + letterGap) * letterScale;
			else
				canvasWidth += letterSprites [spriteIndexes[i]].bounds.size.x * letterScale;
		}

		//Set Letters
		float xPos = ((letterSprites [spriteIndexes [0]].bounds.size.x / 2f) * letterScale) - (canvasWidth / 2f);
		for (int i = 0; i < spriteIndexes.Length; i++) {
			SetLetter (xPos, letterScale, spriteIndexes [i]);
			xPos += (letterSprites [spriteIndexes [i]].bounds.size.x / 2f * letterScale);
			xPos += (letterGap * letterScale);
			if (i < spriteIndexes.Length - 1)
				xPos += (letterSprites [spriteIndexes [i + 1]].bounds.size.x / 2f * letterScale);
		}
	}

	private void SetLetter(float xPos, float scale, int spriteIndex){
		GameObject letter = new GameObject ("Letter");
		letter.transform.parent = this.transform;
		letter.transform.localPosition = new Vector3 (xPos, 0f, 0f);
		letter.transform.localScale = new Vector3 (scale, scale, scale);
		letter.AddComponent<SpriteRenderer> ().sprite = letterSprites [spriteIndex];
	}

	private int GetLetterIndex(char letter){
		return ((int)letter) - 97;
	}

	public string GetText(){
		return text;
	}
	#endregion

}
