﻿using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackGenerator))]
public class TrackMeshGenerator : MonoBehaviour {
    private TrackGenerator tracker;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    [SerializeField] private float Width = 1;
    
    private void Awake() {
        tracker = GetComponent<TrackGenerator>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void OnEnable() {
        tracker.UpdatedTrack += UpdateMesh;
    }

    private void OnDisable() {
        tracker.UpdatedTrack -= UpdateMesh;
    }

    private void OnValidate() {
        if (Width < 0.1) Width = 0.1f;
        
        if(tracker != null) tracker.ShouldUpdate = true;
    }

    void UpdateMesh() {
        var mesh = CreateRoadMesh(tracker.getPath());
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private Mesh CreateRoadMesh(List<TrackGenerator.PathInfo> points) {
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();
        
        for (var i = 0; i < points.Count; i++) {
            if (i >= points.Count - 7) continue;
            
            var currentPos = transform.InverseTransformPoint(points[i].position);
            var nextPos = transform.InverseTransformPoint(points[i + 1].position);
            var nextNextPos = transform.InverseTransformPoint(points[i + 2].position);

            var currentPosRight = Quaternion.AngleAxis(90, nextPos - currentPos) * points[i].normal;
            var nextPosRight = Quaternion.AngleAxis(90, nextNextPos - nextPos) * points[i + 1].normal;


            verts.Add(currentPos - currentPosRight * Width);
            verts.Add(currentPos + currentPosRight * Width);
            verts.Add(nextPos + nextPosRight * Width);
            verts.Add(nextPos - nextPosRight * Width);
            
            uvs.Add(new Vector2(0,0));
            uvs.Add(new Vector2(0,1));
            uvs.Add(new Vector2(1,1));
            uvs.Add(new Vector2(1,0));
            
            //tris.Add(0);
            //tris.Add(3);
            //tris.Add(1);
            
            tris.Add(2 + i * 4);
            tris.Add(1 + i * 4);
            tris.Add(0 + i * 4);
            
            tris.Add(3 + i * 4);
            tris.Add(2 + i * 4);
            tris.Add(0 + i * 4);
            
            tris.Add(0 + i * 4);
            tris.Add(1 + i * 4);
            tris.Add(2 + i * 4);
            
            tris.Add(0 + i * 4);
            tris.Add(2 + i * 4);
            tris.Add(3 + i * 4);
            
            //vertIndex += 2;
            //triIndex += 6;
        }

        Mesh mesh = new Mesh {
            vertices = verts.ToArray(),
            triangles = tris.ToArray(),
            uv = uvs.ToArray(),
            name = "Meshy"
        };

        return mesh;
    }
}
