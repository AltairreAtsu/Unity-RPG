using UnityEngine;
using UnityEngine.EventSystems;

using RPG.Characters;

namespace RPG.CameraUI { 
	public class CameraRaycaster : MonoBehaviour
	{
		[SerializeField] private Texture2D walkCursor = null;
		[SerializeField] private Texture2D enemyCursor = null;
		[SerializeField] private Vector2 cursorHotSpot = new Vector2(96, 96);

		private float maxRaycastDepth = 100f; // Hard coded value
		private int topPriorityLayerLastFrame = -1; // So get ? from start with Default layer terrain

		public delegate void OnMouseOverWalkable(Vector3 position); // declare new delegate type
		public event OnMouseOverWalkable onMouseOverWalkable; // instantiate an observer set

		public delegate void OnMouseOverEnemy(Enemy enemy);
		public event OnMouseOverEnemy onMouseOverEnemy;

		private bool currentlyOverEnemy = false;
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
			var rayHit = HitPriorityTarget();
			UpdateCursorDisplay(rayHit);
			NotifyObservers(rayHit);
		}

		private RayHit HitPriorityTarget()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);
			var rayHit = new RayHit(null, null, false);

			foreach (RaycastHit hit in raycastHits)
			{
				// Layer Priorities
				// Highest Priority returns right away
				var enemy = hit.collider.GetComponent<Enemy>();

				if (enemy != null)
				{
					rayHit.hit = hit;
					rayHit.enemyHit = enemy;
					return rayHit;
				}
				
				// Lowest Priority only sets a flag
				if (hit.collider.gameObject.layer == WALKABLE_LAYER)
				{
					rayHit.hit = hit;
					rayHit.hitWalkable = true;
				}
			}
			return rayHit;
		}

		private void UpdateCursorDisplay(RayHit rayHit)
		{
			if (rayHit.enemyHit != null && !currentlyOverEnemy)
			{
				currentlyOverEnemy = true;
				Cursor.SetCursor(enemyCursor, cursorHotSpot, CursorMode.Auto);
			}
			else if (rayHit.enemyHit == null && currentlyOverEnemy)
			{
				currentlyOverEnemy = false;
				Cursor.SetCursor(walkCursor, cursorHotSpot, CursorMode.Auto);
			}
		}

		private void NotifyObservers(RayHit rayHit)
		{
			if (rayHit.enemyHit != null)
			{
				if(onMouseOverEnemy != null)
				{
					onMouseOverEnemy(rayHit.enemyHit);
				}
				return;
			}
			if (rayHit.hitWalkable)
			{
				var hit = (RaycastHit)rayHit.hit;
				if(onMouseOverWalkable != null)
				{
					onMouseOverWalkable(hit.point);
				}
				return;
			}
		}

	}

	public struct RayHit
	{
		public RaycastHit? hit;
		public Enemy enemyHit;
		public bool hitWalkable;

		public RayHit(RaycastHit? hit, Enemy enemyHit, bool hitWalkable)
		{
			this.hit = hit;
			this.enemyHit = enemyHit;
			this.hitWalkable = hitWalkable;
		}
	}
}