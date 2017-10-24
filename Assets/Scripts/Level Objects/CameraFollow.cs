using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow: MonoBehaviour {

	public Transform target;

	[Range(0f, 1f)]
	public float smoothness;

	public float failOffset;

	public PlayerController playerController;

	private float yOffset;

	private float targetY;

	private float lowestY;

	private float lastLowestTargetY;

	void Start(){
		targetY = this.transform.position.y;
		if (target != null)
			yOffset = this.transform.position.y - target.position.y;
		else
			Debug.LogError ("Error: No target selected in Camera Follow componenent.");
	}

	void Update () {
		if (target != null) {
			targetY = target.position.y;
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

			//Check Fail
			if (target.position.y < this.transform.position.y + failOffset)
				UnityEngine.SceneManagement.SceneManager.LoadScene (1);
		}
	}
}
