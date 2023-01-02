using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TrackGenerator : MonoBehaviour 
{
	[SerializeField] private int numberOfPoints = 20;
	[SerializeField] private List<Transform> locations = new List<Transform>();
	
	private List<Vector3> ballPoints = new List<Vector3>();
	private List<PathInfo> pathPoints = new List<PathInfo>();

	public bool ShouldUpdate;
	
	public delegate void test();
	public event test UpdatedTrack;

	private void Update() {
		if(!Application.isPlaying) UpdateMesh();
	}

	private void FixedUpdate() {
		if(ShouldUpdate) UpdateMesh();
	}

	[ContextMenu("Update Mesh")]
	private void UpdateMesh() 
	{
		ballPoints.Clear();
		pathPoints.Clear();
		locations.Clear();
		
		for (var index = 0; index < transform.childCount; index++) {
			var trans = transform.GetChild(index);
			if (trans.childCount == 0) {
				locations.Add(trans);
				continue;
			}
			
			for (var childIndex = 0; childIndex < trans.childCount; childIndex++) {
				locations.Add(trans.GetChild(childIndex));
			}
		}
		
		for (var index = 0; index < locations.Count; index++)
		{
			var trans = locations[index];
			
			ballPoints.Add(trans.position);

			if (index  >= transform.childCount - 1) {
				continue;
			}

			var transPosition = trans.position;
			ballPoints.Add(transPosition + (locations[index + 1].position - transPosition) * 0.33f);
			ballPoints.Add(transPosition + (locations[index + 1].position - transPosition) * 0.66f);
		}
		
		Vector3 prevVec = Vector3.zero;
		for(var j = 0; j < ballPoints.Count; j++) {
			int p0index = j;
			int p1index = j + 1;
			int p2index = j + 2;
			
			if (p1index == ballPoints.Count) {
				p1index = ballPoints.Count - 1;
			}
			
			if (p2index >= ballPoints.Count) {
				p2index = ballPoints.Count - 1;
			}
			
			Vector3 p0 = (ballPoints[p0index] + ballPoints[p1index]) * 0.5f;
			Vector3 p1 =  ballPoints[p1index];
			Vector3 p2 = (ballPoints[p1index] + ballPoints[p2index]) * 0.5f;
			
			float pointStep = 1.0f / numberOfPoints;
			
			for(var i = 0; i < numberOfPoints; i++) 
			{
				float t = i * pointStep;
				Vector3 position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
				
				var normal = Vector3.up;
				
				var pathInfo = new PathInfo {
					vec = position, 
					normal = normal
				};
				
				var forward = (position - prevVec).normalized;
				
				if (j % 3 == 0) { 
					pathInfo.normal = Quaternion.AngleAxis(45 * (1 - t), forward) * Vector3.up;
				}
				
				if (j % 3 == 1) { 
					pathInfo.normal = Quaternion.AngleAxis(45 * t, forward) * Vector3.up;
				}
				
				if (j % 3 == 2) {
					pathInfo.normal = Quaternion.AngleAxis(45, forward) * Vector3.up;
				}


				prevVec = position;
				
				/*
				if (j % 3 == 2) {
					pathInfo.normal = bankNormals[j / 3];
				} else {
					Vector3 before = bankNormals[j / 3];
					
					int afterIndex = j / 3 + 1;
					if (afterIndex >= bankNormals.Count) afterIndex = j / 3;
						
					Vector3 after = bankNormals[afterIndex];

					Vector3 midNormal = Vector3.Lerp(before, after, 0.5f);
					
					if (j % 3 == 0) {
						pathInfo.normal = Vector3.Lerp(before, midNormal, t * t);
					}

					if (j % 3 == 1) {
						pathInfo.normal = Vector3.Lerp(midNormal, after, 1 - (1 - t) * (1 - t));
					}
				}
				*/
				
				pathPoints.Add(pathInfo);
			}
			
		}

		UpdatedTrack?.Invoke();
		ShouldUpdate = false;
	}

	public struct PathInfo {
		public Vector3 vec;
		public Vector3 normal;
	}

	public void OnValidate() {
		numberOfPoints = Math.Max(1, numberOfPoints);
		ShouldUpdate = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.5f, 0f);
		foreach (var ballPos in ballPoints) {
			Gizmos.DrawSphere(ballPos, 0.2f);
		}
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(pathPoints[0].vec + -Vector3.forward, pathPoints[0].vec + Vector3.forward);
		
		
		for (var index = 0; index < pathPoints.Count; index++)
		{
			int start = index;
			int next = index + 1;
			if (next >= pathPoints.Count) next = index;
			
			var direction = pathPoints[next].vec - pathPoints[start].vec;

			var up = pathPoints[start].normal;
			Vector3 right = Quaternion.AngleAxis(90f, direction) * pathPoints[start].normal;
			
			Gizmos.color = new Color(0.11f, 1f, 0f);
			Gizmos.DrawLine(pathPoints[start].vec + right, pathPoints[next].vec + right);
			Gizmos.DrawLine(pathPoints[start].vec + -right, pathPoints[next].vec + -right);
			
			Gizmos.color = new Color(0f, 0, 1f);
			Gizmos.DrawLine(pathPoints[start].vec, pathPoints[start].vec + Vector3.up);
			
			Gizmos.color = new Color(1f, 0f, 0f);
			Gizmos.DrawLine(pathPoints[start].vec, pathPoints[start].vec + up);
		}
	}

	public List<PathInfo> getPath() {
		return pathPoints;
	}
}