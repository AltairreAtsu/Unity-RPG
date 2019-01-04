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

		public delegate void OnRightClick (RaycastHit raycastHit, int layerHit);
		public event OnRightClick notifyRightClickObservers;

		public delegate void OnClickWalkable(Vector3 position); // declare new delegate type
		public event OnClickWalkable notifyWalkableClickObservers; // instantiate an observer set

		public delegate void OnClickEnemy(Enemy enemy);
		public event OnClickEnemy notifyEnemyClickObsevers;

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
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] raycastHits = Physics.RaycastAll (ray, maxRaycastDepth);

			var rayHit = HitPriorityTarget(raycastHits);

			UpdateCursorDisplay(rayHit);
			HandleMouseInput(rayHit);
		}

		private RayHit HitPriorityTarget(RaycastHit[] raycastHits)
		{
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

		private void HandleMouseInput(RayHit rayHit)
		{
			// Notify delegates of highest priority game object under mouse when clicked
			if (Input.GetMouseButton(0))
			{
				if (rayHit.enemyHit != null)
				{
					notifyEnemyClickObsevers(rayHit.enemyHit);
					return;
				}
				if (rayHit.hitWalkable)
				{
					RaycastHit hit = (RaycastHit)rayHit.hit;
					notifyWalkableClickObservers(hit.point);
					return;
				}
			}
			if (Input.GetMouseButtonDown(1))
			{
				RaycastHit hit = (RaycastHit)rayHit.hit;
				notifyRightClickObservers(hit, hit.collider.gameObject.layer);
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