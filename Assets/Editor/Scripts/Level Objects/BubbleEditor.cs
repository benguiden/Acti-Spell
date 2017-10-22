using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bubble))]
public class BubbleEditor : Editor {

	Bubble bubble;

	public override void OnInspectorGUI(){
		bubble = (Bubble)target;
		EditorGUI.BeginChangeCheck ();
		string letters = EditorGUILayout.TextField ("Letter", bubble.letter.ToString ()).ToUpper ();
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (target, "Bubble Letter");
			bubble.letter = letters.ToCharArray () [letters.Length - 1];
		}

		EditorGUI.BeginChangeCheck ();
		bubble.radius = EditorGUILayout.FloatField ("Radius", bubble.radius);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (target, "Bubble Radius");
		}

		if (GUI.changed)
			EditorUtility.SetDirty (bubble);
	}

}
