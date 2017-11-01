using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow: MonoBehaviour {

	public Transform target;

	[Range(0f, 1f)]
	public float smoothness;

	public float failOffset;

	public PlayerController playerController;

	public GameObject endGameObject;

	public AnimationCurve endGameCurve;

	public float endGameCurveTime;

	private float yOffset;

	private float targetY;

	private float lowestY;

	private float lastLowestTargetY;

	private bool follow;

	void Start(){
		follow = true;
		targetY = this.transform.position.y;
		if (target != null)
			yOffset = this.transform.position.y - target.position.y;
		else
			Debug.LogError ("Error: No target selected in Camera Follow componenent.");
	}

	void Update () {
		if ((target != null) && (follow)) {
			targetY = target.position.y;

			//Check Fail
			if (target.position.y < this.transform.position.y + failOffset) {
				follow = false;
				target.gameObject.SetActive (false);
				target = null;
				StartCoroutine (EndGameUI ());
			}
		}

		Vector3 pos = this.transform.position;
		pos.y = targetY + yOffset;
		Vector3 newPos = new Vector3();
		if (playerController.IsGrounded ())
			newPos = Vector3.Lerp (this.transform.position, pos, 1f - smoothness);
		else
			newPos = Vector3.Lerp (this.transform.position, pos, Mathf.Pow (1f - smoothness, 1.5f));
		if (newPos.y > transform.position.y) {
			transform.position = newPos;
		} else {
			transform.position = newPos;
		}
	}

	private IEnumerator EndGameUI(){
		endGameObject.SetActive (true);
		float time = endGameCurveTime;
		float scale;
		while (time > 0f) {
			scale = endGameCurve.Evaluate (1f - (time / endGameCurveTime));
			endGameObject.transform.localScale = new Vector3 (scale, scale, 1f);
			time -= Time.deltaTime;
			yield return null;
		}
		endGameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
	}
}
