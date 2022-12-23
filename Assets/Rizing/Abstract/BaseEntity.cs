using Rizing.Core;
using Rizing.Interface;
using UnityEngine;

namespace Rizing.Abstract {
    public abstract class BaseEntity : MonoBehaviour, IEntity {
        protected virtual void Start() {
            GameManager.Instance.AddEntity(this);
        }

        public virtual void Play() {
            
        }

        public virtual void Pause() {
            
        }

        public virtual void Process(float deltaTime) {
            
        }

        public virtual void FixedProcess(float deltaTime) {
            
        }

        public virtual void LateProcess(float deltaTime) {
            
        }

        public virtual void LateFixedProcess(float deltaTime) {
            
        }
    }
}