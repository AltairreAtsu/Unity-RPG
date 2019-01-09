using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(WaypointPath))]
public class WaypointEditor : Editor
{
	private ReorderableList reorderableList;
	private bool drawHandles = true;
	private WaypointPath waypointPath { get { return target as WaypointPath; } }

	private const float VECTOR_LABEL_WIDTH = 50;
	private const float VECTOR3_WIDTH = 200;
	private const float PAUSE_LABEL_WIDTH = 50;
	private const float PAUSE_FLOAT_WIDTH = 50;

	private void OnEnable()
	{
		reorderableList = new ReorderableList(waypointPath.waypoints, typeof(Waypoint), 
			draggable: true, displayHeader: true, displayAddButton: true, displayRemoveButton: true);

		reorderableList.drawHeaderCallback += DrawHeader;
		reorderableList.drawElementCallback += DrawElement;
		reorderableList.onAddCallback += AddItem;
		reorderableList.onRemoveCallback += RemoveItem;
		reorderableList.onReorderCallback += Reorder;

		//reorderableList.serializedProperty = serializedObject.FindProperty("waypoints");
	}

	private void OnDisable()
	{
		reorderableList.drawHeaderCallback -= DrawHeader;
		reorderableList.drawElementCallback -= DrawElement;
		reorderableList.onAddCallback -= AddItem;
		reorderableList.onRemoveCallback -= RemoveItem;
	}

	private void DrawHeader(Rect rect)
	{
		GUI.Label(rect, "Waypoints");
	}

	private void Reorder(ReorderableList list)
	{
		EditorUtility.SetDirty(target);
	}

	private void DrawElement(Rect rect, int index, bool active, bool focused)
	{
		Waypoint waypoint = waypointPath.waypoints[index];

		EditorGUI.BeginChangeCheck();

		float startingX = rect.x;
		EditorGUI.LabelField(
			new Rect(startingX, rect.y, VECTOR_LABEL_WIDTH, rect.height),
			"Point:");

		startingX += VECTOR_LABEL_WIDTH;

		waypoint.point = EditorGUI.Vector3Field(
			new Rect(startingX, rect.y, VECTOR3_WIDTH, rect.height),
			"", waypoint.point);

		startingX += VECTOR3_WIDTH + 20;

		EditorGUI.LabelField(
			new Rect(startingX, rect.y, PAUSE_LABEL_WIDTH, rect.height),
			"Pause:");

		startingX += PAUSE_LABEL_WIDTH;

		waypoint.pause = EditorGUI.FloatField(
			new Rect(startingX, rect.y, PAUSE_FLOAT_WIDTH, rect.height),
			waypoint.pause);

		if (EditorGUI.EndChangeCheck())
		{
			waypointPath.waypoints[index] = waypoint;
			EditorUtility.SetDirty(target);
		}
	}

	private void AddItem(ReorderableList list)
	{
		waypointPath.waypoints.Add(new Waypoint());

		EditorUtility.SetDirty(target);
	}

	private void RemoveItem(ReorderableList list)
	{
		waypointPath.waypoints.RemoveAt(list.index);

		EditorUtility.SetDirty(target);
	}

	public override void OnInspectorGUI()
	{
		drawHandles = EditorGUILayout.Toggle("Draw Waypoint Handles", drawHandles);
		
		// Draw the Default Inspector
		base.OnInspectorGUI();
		reorderableList.DoLayoutList();
	}

	protected virtual void OnSceneGUI()
	{
		if(!drawHandles) { return; }
		DrawHandles();
	}

	private void DrawHandles()
	{
		EditorGUI.BeginChangeCheck();
		var newWaypoints = new List<Waypoint>(waypointPath.waypoints);
		for (int i = 0; i < newWaypoints.Count; i++)
		{
			var waypoint = newWaypoints[i];
			waypoint.point = (Handles.PositionHandle(waypointPath.waypoints[i].point + waypointPath.transform.position,
						Quaternion.identity)) - waypointPath.transform.position;
			newWaypoints[i] = waypoint;
		}
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(waypointPath, "Change Waypoint Positions");
			waypointPath.waypoints = new List<Waypoint>(newWaypoints);
		}
	}
}
