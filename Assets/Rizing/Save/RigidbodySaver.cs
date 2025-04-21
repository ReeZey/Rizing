using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rizing.Abstract;
using Rizing.Interface;
using UnityEngine;

namespace Rizing.Save {
    
    [RequireComponent(typeof(Rigidbody), typeof(SaveableEntity))]
    public class RigidbodySaver : MonoBehaviour, ISaveable {
        [SerializeField] private bool _loadVelocity = true;
        [SerializeField] private bool _loadAngularVelocity = true;

        private bool _load;
        private Rigidbody rigid;

        private void Awake() {
            rigid = GetComponent<Rigidbody>();
        }

        public object SaveState()
        {
            return new SaveData
            {
                velocity = rigid.linearVelocity,
                angularVelocity = rigid.angularVelocity,
            };
        }

        public void LoadState(object inputData)
        {
            SaveData _saveData = JObject.FromObject(inputData).ToObject<SaveData>();
            
            if(_loadVelocity) rigid.linearVelocity = _saveData.velocity;
            if(_loadAngularVelocity) rigid.angularVelocity = _saveData.angularVelocity;
        }

        [Serializable]
        private struct SaveData
        {
            [JsonConverter(typeof(smolVector3))]
            public Vector3 velocity;

            [JsonConverter(typeof(smolVector3))]
            public Vector3 angularVelocity;
        }
    }
}