using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectUtility : MonoBehaviour {

	public float desiredRatio = 1.77777778f;
	public float scaleConstant = 5;

	private int lastWidth = 0;
	private int lastHeight = 0;

	private void Awake(){
		CheckRatio ();
	}

	private void Update(){
		if ((lastWidth != Screen.width) || (lastHeight != Screen.height)) {
			lastWidth = Screen.width;
			lastHeight = Screen.height;
			CheckRatio ();
		}
	}

	private void CheckRatio(){
		float scaleAmount = desiredRatio / ((float)Screen.width / (float)Screen.height);
		Camera.main.orthographicSize = scaleConstant * scaleAmount;
	}

}
