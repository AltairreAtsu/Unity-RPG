using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
	[SerializeField] public bool pathIsRingLoop;
	[SerializeField] private float precesionThreshold = 1.6f;

	[HideInInspector] public List<Waypoint> waypoints;

	private const float DEBUG_SPHERE_RADIUS = .3f;

	public Vector3 GetNextWaypoint(WaypointIterator iterator)
	{
		var atArrayStart = iterator.index == 0;
		var atEndOfArray = iterator.index == waypoints.Count - 1;

		if (pathIsRingLoop && atEndOfArray)
		{
			iterator.index = 0;
		}
		else if (!pathIsRingLoop && atEndOfArray)
		{
			iterator.returning = true;
			iterator.index -= 1;
		}
		else if (!pathIsRingLoop && iterator.returning && atArrayStart)
		{
			iterator.returning = false;
			iterator.index += 1;
		}
		else if (!pathIsRingLoop && iterator.returning)
		{
			iterator.index -= 1;
		}
		else
		{
			iterator.index += 1;
		}
		if(waypoints[iterator.index].pause > 0f)
		{
			iterator.pause = waypoints[iterator.index].pause;
		}
		return waypoints[iterator.index].point + transform.position;
	}

	public bool AtWaypoint(WaypointIterator iterator, Vector3 agentPosition)
	{
		// We don't care about the y value, that is handled by nav mesh well enough
		agentPosition.y = waypoints[iterator.index].point.y;
		return Vector3.Distance(agentPosition, waypoints[iterator.index].point + transform.position) <= precesionThreshold;
	}

	public WaypointIterator GetNewIterator()
	{
		return new WaypointIterator(0, false, waypoints[0].pause);
	}

	public Vector3 GetFirstWaypoint()
	{
		return waypoints[0].point + transform.position;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		
		for (int i = 0; i < waypoints.Count; i++)
		{
			if (i == 0) Gizmos.color = Color.blue;
			Gizmos.DrawSphere(waypoints[i].point + transform.position, DEBUG_SPHERE_RADIUS);
			Gizmos.color = Color.yellow;
			if (i > 0)
			{
				Gizmos.DrawLine(waypoints[i - 1].point + transform.position, waypoints[i].point + transform.position);
			}
		}
		if (pathIsRingLoop)
		{
			Gizmos.DrawLine(waypoints[0].point, waypoints[waypoints.Count - 1].point);
		}
	}
}

[System.Serializable]
public struct Waypoint
{
	[SerializeField] public Vector3 point;
	[SerializeField] public float pause;

	public void SetPoint(Vector3 point)
	{
		this.point = point;
	}
}

public class WaypointIterator
{
	public int index;
	public bool returning;
	public float pause;

	public WaypointIterator(int index, bool returning, float pause)
	{
		this.index = index;
		this.returning = returning;
		this.pause = pause;
	}
}
