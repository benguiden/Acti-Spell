using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	#region Public Variables
	[Tooltip("If true, this instance of the class will become the static 'main' object, which other classes can reference.")]
	public bool isMain;

	public int lives = 3;

	public Text livesText;

	[Tooltip("The transfrom reference that score will increase to.")]
	public Transform reference;

	[Tooltip("The constant that will determine the rate at which score increase by in terms of the references height.")]
	public float scoreModifier = 1f;

	[Tooltip("The Text Mesh that the score will be displayed with.")]
	public TextMesh scoreText;

	[Tooltip("The Tranform of the ui element that transforms in reference to the score.")]
	public Transform uiTransform;

	[Tooltip("The distance between each CM marking on the ruler.")]
	public float rulerCMDistance;

	[Tooltip("The score at which the ui will be at it's highest position.")]
	public float[] uiFullScore;

	[Tooltip("The different word multipliers with their word count amount.")]
	public WordMultiplier[] wordMultipliers;

	public Doodles doodles;

	public static Score main;

	public int levelCap = 11;

	//private bool Random

	[HideInInspector]
	public int level = 0;

	[HideInInspector]
	public bool nextLevelReady;
	#endregion

	#region Private Variables
	private float score = 0f;
	private float highestPoint;
	private float uiOriginalYPos;
	private int wordMultiplierIndex = 0;
	private int correctWordCount = 0;
	private int totalCorrectSpelling = 0;
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
		uiOriginalYPos = uiTransform.localPosition.y;

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

		//Check Level
		if (wrappedScore >= uiFullScore [level]) {
			if ((level < uiFullScore.Length - 1) && (level < levelCap - 1)) {
				level++;
				if (doodles != null)
					doodles.IncreaseIndex ();
				print ("Level: " + level);
				nextLevelReady = true;
			} else if (level == levelCap - 1) {
				Debug.Log ("Capped");
			}
		}

		//Set UI Position
		float uiOffset = 0f;
		wrappedScore = highestPoint;
		for (int i = 0; i <= level; i++) {
			if (wrappedScore > uiFullScore [i]) {
				uiOffset += rulerCMDistance;
				wrappedScore -= uiFullScore [i];
			} else {
				uiOffset += (wrappedScore / uiFullScore [i]) * rulerCMDistance;
				i = level + 1;
			}
		}
		newPosition.y = uiOriginalYPos + uiOffset;
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
