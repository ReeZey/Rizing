using System.Collections.Generic;
using Rizing.Abstract;
using UnityEngine;

public class TrackFollower : BaseEntity {
    [SerializeField] private TrackGenerator tracker;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float turnSpeed = 1f;

    [Header("Settings")]
    [SerializeField] private List<GameObject> wheels = new List<GameObject>();
    [SerializeField] private int wheelSpacing = 10;

    private int lastBogie;
    private bool start;
    private bool running;
    private bool hasPath => pathInfos.Count > 0;
    private List<TrackGenerator.PathInfo> pathInfos = new List<TrackGenerator.PathInfo>();

    private void OnCollisionEnter(Collision collision) {
        if(!collision.transform.CompareTag("Player")) return;

        collision.transform.parent = transform;
        
        start = true;
    }

    private void OnCollisionExit(Collision collision) {
        if(!collision.transform.CompareTag("Player")) return;
        
        collision.transform.parent = null;
    }

    public override void FixedProcess(float deltaTime) {
        if (start) running = true;
        if (!start && !running) return;

        start = false;

        if (!hasPath) {
            pathInfos.AddRange(tracker.getPath());
        }
        
        for (var wheelIndex = 0; wheelIndex < wheels.Count; wheelIndex++) {
            int pathIndex = wheelSpacing * wheelIndex + lastBogie;

            if (pathIndex >= pathInfos.Count) {
                lastBogie = 0;
                break;
            }
            
            //continue
            var nextPath = pathInfos[pathIndex];
            var nextPosition = nextPath.position + nextPath.normal;

            var wheelTransform = wheels[wheelIndex].transform;
            var wheelPosition = wheelTransform.position;

            var diff = nextPosition - wheelPosition;
            var moveAmount = diff.normalized * speed * deltaTime;
            
            if (moveAmount.sqrMagnitude >= diff.sqrMagnitude) {
                if (wheelIndex == 0) lastBogie++;
                
                wheels[wheelIndex].transform.position = nextPosition;
                continue;
            }
            
            if(diff.normalized.sqrMagnitude > 0.1f) wheelTransform.rotation = Quaternion.Slerp(wheelTransform.rotation, Quaternion.LookRotation(diff.normalized, nextPath.normal), deltaTime * turnSpeed);
            wheelTransform.position += moveAmount;
        }

        var SumsOfWheels = MiddleOfWheels();

        var cartTransform = transform;
        cartTransform.position = SumsOfWheels.pos + SumsOfWheels.normal * 0.5f;
        cartTransform.rotation = SumsOfWheels.quat;
    }
    
    private (Vector3 pos, Quaternion quat, Vector3 normal) MiddleOfWheels() {
        Vector3 pos = Vector3.zero;
        Vector3 upVector = Vector3.zero;
        
        foreach (var wheel in wheels) {
            pos += wheel.transform.position;
            upVector += wheel.transform.up;
        }

        pos /= wheels.Count;
        upVector /= wheels.Count;
        var quat = Quaternion.LookRotation(wheels[1].transform.position - wheels[0].transform.position, upVector);
        
        return (pos, quat, upVector);
    }
}
