using UnityEngine;

public class UpdateTrack : MonoBehaviour {
    private TrackGenerator tracker;
    private void Start() {
        tracker = GetComponentInParent<TrackGenerator>();
    }

    private void Update() {
        if (!transform.hasChanged) return;
        
        tracker.ShouldUpdate = true;
        transform.hasChanged = false;
    }
}
