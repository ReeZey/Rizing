using System;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

[ExecuteInEditMode]
public class TrackGenerator : MonoBehaviour 
{
	[SerializeField] private int numberOfPoints = 20;

	private List<Vector3> controlPoints = new List<Vector3>();
	private List<Vector3> controlPoints2 = new List<Vector3>();

	private void Update () 
	{
		controlPoints.Clear();
		controlPoints2.Clear();
		for (var index = 0; index < transform.childCount; index++)
		{
			Transform start = transform.GetChild(index);
			var trackPointData = start.GetComponent<ITrackPoint>();

			if (trackPointData == null) continue;
			
			controlPoints.Add(start.position);
		}
		
		for(var j = 0; j < controlPoints.Count; j++)
		{
			int p1index = j + 1 >= controlPoints.Count ? j + 1 - controlPoints.Count : j + 1;
			int p2index = j + 2 >= controlPoints.Count ? j + 2 - controlPoints.Count : j + 2;
			
			Vector3 p0 = 0.5f * (controlPoints[j] + controlPoints[p1index]);
			Vector3 p1 = controlPoints[p1index];
			Vector3 p2 = 0.5f * (controlPoints[p1index] + controlPoints[p2index]);
			
			float pointStep = 1.0f / numberOfPoints;
			if (j == controlPoints.Count)
			{
				pointStep = 1.0f / (numberOfPoints - 1.0f);
			}

			for(var i = 0; i < numberOfPoints; i++) 
			{
				float t = i * pointStep;
				Vector3 position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
				controlPoints2.Add(position);
			}
		}
	}

	public void OnValidate() {
		if(numberOfPoints < 2)
		{
			numberOfPoints = 2;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		for (var index = 0; index < controlPoints2.Count; index++)
		{
			int start = index;
			int finish = index + 1;
			if (finish == controlPoints2.Count)
			{
				finish = 0;
			}
			
			Gizmos.DrawLine(controlPoints2[start], controlPoints2[start] + new Vector3(0, 2, 0));
			Gizmos.DrawLine(controlPoints2[start], controlPoints2[finish]);
		}
	}

	public List<Vector3> GetPath()
	{
		return controlPoints2;
	}
}