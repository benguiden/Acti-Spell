using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public Vector2 bounds;

	public Vector2 boundsOffset;

	[HideInInspector]
	public int platformIndex;

	private void Start(){
		if (this.gameObject.tag == "Platform") {
			platformIndex = LevelController.main.GetPlatformCount ();
			LevelController.main.AddPlatformToList (this);
		} else {
			Debug.LogWarning ("Warning: Instance of Platform with no 'Platform' tag.");
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

}
