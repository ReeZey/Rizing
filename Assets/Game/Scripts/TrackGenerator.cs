using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TrackGenerator : MonoBehaviour 
{
	[SerializeField] private int numberOfPoints = 20;
	[SerializeField] private float turnAngle = 45f;
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
		
		for (var locationIndex = 0; locationIndex < locations.Count; locationIndex++)
		{
			var trans = locations[locationIndex];
			
			ballPoints.Add(trans.position);

			if (locationIndex  >= transform.childCount - 1) {
				continue;
			}

			var transPosition = trans.position;
			ballPoints.Add(transPosition + (locations[locationIndex + 1].position - transPosition) * 0.33f);
			ballPoints.Add(transPosition + (locations[locationIndex + 1].position - transPosition) * 0.66f);
		}
		
		Vector3 prevVec = Vector3.zero;
		for(var ballIndex = 0; ballIndex < ballPoints.Count; ballIndex++) {
			int p0index = ballIndex;
			int p1index = ballIndex + 1;
			int p2index = ballIndex + 2;
			
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
			
			int modulu = (ballIndex + 1) % 3;

			if (modulu == 2 && ballIndex < ballPoints.Count - 3) {
				p0index++;
				p1index++;
				p2index++;
			}
			
			if (modulu == 1 && ballIndex > 2){
				p0index--;
				p1index--;
				p2index--;
			}

			for(var i = 0; i < numberOfPoints; i++) 
			{
				float t = i * pointStep;
				Vector3 position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
				
				var normal = Vector3.up;
				
				var pathInfo = new PathInfo {
					position = position, 
					normal = normal
				};
				
				var forward = (position - prevVec).normalized;

				var firstForward = (ballPoints[p1index] - ballPoints[p0index]).normalized;
				var secoundForward = (ballPoints[p2index] - ballPoints[p1index]).normalized;

				Vector3 cross = Vector3.Cross(secoundForward, firstForward);
				float angle = 1 - Vector3.Dot(secoundForward, firstForward);
				float final = cross.y * angle * turnAngle;

				//Debug.Log(turn);
				
				
				switch (modulu) {
					case 0:
						pathInfo.normal = Quaternion.AngleAxis(final, forward) * Vector3.up; // control
						break;
					case 1:
						if (ballIndex < ballPoints.Count - 4) {
							var firstForwarda = (ballPoints[p1index + 3] - ballPoints[p0index + 3]).normalized;
							var secoundForwarda = (ballPoints[p2index + 3] - ballPoints[p1index + 3]).normalized;

							Vector3 crossa = Vector3.Cross(secoundForwarda, firstForwarda);
							float anglea = 1 - Vector3.Dot(secoundForwarda, firstForwarda);
							float finala = crossa.y * anglea * turnAngle;
							
							pathInfo.normal = Quaternion.AngleAxis(Mathf.Lerp(final, finala, t * 0.5f), forward) * Vector3.up;
						} else {
							pathInfo.normal = Quaternion.AngleAxis(final * (1 - t), forward) * Vector3.up;
						}
						break;
					case 2:
						if (ballIndex > 2) {
							var firstForwarda = (ballPoints[p1index - 3] - ballPoints[p0index - 3]).normalized;
							var secoundForwarda = (ballPoints[p2index - 3] - ballPoints[p1index - 3]).normalized;

							Vector3 crossa = Vector3.Cross(secoundForwarda, firstForwarda);
							float anglea = 1 - Vector3.Dot(secoundForwarda, firstForwarda);
							float finala = crossa.y * anglea * turnAngle;
							
							pathInfo.normal = Quaternion.AngleAxis(Mathf.Lerp(final, finala,  (1 - t) * 0.5f), forward) * Vector3.up;
						} else {
							pathInfo.normal = Quaternion.AngleAxis(final * t, forward) * Vector3.up;
						}
						break;
				}


				prevVec = position;
				pathPoints.Add(pathInfo);
			}
			
		}

		UpdatedTrack?.Invoke();
		ShouldUpdate = false;
	}

	[Serializable]
	public struct PathInfo {
		public Vector3 position;
		public Vector3 normal;
	}

	public void OnValidate() {
		numberOfPoints = Math.Max(1, numberOfPoints);
		ShouldUpdate = true;
	}

	private void OnDrawGizmos() {
		/*
		Gizmos.color = new Color(1f, 0.5f, 0f);
		foreach (var ballPos in ballPoints) {
			Gizmos.DrawSphere(ballPos, 0.1f);
		}
		*/

		for (var index = 0; index < pathPoints.Count; index++) {
			if (index >= pathPoints.Count - 2) break;
			
			var currentPos = transform.InverseTransformPoint(pathPoints[index].position);
			var nextPos = transform.InverseTransformPoint(pathPoints[index + 1].position);
			var nextNextPos = transform.InverseTransformPoint(pathPoints[index + 2].position);
			
			var currentPosRight = Quaternion.AngleAxis(90, nextPos - currentPos) * pathPoints[index].normal;
			var nextPosRight = Quaternion.AngleAxis(90, nextNextPos - nextPos) * pathPoints[index + 1].normal;
			
			var path = pathPoints[index];
			var next = pathPoints[index + 1];
			
			
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(path.position, path.position + path.normal);
			
			Gizmos.color = Color.green;
			Gizmos.DrawLine(path.position + currentPosRight, next.position + nextPosRight);
			Gizmos.DrawLine(path.position - currentPosRight, next.position - nextPosRight);
		}

		for (var index = 0; index < ballPoints.Count; index++) {
			if (index == ballPoints.Count - 2) break;
			
			int parent = index / 3;
			int mod = index % 3;

			//Handles.Label(ballPoints[index] + Vector3.up, $"index: {index}, mod {mod}, parent {parent}");
		}
	}

	public List<PathInfo> getPath() {
		return pathPoints;
	}
}