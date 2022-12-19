using Cinemachine;
using Rizing.Abstract;
using Rizing.Core;
using Rizing.Interface;
using UnityEngine;

namespace Rizing.Singletons {
    public class FPSCamera : SingletonMono<FPSCamera>, IEntity {
        
        private CinemachineVirtualCamera virtualCamera;
        private InputParser _inputParser;

        private void Start() {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

            _inputParser = InputParser.Instance;
            
            GameManager.Instance.AddEntity(this);
        }

        public void LateProcess(float deltaTime) {
            var direction = _inputParser.GetKey("Look").ReadValue<Vector2>();

            //Debug.Log(direction);
            
            var cameraTransform = virtualCamera.transform;
            
            var eulerAngles = cameraTransform.eulerAngles;
            
            eulerAngles += new Vector3(-(direction.y * Time.fixedDeltaTime * _inputParser.mouseSpeed), direction.x * Time.fixedDeltaTime * _inputParser.mouseSpeed, 0);
            
            if (eulerAngles.x > 180) eulerAngles.x -= 360;

            eulerAngles.x = Mathf.Clamp(eulerAngles.x, -89, 89);

            cameraTransform.eulerAngles = eulerAngles;
        }

        public void Play() {
            
        }

        public void Pause() {
            
        }

        public void Process(float deltaTime) {
            
        }

        public void FixedProcess(float deltaTime) {
            
        }
    }
}
