using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor {

	public override void OnInspectorGUI(){

		PlayerController controller = (PlayerController)target;

		#region Parameters
		EditorGUILayout.LabelField ("Movement", EditorStyles.boldLabel);
		controller.horizontalSpeed = EditorGUILayout.FloatField ("Horizontal Speed", controller.horizontalSpeed); 
		controller.jumpHeight = EditorGUILayout.FloatField ("Jump Height", controller.jumpHeight); 
		controller.jumpTime = EditorGUILayout.FloatField ("Jump Time", controller.jumpTime); 
		controller.wrapWidth = EditorGUILayout.FloatField ("Warp Width", controller.wrapWidth); 

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Physics", EditorStyles.boldLabel);
		EditorGUILayout.LabelField ("Platforms");
		controller.bounds = EditorGUILayout.Vector2Field ("Collision Bounds", controller.bounds);
		controller.boundsOffset = EditorGUILayout.Vector2Field ("Collision Bounds Offset", controller.boundsOffset);
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Bubbles");
		controller.bubbleBounds = EditorGUILayout.Vector2Field ("Collision Bounds", controller.bubbleBounds);
		controller.bubbleBoundsOffset = EditorGUILayout.Vector2Field ("Collision Bounds Offset", controller.bubbleBoundsOffset);
		#endregion

		#region Validation
		if (controller.horizontalSpeed < 0f)
			controller.horizontalSpeed = 0f;
		if (controller.jumpHeight < 0f)
			controller.jumpHeight = 0f;
		if (controller.jumpTime < 0f)
			controller.jumpTime = 0f;
		if (controller.wrapWidth < 0f)
			controller.wrapWidth = 0f;
		
		if (controller.bounds.x < 0f)
			controller.bounds.x = 0f;
		if (controller.bounds.y < 0f)
			controller.bounds.y = 0f;
		
		#endregion

		if (GUI.changed)
			EditorUtility.SetDirty (controller);

	}

}
