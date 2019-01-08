using UnityEngine;
using UnityEngine.EventSystems;

using RPG.Characters;
using RPG.Weapons;

namespace RPG.CameraUI {

	public enum CursorState { Enemy, Walkable, Pickup }

	public class CameraRaycaster : MonoBehaviour
	{
		[SerializeField] private Texture2D walkCursor = null;
		[SerializeField] private Texture2D interactableCursor = null;
		[SerializeField] private Texture2D enemyCursor = null;
		[SerializeField] private Vector2 cursorHotSpot = new Vector2(96, 96);

		private float maxRaycastDepth = 100f; // Hard coded value
		private int topPriorityLayerLastFrame = -1; // So get ? from start with Default layer terrain

		public delegate void OnMouseOverWalkable(Vector3 position); // declare new delegate type
		public event OnMouseOverWalkable onMouseOverWalkable; // instantiate an observer set

		public delegate void OnMouseOverEnemy(Enemy enemy);
		public event OnMouseOverEnemy onMouseOverEnemy;

		public delegate void OnMouseOverPickup(WeaponPickupPoint pickup);
		public event OnMouseOverPickup onMouseOverPickup;

		private CursorState currentState = CursorState.Walkable;
		private static int WALKABLE_LAYER = 9;

		private void Start()
		{
			Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
		}

		private void Update()
		{
			// Check if pointer is over an interactable UI element
			if (EventSystem.current.IsPointerOverGameObject ())
			{
				return; // Do Nothing
			}

			// Raycast to max depth, every frame as things can move under mouse
			PerformRaycast();

		}

		private void PerformRaycast()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);

			if (RaycastForEnemies(raycastHits)) { return; }
			if (RaycastForPickups(raycastHits)) { return; }
			if (RaycastForWalkable(raycastHits)) { return; }
			UpdateCursorDisplay(CursorState.Walkable);
		}

		private bool RaycastForEnemies(RaycastHit[] raycastHits)
		{
			foreach (RaycastHit hit in raycastHits)
			{
				var enemy = hit.collider.GetComponent<Enemy>();
				if(enemy != null && enemy.Health.Alive)
				{
					NotifyEnemyObservers(enemy);
					UpdateCursorDisplay(CursorState.Enemy);
					return true;
				}
			}
			return false;
		}

		private bool RaycastForWalkable(RaycastHit[] raycastHits)
		{
			foreach (RaycastHit hit in raycastHits)
			{
				if (hit.collider.gameObject.layer == WALKABLE_LAYER)
				{
					NotifyWalkableObservers(hit);
					UpdateCursorDisplay(CursorState.Walkable);
					return true;
				}
			}
			return false;
		}

		private bool RaycastForPickups(RaycastHit[] raycastHits)
		{
			foreach(RaycastHit hit in raycastHits)
			{
				var weaponPickup = hit.collider.GetComponent<WeaponPickupPoint>();
				if (weaponPickup != null)
				{
					NotifyPickupObservers(weaponPickup);
					UpdateCursorDisplay(CursorState.Pickup);
					return true;
				}
			}
			return false;
		}

		private void NotifyPickupObservers(WeaponPickupPoint weapon)
		{
			if(onMouseOverPickup != null)
			{
				onMouseOverPickup(weapon);
			}
		}

		private void NotifyEnemyObservers(Enemy enemy)
		{
			if (onMouseOverEnemy != null)
			{
				onMouseOverEnemy(enemy);
			}
		}

		private void NotifyWalkableObservers(RaycastHit hit)
		{
			if (onMouseOverWalkable != null)
			{
				onMouseOverWalkable(hit.point);
			}
		}

		private void UpdateCursorDisplay(CursorState targetState)
		{
			if(currentState == targetState) { return;  }
			currentState = targetState;
			switch (targetState)
			{
				case CursorState.Enemy:
					Cursor.SetCursor(enemyCursor, cursorHotSpot, CursorMode.Auto);
					break;
				case CursorState.Pickup:
					Cursor.SetCursor(interactableCursor, cursorHotSpot, CursorMode.Auto);
					break;
				case CursorState.Walkable:
					Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
					break;
			}
		}
	}
}