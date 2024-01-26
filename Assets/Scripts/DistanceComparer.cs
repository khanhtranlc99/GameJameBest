using System.Collections.Generic;
using UnityEngine;

public class DistanceComparer<T> : IComparer<T> where T : Component
{
	public Vector3 playerPosition;

	public int Compare(T a, T b)
	{
		return (playerPosition - a.transform.position).sqrMagnitude.CompareTo((playerPosition - b.transform.position).sqrMagnitude);
	}
}
