using System;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

[ExecuteInEditMode]
public class TrackGenerator : MonoBehaviour 
{
	[SerializeField] private int numberOfPoints = 20;
	[SerializeField] private List<Vector3> points = new List<Vector3>();
	
	private List<Vector3> breakPoints = new List<Vector3>();
	private List<Vector3> pathPoints = new List<Vector3>();

	private void Update () 
	{
		breakPoints.Clear();
		pathPoints.Clear();
		points.Clear();
		
		for (var index = 0; index < transform.childCount; index++) {
			var trans = transform.GetChild(index);
			if (trans.childCount == 0) {
				points.Add(trans.position);
				continue;
			}
			
			for (var childIndex = 0; childIndex < trans.childCount; childIndex++) {
				points.Add(trans.GetChild(childIndex).position);
			}
		}
		
		for (var index = 0; index < points.Count; index++)
		{
			var position = points[index];
			breakPoints.Add(position);

			if (index == 0 || index == points.Count - 1) {
				breakPoints.Add(position);
			}
			
			if (index + 1 >= transform.childCount) {
				continue;
			}
			breakPoints.Add(position + (points[index + 1] - position) * 0.5f);
		}

		for(var j = 0; j < breakPoints.Count; j++) {
			int p0index = j;
			int p1index = j + 1;
			int p2index = j + 2;
			
			if (p1index == breakPoints.Count) {
				p1index = j;
			}
			
			if (p2index >= breakPoints.Count) {
				p2index = j;
			}
			
			Vector3 p0 = (breakPoints[p0index] + breakPoints[p1index]) * 0.5f;
			Vector3 p1 =  breakPoints[p1index];
			Vector3 p2 = (breakPoints[p1index] + breakPoints[p2index]) * 0.5f;
			
			float pointStep = 1.0f / numberOfPoints;

			for(var i = 0; i < numberOfPoints; i++) 
			{
				float t = i * pointStep;
				Vector3 position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
				pathPoints.Add(position);
			}
		}
	}

	public void OnValidate() {
		numberOfPoints = Math.Max(2, numberOfPoints);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		
		for (var index = 0; index < breakPoints.Count; index++) {
			//int finish = index + 1;
			//if (finish == breakPoints.Count) finish = index;

			Gizmos.DrawLine(breakPoints[index], breakPoints[index] + new Vector3(0, 2, 0));
			//Gizmos.DrawLine(breakPoints[index], breakPoints[finish]);
		}
		
		Gizmos.color = new Color(1f, 0.02f, 0f);
		for (var index = 0; index < pathPoints.Count; index++)
		{
			int start = index;
			int finish = index + 1;
			if (finish == pathPoints.Count) finish = index;

			//Gizmos.DrawLine(pathPoints[start], pathPoints[start] + new Vector3(2, 0, 0));
			//Gizmos.DrawLine(pathPoints[start], pathPoints[finish]);

			var direction = pathPoints[finish] - pathPoints[start];
			
			//Gizmos.color = Color.red;
			//Gizmos.DrawLine(pathPoints[start], pathPoints[start] + direction.normalized);
			
			Vector3 right = Quaternion.AngleAxis(90f, Vector3.up) * direction;
			
			Gizmos.DrawLine(pathPoints[start], pathPoints[start] + right.normalized);
			Gizmos.DrawLine(pathPoints[start], pathPoints[start] + -right.normalized);
		}
	}
}