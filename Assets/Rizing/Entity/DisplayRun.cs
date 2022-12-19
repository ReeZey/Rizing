using Rizing.Abstract;
using Rizing.Core;
using TMPro;
using UnityEngine;

namespace Rizing.Entity {
    public class DisplayRun : BaseEntity {

        [SerializeField] private TextMeshProUGUI textMesh;

        protected override void Start() {
            base.Start();
            textMesh.text = "State: " + GameManager.Instance.GetCurrentState();
        }
        
        public override void Play() {
            textMesh.text = "State: Running";
        }

        public override void Pause() {
            textMesh.text = "State: Paused";
        }
    }
}