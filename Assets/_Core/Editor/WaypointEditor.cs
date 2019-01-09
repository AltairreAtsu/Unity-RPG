using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointPath))]
public class WaypointEditor : Editor
{
	private bool drawHandles = true;

	public override void OnInspectorGUI()
	{
		drawHandles = EditorGUILayout.Toggle("Draw Waypoint Handles", drawHandles);
		
		// Draw the Default Inspector
		base.OnInspectorGUI();
	}

	protected virtual void OnSceneGUI()
	{
		if(!drawHandles) { return; }
		WaypointPath path = (WaypointPath)target;
		DrawHandles(path);
	}

	private void DrawHandles(WaypointPath path)
	{
		EditorGUI.BeginChangeCheck();
		var newWaypoints = (Waypoint[])path.waypoints.Clone();
		for (int i = 0; i < newWaypoints.Length; i++)
		{
			newWaypoints[i].point = (Handles.PositionHandle(path.waypoints[i].point + path.transform.position,
				Quaternion.identity)) - path.transform.position;
		}
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(path, "Change Waypoint Positions");
			path.waypoints = newWaypoints;
		}
	}
}
