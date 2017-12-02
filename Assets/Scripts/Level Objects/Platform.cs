using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public Vector2 bounds;

	public Vector2 boundsOffset;

	public AnimationCurve bobAmount;

	public Vector2 squashAmount;

	public float bobTime;

	public bool isGround = false;

	[HideInInspector]
	public int platformIndex;

	//[HideInInspector]
	//public bool isBobbing;

	private float originalY;

	private Coroutine bobCo;

	private float bobResetTime = 0f;

	private Vector3 orgScale;

	private void Start(){
		if (this.gameObject.tag == "Platform") {
			platformIndex = LevelController.main.GetPlatformCount ();
			LevelController.main.AddPlatformToList (this);
		} else {
			Debug.LogWarning ("Warning: Instance of Platform with no 'Platform' tag.");
		}
		originalY = this.transform.localPosition.y;
		orgScale = this.transform.localScale;
	}

	private void Update(){
		if (bobResetTime > 0f) {
			bobResetTime -= Time.deltaTime;
		} else if (bobResetTime < 0f) {
			bobResetTime = 0f;
		}
			
	}

	private void OnDestroy(){
		if (LevelController.main.GetPlatform (platformIndex) != this) {
			Debug.LogError ("Error: Removing wrong platfrom from list.");
		}
		if (this.gameObject.tag == "Platform") {
			LevelController.main.RemovePlatformFromList (platformIndex);
		}
	}

	#if UNITY_EDITOR
	private void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube (this.transform.position + (Vector3)boundsOffset, (Vector3)bounds);
	}
	#endif

	public Vector2 BoundsToPosition(Vector2 boundPos){
		boundPos.x = Mathf.Clamp01 (boundPos.x);
		boundPos.y = Mathf.Clamp01 (boundPos.y);
		float xPosition = this.transform.position.x - (bounds.x / 2f) + (bounds.x * boundPos.x) + boundsOffset.x;
		float yPosition = this.transform.position.y - (bounds.y / 2f) + (bounds.y * boundPos.y) + boundsOffset.y;
		return new Vector2 (xPosition, yPosition);
	}

	public float BoundsXToPosition(float x){
		x = Mathf.Clamp01 (x);
		return this.transform.position.x - (bounds.x / 2f) + (bounds.x * x) + boundsOffset.x;
	}

	public float BoundsYToPosition(float y){
		y = Mathf.Clamp01 (y);
		return this.transform.position.y - (bounds.y / 2f) + (bounds.y * y) + boundsOffset.y;
	}

	public void Bob(){
		if (bobResetTime <= 0f) {
			if (bobCo != null) {
				StopCoroutine (StartBob ());
			}
			bobCo = StartCoroutine (StartBob ());
		}
	}

	private IEnumerator StartBob(){
		bobResetTime = bobTime * 2.5f;
		float time = 0f;
		Vector3 pos = this.transform.localPosition;
		Vector3 scale = this.transform.localScale;
		while (time < bobTime) {
			pos.y = originalY + (bobAmount.Evaluate (time / bobTime));
			this.transform.localPosition = pos;

			float squashTime = Mathf.Sin ((time / bobTime) * Mathf.PI);
			scale.x = orgScale.x + (squashAmount.x * squashTime);
			scale.y = orgScale.y + (squashAmount.y * squashTime);
			this.transform.localScale = scale;

			time += Time.deltaTime;
			yield return null;
		}
		this.transform.localScale = orgScale;
		pos = this.transform.localPosition;
		pos.y = originalY;
		this.transform.localPosition = pos;
	}

}
