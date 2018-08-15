using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour
{
	[SerializeField] private Texture2D walkCursor = null;
	[SerializeField] private Texture2D enemyCursor = null;
	[SerializeField] private Texture2D unknownCursor = null;
	[SerializeField] private Vector2 cursorHotSpot = new Vector2(96, 96);

	private CameraRaycaster cameraRaycaster;
	

	private void Start ()
	{
		cameraRaycaster = GetComponent<CameraRaycaster>();
		cameraRaycaster.notifyLayerChangeObservers += OnLayerChanged;
	}

	private void OnLayerChanged (int layer)
	{
		switch (layer)
		{
			case 9:
				Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
				break;
			case 10:
				Cursor.SetCursor(enemyCursor, cursorHotSpot, CursorMode.Auto);
				break;
			default:
				Cursor.SetCursor(unknownCursor, cursorHotSpot, CursorMode.Auto);
				break;
		}
	}
}
