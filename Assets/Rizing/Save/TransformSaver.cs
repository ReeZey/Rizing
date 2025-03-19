using UnityEngine;
using System;
using Rizing.Interface;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Rizing.Abstract;

namespace Rizing.Save {
    
    [RequireComponent(typeof(SaveableEntity))]
    public class TransformSaver : MonoBehaviour, ISaveable {
        [SerializeField] private bool _loadPosition = true;
        [SerializeField] private bool _loadScale = true;
        [SerializeField] private bool _loadRotation = true;
    
        public object SaveState()
        {
            var objectTransform = transform;
            return new SaveData
            {
                position = objectTransform.position,
                scale = objectTransform.localScale,
                rotation = objectTransform.rotation
            };
        }

        public void LoadState(object inputData)
        {
            var saveData = JObject.FromObject(inputData).ToObject<SaveData>();
            
            var objectTransform = transform;
            if(_loadPosition) objectTransform.position = saveData.position;
            if(_loadScale) objectTransform.localScale = saveData.scale;
            if(_loadRotation) objectTransform.rotation = saveData.rotation;
        }
    
        [Serializable]
        private struct SaveData
        {

            [JsonConverter(typeof(smolVector3))]
            public Vector3 position;
            
            [JsonConverter(typeof(smolVector3))]
            public Vector3 scale;

            [JsonConverter(typeof(smolQuaternion))]
            public Quaternion rotation;
        }
    }
}