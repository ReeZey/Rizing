using Rizing.Abstract;
using UnityEngine;

namespace Rizing.Entity {
    public class PauseEntity : BaseEntity {
        [SerializeField] private bool flip;
        
        public override void Play() {
            gameObject.SetActive(flip);
        }

        public override void Pause() {
            gameObject.SetActive(!flip);
        }
    }
}
