using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

	#region Public Variables
	[Tooltip("If true, this instance of the class will become the static 'main' object, which other classes can reference.")]
	public bool isMain;

	[Tooltip("The transfrom reference that score will increase to.")]
	public Transform reference;

	[Tooltip("The constant that will determine the rate at which score increase by in terms of the references height.")]
	public float scoreModifier = 1f;

	[Tooltip("The Text Mesh that the score will be displayed with.")]
	public TextMesh scoreText;

	[Tooltip("The Tranform of the ui element that transforms in reference to the score.")]
	public Transform uiTransform;

	[Tooltip("The amount that the ui translates by on the Y-Axis at the full score.")]
	public float uiFullScoreOffset;

	[Tooltip("The score at which the ui will be at it's highest position.")]
	public float[] uiFullScore;

	[Tooltip("The different word multipliers with their word count amount.")]
	public WordMultiplier[] wordMultipliers;

	public Doodles doodles;

	public static Score main;

	[HideInInspector]
	public int level;

	[HideInInspector]
	public bool nextLevelReady;
	#endregion

	#region Private Variables
	private float score;
	private float highestPoint;
	private float uiOriginalYPos;
	private int wordMultiplierIndex;
	private int correctWordCount;
	private int totalCorrectSpelling;
	#endregion

	[System.Serializable]
	public struct WordMultiplier{
		public float multiplier;
		public int correctWordCount; //This being the amount of correct words **AFTER THE LAST WORDMULTIPLIER LEVEL**
	}

	#region Mono Methods
	private void Awake(){
		if (isMain)
			main = this;
	}

	private void Start(){
		highestPoint = reference.position.y;
		score = 0f;
		uiOriginalYPos = uiTransform.localPosition.y;
		wordMultiplierIndex = 0;
		correctWordCount = 0;
		level = 0;
		totalCorrectSpelling = 0;

		if (wordMultipliers.Length <= 0){
			Debug.LogError("Error: No Word Multipliers set up in Score componenet.");
			this.enabled = false;
		}
	}

	private void Update(){
		if (reference.position.y > highestPoint) {
			IncreaseScore ();
			highestPoint = reference.position.y;
			SetScoreText ();
			SetUIPosition ();
		}
	}
	#endregion

	private void SetScoreText(){
		scoreText.text = ((int)Mathf.Round (score)).ToString () + " X" + wordMultipliers [wordMultiplierIndex].multiplier.ToString ();
	}

	private void IncreaseScore(){
		float amount = (reference.position.y - highestPoint) * scoreModifier;
		amount *= wordMultipliers [wordMultiplierIndex].multiplier;
		score += amount;
	}

	private void SetUIPosition(){
		Vector3 newPosition = uiTransform.localPosition;
		float wrappedScore = highestPoint;
		for (int i = level - 1; i >= 0; i--) {
			wrappedScore -= uiFullScore [i];
		}

		if (wrappedScore >= uiFullScore [level]) {
			if (level < uiFullScore.Length - 1) {
				level++;
				if (doodles != null)
					doodles.IncreaseIndex ();
				print ("Level: " + level);
				nextLevelReady = true;
			}
		}
		newPosition.y = uiOriginalYPos + ((uiFullScoreOffset * ((float)wrappedScore / uiFullScore [level])) % uiFullScoreOffset);
		uiTransform.localPosition = newPosition;
	}

	public float GetScore(){
		return score;
	}

	public float GetHeight(){
		return highestPoint;
	}

	public float GetTotalWords(){
		return totalCorrectSpelling;
	}

	public void RestartWordMultiplier(){
		wordMultiplierIndex = 0;
	}

	public void CheckWordCount(bool increase){
		if (increase) {
			correctWordCount++;
			totalCorrectSpelling++;
		}
		if (correctWordCount >= wordMultipliers [wordMultiplierIndex].correctWordCount) {
			ToNextWordMultiplier ();
		}
	}

	private void ToNextWordMultiplier(){
		if (wordMultiplierIndex < wordMultipliers.Length - 1) {
			wordMultiplierIndex++;
			SetScoreText ();
			correctWordCount = 0;
		}
	}

}
