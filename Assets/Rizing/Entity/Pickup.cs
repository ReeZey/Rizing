using System;
using System.Linq;
using Rizing.Abstract;
using Rizing.Core;
using Rizing.Interface;
using Rizing.Singletons;
using UnityEngine;

namespace Rizing.Entity {
    
    [RequireComponent(typeof(SaveableEntity))]
    public class Pickup : BaseEntity, ISaveable {
    
        private InputParser _inputParser;
        private FPSCamera _fpsCamera;
        
        private Transform _prevParent;
        private Transform _pickupTransform;
        
        private Rigidbody _pickupRigidbody;
        private string _pickupGUID;
        private bool _holding;
        
        private bool _togglePickup;
        private float _objectDistance;

        

        protected override void Start() {
            _inputParser = InputParser.Instance;
            _fpsCamera = FPSCamera.Instance;

            base.Start();
        }

        public override void Process(float deltaTime) {
            if (_inputParser.GetKey("Pickup").WasPressedThisFrame()) {
                if (_holding) {
                    DropItem();
                    return;
                }
                
                var cameraTransform = _fpsCamera.transform;
                bool hit = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var raycastHit, 5);

                if (!hit) return;
                if (!raycastHit.collider.CompareTag("Pickupable")) return;

                _pickupTransform = raycastHit.transform;
                _prevParent = _pickupTransform.parent;
                _pickupTransform.parent = _fpsCamera.transform;
                
                raycastHit.rigidbody.useGravity = false;
                _pickupRigidbody = raycastHit.rigidbody;
                _pickupRigidbody.angularDrag = 2;

                _objectDistance = (transform.position - _pickupTransform.position).magnitude;//raycastHit.distance;
                _pickupGUID = _pickupTransform.TryGetComponent<SaveableEntity>(out var saveableEntity) ? saveableEntity.id : null;
                
                _holding = true;
            }

            if (_holding && _inputParser.GetKey("Fire1").WasPressedThisFrame()) {
                DropItem();
                
                _pickupRigidbody.AddForce(_fpsCamera.transform.forward * 100, ForceMode.Impulse);
            }
        }

        public override void FixedProcess(float deltaTime) {
            if (!_holding) return;
            
            var cameraTransform = _fpsCamera.transform;
            Vector3 pos = cameraTransform.forward * Mathf.Max(2, _objectDistance);
            Vector3 wantedPos = cameraTransform.position + pos - _pickupTransform.position;
        
            if (wantedPos.magnitude > 1) {
                DropItem();
                Debug.Log("whoopsie i dropped the soap");
            }
            
            _pickupRigidbody.velocity = wantedPos * 10;
        }

        private void OnDrawGizmosSelected() {
            if (!Application.isPlaying) return;
        
            Gizmos.color = Color.magenta;
        
            var cameraTransform = _fpsCamera.transform;
            var cameraPosition = cameraTransform.position;
            Gizmos.DrawLine(cameraPosition, cameraPosition + cameraTransform.forward * 10);
        }

        private void DropItem() {
            if (!_holding) return;
            _holding = false;

            _pickupTransform.parent = _prevParent;
            
            if (_pickupRigidbody == null) return;

            _pickupRigidbody.velocity = GetComponent<Rigidbody>().velocity;
            
            _pickupRigidbody.useGravity = true;
            _pickupRigidbody.angularDrag = 0.05f;
        }

        public object SaveState()
        {
            return new SaveData
            {
                holding = _holding,
                pickupGUID = _pickupGUID,
                objectDistance = _objectDistance
            };
        }

        public void LoadState(object inputData)
        {
            var saveData = (SaveData) inputData;

            _holding = saveData.holding;
            
            if (!_holding) return;
            
            _pickupGUID = saveData.pickupGUID;
            
            _pickupRigidbody = FindObjectsOfType<SaveableEntity>()
                .FirstOrDefault(entity => entity.id == _pickupGUID)
                ?.GetComponent<Rigidbody>();
            
            _objectDistance = saveData.objectDistance;
        }
    
        [Serializable]
        private struct SaveData
        {
            public bool holding;
            public string pickupGUID;
            public float objectDistance;
        }
    }
}
