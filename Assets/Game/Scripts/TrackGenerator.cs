using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class TrackGenerator : MonoBehaviour 
{
	[Header("Settings")]
	[SerializeField] private float LOD = 2;
	[SerializeField] private float turnAngle = 45f;
	
	[SerializeField] private int maxLength = 20;
	[SerializeField] private int pointSpacing = 50;
	[SerializeField] private int heightsAndValleys = 20;
	[SerializeField] private bool shouldCreateNext;
	
	[Header("Linking")]
	[SerializeField] private GameObject StationPrefab;
	[SerializeField] private CinemachineVirtualCamera virtualCamera;
	
	private bool createNextPath = true;

	private int pointSpacingMin = 25;
	
	private List<Transform> locations = new List<Transform>();
	private List<Vector3> ballPoints = new List<Vector3>();
	private List<PathInfo> pathPoints = new List<PathInfo>();
	private FastNoiseLite fnl = new FastNoiseLite();

	[NonSerialized] public bool ShouldUpdate;
	private int point;

	public delegate void UpdateTrack();
	public event UpdateTrack UpdatedTrack;

	private void Start() {
		fnl.SetSeed(Random.Range(0, 1000));
	}

	private void Update() {
		if (!Application.isPlaying) {
			UpdateMesh();
		} else {
			if (createNextPath && shouldCreateNext) {
				createNextPath = false;
				StartCoroutine(CreateNextCoroutine());
			}
		}
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
			ballPoints.Add(transPosition + (locations[locationIndex + 1].position - transPosition) * 0.45f);
			ballPoints.Add(transPosition + (locations[locationIndex + 1].position - transPosition) * 0.55f);
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
			
			float distanceCalc = Vector3.Distance(p0, p2) / LOD;
			if (distanceCalc < 2) distanceCalc = 2;
			
			float pointStep = 1f / distanceCalc;
			
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

			for(var i = 0; i < distanceCalc; i++) 
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
				float final = Mathf.Clamp(cross.y * angle * turnAngle, -45, 45);

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
							float finala = Mathf.Clamp(crossa.y * anglea * turnAngle, -45, 45);
							
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
							float finala = Mathf.Clamp(crossa.y * anglea * turnAngle, -45, 45);
							
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

	[ContextMenu("Find Next Path")]
	private void CreateNext() {
		var trackTransform = transform;
		int childCount = trackTransform.childCount;

		var forward = Vector3.forward;
		
		var secondLastChild = transform.GetChild(childCount - 2);
		var secondLastChildPos = secondLastChild.position;
		
		var lastChild = transform.GetChild(childCount - 1);
		var lastChildPos = lastChild.position;
		

		lastChildPos.y = 0;
		secondLastChildPos.y = 0;

		if ((lastChildPos - secondLastChildPos).sqrMagnitude > 1) {
			forward = (lastChildPos - secondLastChildPos).normalized;
		}
		
		var scaledForward = forward * Random.Range(pointSpacingMin, Mathf.Max(pointSpacingMin, pointSpacing) + 1);
		
		float angleNoise = Mathf.Clamp(fnl.GetNoise(point, 0), -1, 1) * 10;
		float heightNoise = (Mathf.Clamp(fnl.GetNoise(0, point), -1, 1) + 0.5f) * heightsAndValleys;
		point++;

		var rotatedForward = Quaternion.AngleAxis(angleNoise, Vector3.up) * scaledForward;
		var height = Vector3.up * heightNoise;

		if (height.y < 0) height.y = 0;
		
		Instantiate(StationPrefab, lastChildPos + rotatedForward + height, Quaternion.identity, trackTransform);

		if (childCount > 2) {
			int middleIndex = childCount / 2;
			var middle = trackTransform.GetChild(middleIndex);
			var nextMiddle = trackTransform.GetChild(middleIndex + 1);
		
			virtualCamera.Follow = middle;
			virtualCamera.LookAt = nextMiddle;
		}
	}

	private IEnumerator CreateNextCoroutine() {
		yield return new WaitForSecondsRealtime(1f / 120);
		createNextPath = true;
		
		if (transform.childCount > maxLength) {
			Destroy(transform.GetChild(0).gameObject);
			yield break;
		}
		
		CreateNext();
	}

	[Serializable]
	public struct PathInfo {
		public Vector3 position;
		public Vector3 normal;
	}

	public void OnValidate() {
		LOD = Math.Max(1, LOD);
		pointSpacing = Math.Max(pointSpacingMin, pointSpacing);
		ShouldUpdate = true;
	}

	private void OnDrawGizmos() {
		/*
		Gizmos.color = new Color(1f, 0.5f, 0f);
		foreach (var ballPos in ballPoints) {
			Gizmos.DrawSphere(ballPos, 0.1f);
		}
		*/

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