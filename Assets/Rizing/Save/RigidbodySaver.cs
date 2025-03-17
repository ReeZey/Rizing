using System;
using Newtonsoft.Json.Linq;
using Rizing.Interface;
using UnityEngine;

namespace Rizing.Save {
    
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodySaver : MonoBehaviour, ISaveable {
        [SerializeField] private bool _loadVelocity;
        [SerializeField] private bool _loadAngularVelocity;

        private bool _load;
        private Rigidbody rigid;

        private void Start() {
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
            public Vector3 velocity;
            public Vector3 angularVelocity;
        }
    }
}