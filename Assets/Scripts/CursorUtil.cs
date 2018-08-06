using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorUtil : MonoBehaviour
{
	[SerializeField] private Texture2D walkCursor = null;
	[SerializeField] private Texture2D enemyCursor = null;
	[SerializeField] private Texture2D unknownCursor = null;
	[SerializeField] private Vector2 cursorHotSpot = new Vector2(96, 96);

	private CameraRaycaster cameraRaycaster;
	

	private void Start ()
	{
		cameraRaycaster = GetComponent<CameraRaycaster>();
		cameraRaycaster.onLayerChange += OnLayerChanged;
	}

	private void OnLayerChanged (Utils.Layer layer)
	{
		switch (layer)
		{
			case Utils.Layer.Walkable:
				Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
				break;
			case Utils.Layer.Enemy:
				Cursor.SetCursor(enemyCursor, cursorHotSpot, CursorMode.Auto);
				break;
			default:
				Cursor.SetCursor(unknownCursor, cursorHotSpot, CursorMode.Auto);
				break;
		}
	}
}
