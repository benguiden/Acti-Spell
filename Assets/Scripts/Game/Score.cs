using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	#region Public Variables
	[Tooltip("If true, this instance of the class will become the static 'main' object, which other classes can reference.")]
	public bool isMain;

	public int lives = 3;

	public GameObject heartOne;
	public GameObject heartTwo;
	public GameObject heartThree;

	[Tooltip("The transfrom reference that score will increase to.")]
	public Transform reference;

	[Tooltip("The constant that will determine the rate at which score increase by in terms of the references height.")]
	public float scoreModifier = 1f;

	[Tooltip("The Text Mesh that the score will be displayed with.")]
	public TextMesh scoreText;
	public TextMesh scoreMultiplierText;

	[Tooltip("The Tranform of the ui element that transforms in reference to the score.")]
	public Transform uiTransform;

	[Tooltip("The distance between each CM marking on the ruler.")]
	public float rulerCMDistance;

	[Tooltip("The score at which the ui will be at it's highest position.")]
	public float[] uiFullScore;

	[Tooltip("The different word multipliers with their word count amount.")]
	public WordMultiplier[] wordMultipliers;

	public Doodles doodles;

	public PlatformGeneration platformGen;

	public static Score main;

	public int levelCap = 8;

	[HideInInspector]
	public bool isCapped = false;

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
		if (lives < 3) {
			heartThree.SetActive (false);
		}
		if(lives < 2) {
			heartTwo.SetActive(false);
		}
		if (lives < 1) {
			heartOne.SetActive(false);
		}
	}
	#endregion

	private void SetScoreText(){
		scoreText.text = ((int)Mathf.Round (score)).ToString ();
		scoreMultiplierText.text = wordMultiplierIndex.ToString ();
	}

	private void StretchScore(){
		StartCoroutine (IStretchScore (0.4f, 0.35f));
	}

	private IEnumerator IStretchScore(float amount, float timeToTake){
		float time = 0f;
		Color orgColour = scoreMultiplierText.color;
		while (time < timeToTake) {
			float newScale = 1f + (amount * Mathf.Sin ((time / timeToTake) * Mathf.PI));
			scoreMultiplierText.transform.localScale = new Vector3 (newScale, newScale, 1f);
			scoreMultiplierText.color = Color.Lerp (orgColour, Color.red, Mathf.Sin ((time / timeToTake) * Mathf.PI));

			time += Time.deltaTime;
			yield return null;
		}
		scoreMultiplierText.transform.localScale = new Vector3 (1f, 1f, 1f);
		scoreMultiplierText.color = orgColour;
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
		if ((wrappedScore >= uiFullScore [level]) && (!isCapped)) {
			if ((level < uiFullScore.Length - 1) && (level < levelCap)) {
				level++;
				nextLevelReady = true;
			} else if ((level >= levelCap)) {
				Debug.Log ("Level Capped!");
				uiTransform.gameObject.SetActive (false);
				isCapped = true;
			}
			if (doodles != null)
				doodles.IncreaseIndex ();
			if (platformGen != null)
				platformGen.IncreaseIndex ();
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
		StretchScore ();
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
			StretchScore ();
			correctWordCount = 0;
		}
	}

}
