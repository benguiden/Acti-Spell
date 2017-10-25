using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	private static LevelController _main;

	public static LevelController main{
		get{
			if (_main == null) {
				GameObject levelObj = GameObject.FindGameObjectWithTag ("MainLevelController");
				if ((levelObj == null) && (Application.isPlaying)) {
					//Debug.LogWarning ("Attention: No Game Object with Level Controller component with 'MainLevelController' tag. Returning null.");
				} else {
					_main = levelObj.GetComponent<LevelController> ();
					//if ((_main == null) && (Application.isPlaying))
						//Debug.LogWarning ("Attention: No Game Object with Level Controller component with 'MainLevelController' tag. Returning null.");
				}
			}
			return _main;
		}
	}
		
	#region Playforms
	private List<Platform> platforms = new List<Platform>();

	public void AddPlatformToList(Platform platform){
		platforms.Add (platform);
	}

	public void RemovePlatformFromList(int index){
		platforms.RemoveAt (index);
		for (int i = index; i < platforms.Count; i++) {
			platforms [i].platformIndex--;
		}
	}

	public Platform[] GetPlatforms(){
		return platforms.ToArray ();
	}

	public Platform GetPlatform(int index){
		return platforms [index];
	}

	public int GetPlatformCount(){
		return platforms.Count;
	}
	#endregion

	#region Bubbles
	private List<Bubble> bubbles = new List<Bubble>();

	public void AddBubbleToList(Bubble bubble){
		bubbles.Add (bubble);
	}

	public void RemoveBubbleFromList(int index){
		bubbles.RemoveAt (index);
		for (int i = index; i < bubbles.Count; i++) {
			bubbles [i].bubbleIndex = i;
		}
	}

	public Bubble[] GetBubbles(){
		return bubbles.ToArray ();
	}

	public Bubble GetBubble(int index){
		return bubbles [index];
	}

	public int GetBubbleCount(){
		return bubbles.Count;
	}

	public void RemoveBubbles(){
		foreach (Bubble bubble in bubbles) {
			Animator bubAnim = bubble.GetComponent<Animator> ();
			bubAnim.SetTrigger ("collected");
			Destroy (bubble.gameObject, 0.5f);
		}
	}
	#endregion

}
