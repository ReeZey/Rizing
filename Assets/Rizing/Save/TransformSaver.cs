using UnityEngine;
using System;
using Rizing.Interface;

namespace Rizing.Save {
    public class TransformSaver : MonoBehaviour, ISaveable {
        [SerializeField] private bool _loadPosition;
        [SerializeField] private bool _loadScale;
        [SerializeField] private bool _loadRotation;
    
        public object SaveState()
        {
            var objectTransform = transform;
            return new SaveData
            {
                enabled = isActiveAndEnabled,
                position = objectTransform.position,
                scale = objectTransform.localScale,
                rotation = objectTransform.rotation
            };
        }

        public void LoadState(object inputData)
        {
            var saveData = (SaveData) inputData;
        
            gameObject.SetActive(saveData.enabled);
            
            var objectTransform = transform;
            if(_loadPosition) objectTransform.position = saveData.position;
            if(_loadScale) objectTransform.localScale = saveData.scale;
            if(_loadRotation) objectTransform.rotation = saveData.rotation;
        }
    
        [Serializable]
        private struct SaveData
        {
            public bool enabled;
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
        }
    }
}