using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebuggingStats : MonoBehaviour {

	public bool FPSCounter;

	public GameObject DebugUI;

	public Text logText;

	private float lowestFPS, averageFPSSum, averageFPSCount, highestFPS;

	private int mostPlatforms = 0;

	private int mostBubbles = 0;

	private void Awake(){
		lowestFPS = float.MaxValue;
		highestFPS = float.MinValue;
		if (DebugUI != null)
			DebugUI.SetActive (true);
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
		if (LevelController.main.GetPlatformCount () > mostPlatforms)
			mostPlatforms = LevelController.main.GetPlatformCount ();
		if (LevelController.main.GetBubbleCount () > mostBubbles)
			mostBubbles = LevelController.main.GetBubbleCount ();
	}

	#if UNITY_EDITOR
	private void OnApplicationQuit(){
		LogToConsole ();
	}

	private void LogToConsole(){
		if (FPSCounter) {
			float averageFPS = averageFPSSum / averageFPSCount;
			string output = "Average FPS: " + (Mathf.Round (averageFPS * 10f) / 10f).ToString ();
			output += "   Lowest FPS: " + (Mathf.Round (lowestFPS * 10f) / 10f).ToString ();
			output += "   Highest FPS: " + (Mathf.Round (highestFPS * 10f) / 10f).ToString ();
			Debug.LogWarning (output);
		}
		Debug.LogWarning ("Max Platform Count: " + mostPlatforms.ToString () + "\tMax Bubble Count: " + mostBubbles.ToString ());
	}
	#endif

	public void LogToScreen(){
		logText.text = "";
		if (FPSCounter) {
			float averageFPS = averageFPSSum / averageFPSCount;
			string output = "Average FPS: " + (Mathf.Round (averageFPS * 10f) / 10f).ToString ();
			output += "   Lowest FPS: " + (Mathf.Round (lowestFPS * 10f) / 10f).ToString ();
			output += "   Highest FPS: " + (Mathf.Round (highestFPS * 10f) / 10f).ToString ();
			logText.text = output + "\n";
		}
		logText.text += "Max Platform Count: " + mostPlatforms.ToString () + "\tMax Bubble Count: " + mostBubbles.ToString ();
	}


}
