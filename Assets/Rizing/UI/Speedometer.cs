using Rizing.Abstract;
using TMPro;
using UnityEngine;

namespace Rizing.UI {
    public class Speedometer : BaseEntity {
        [SerializeField] private TextMeshProUGUI _textMesh;
        [SerializeField] private Rigidbody rigid;
    
        public override void Process(float deltaTime) {
            _textMesh.text = Mathf.RoundToInt(rigid.velocity.magnitude).ToString();
        }
    }
}
