using System.Collections.Generic;
using Rizing.Abstract;
using UnityEngine;

public class TrackFollower : BaseEntity {
    [SerializeField] private TrackGenerator tracker;
    [SerializeField] private List<Rigidbody> moveables = new List<Rigidbody>();

    [SerializeField] private float speed = 1f;
    
    private bool start;
    private bool running;
    private bool hasPath => pathInfos.Count > 0;
    private Queue<TrackGenerator.PathInfo> pathInfos = new Queue<TrackGenerator.PathInfo>();

    private Rigidbody rb;

    protected override void Start() {
        rb = GetComponent<Rigidbody>();
        base.Start();
    }

    private void OnCollisionEnter(Collision collision) {
        if(!collision.transform.CompareTag("Player")) return;
        
        moveables.Add(collision.rigidbody);
        start = true;
    }

    public override void FixedProcess(float deltaTime) {
        if (start) running = true;
        if (!start && !running) return;

        start = false;

        if (hasPath) {
            //continue
            var next = pathInfos.Peek().position + Vector3.up;
            var cartPosition = transform.position;
            
            var diff = (next - cartPosition).normalized;
            
            var moveAmount = diff.normalized * speed;
            var movePos = cartPosition + moveAmount;
            
            if (Vector3.Distance(movePos, next) < 0.5f) {
                pathInfos.Dequeue();
                return;
            }
            
            rb.MovePosition(movePos);
            transform.forward = Vector3.up;
        } else {
            foreach (var path in tracker.getPath()) {
                pathInfos.Enqueue(path);
            }
        }
    }
}
