using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebuggingStats : MonoBehaviour {

	public bool FPSCounter;

	private float lowestFPS, averageFPSSum, averageFPSCount, highestFPS;

	#if UNITY_EDITOR
	private void Awake(){
		lowestFPS = float.MaxValue;
		highestFPS = float.MinValue;
	}

	private void Update(){
		if (FPSCounter) {
			float fps = 1f / Time.deltaTime;
			averageFPSSum += fps;
			averageFPSCount++;
			if (fps < lowestFPS)
				lowestFPS = fps;
			if (fps > highestFPS)
				highestFPS = fps;
		}
	}

	private void OnApplicationQuit(){
		if (FPSCounter) {
			float averageFPS = averageFPSSum / averageFPSCount;
			string output = "Average FPS: " + (Mathf.Round (averageFPS * 10f) / 10f).ToString ();
			output += "   Lowest FPS: " + (Mathf.Round (lowestFPS * 10f) / 10f).ToString ();
			output += "   Highest FPS: " + (Mathf.Round (highestFPS * 10f) / 10f).ToString ();
			Debug.LogWarning (output);
		}
	}
	#endif



}
