using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow: MonoBehaviour {

	public Transform target;

	[Range(0f, 1f)]
	public float smoothness;

	public float failOffset;

	public PlayerController playerController;

	public GameObject endGameObject;

	public AnimationCurve endGameCurve;

	public float endGameCurveTime;

	public AudioClip fallingClip;

	private float yOffset;

	private float targetY;

	private float highestY = float.MinValue;

	private bool follow = true;

	void Start(){
		targetY = this.transform.position.y;
		if (target != null)
			yOffset = this.transform.position.y - target.position.y;
		else
			Debug.LogError ("Error: No target selected in Camera Follow componenent.");
	}

	void Update () {
		if ((target != null) && (follow)) {
			targetY = target.position.y;

			if (target.position.y > highestY)
				highestY = target.position.y;


			//Check Fail
			if (target.position.y < highestY + failOffset) {
				Score.main.lives--;
				if (Score.main.lives == 0) {
					Score.main.livesText.text =	"";
					follow = false;
					target.gameObject.SetActive (false);
					target = null;
					StartCoroutine (EndGameUI ());
				} else {
					Score.main.livesText.text =	Score.main.lives.ToString ();
					playerController.Respawn ();
				}
			}
		}

		Vector3 pos = this.transform.position;
		pos.y = targetY + yOffset;
		Vector3 newPos = new Vector3();
		if (playerController.IsGrounded ())
			newPos = Vector3.Lerp (this.transform.position, pos, 1f - smoothness);
		else
			newPos = Vector3.Lerp (this.transform.position, pos, Mathf.Pow (1f - smoothness, 1.15f));
		if (newPos.y > transform.position.y) {
			transform.position = newPos;
		} else {
			transform.position = newPos;
		}
	}

	private IEnumerator EndGameUI(){

		AudioSource fallingAudio = this.gameObject.AddComponent<AudioSource> ();
		fallingAudio.clip = fallingClip;
		fallingAudio.Play ();

		//Wait
		float time = 0.5f;
		while (time > 0f) {
			time -= Time.deltaTime;
			yield return null;
		}

		endGameObject.SetActive (true);

		//Write Score
		endGameObject.transform.Find ("Score Amount").GetComponent<Text> ().text = Mathf.Round (Score.main.GetScore ()).ToString ();

		//Write Height
		endGameObject.transform.Find ("Height Amount").GetComponent<Text> ().text = Mathf.Round (Score.main.GetHeight()).ToString ();

		//Correct Word Count
		endGameObject.transform.Find ("Correct Word Amount").GetComponent<Text> ().text = Score.main.GetTotalWords().ToString ();

		time = endGameCurveTime;
		float scale;
		while (time > 0f) {
			scale = endGameCurve.Evaluate (1f - (time / endGameCurveTime));
			endGameObject.transform.localScale = new Vector3 (scale, scale, 1f);
			time -= Time.deltaTime;
			yield return null;
		}
		endGameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
		GameObject mainMusic = GameObject.FindWithTag("Music");
		if (mainMusic != null)
			mainMusic.SetActive (false);
	}
}
