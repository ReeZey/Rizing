using UnityEngine;

namespace Rizing.Other {
    public class DontDestroyThis : MonoBehaviour {
        private void Awake() {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
    }
}