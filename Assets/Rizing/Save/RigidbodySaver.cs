using System;
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
                velocity = rigid.velocity,
                angularVelocity = rigid.angularVelocity,
            };
        }

        public void LoadState(object inputData)
        {
            SaveData _saveData = (SaveData) inputData;
            
            if(_loadVelocity) rigid.velocity = _saveData.velocity;
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