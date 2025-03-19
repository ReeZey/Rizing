using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rizing.Interface;
using UnityEditor.Overlays;
using UnityEngine;

namespace Rizing.Abstract {
    public class SaveableEntity : BaseEntity
    {
        public string id => ID;

        [SerializeField] private string ID = "unset";

        [ContextMenu("Generate new ID")]
        public string RegenerateGUID()
        {
            ID = Guid.NewGuid().ToString();
    
            //ConsoleProDebug.Watch("Last generated GUID: ", $"{gameObject.name}, new guid [{ID}]");
            return ID;
        }
    
        public object SaveState()
        {
            var stateDict = new Dictionary<string, object>();

            stateDict.Add("enabled", isActiveAndEnabled);

            foreach (var saveable in GetComponents<ISaveable>())
            {
                stateDict[saveable.GetType().FullName] = saveable.SaveState();
            }

            return stateDict;
        }

        public void LoadState(object state)
        {
            var stateDict = JObject.FromObject(state).ToObject<Dictionary<string, object>>();

            if (stateDict.TryGetValue("enabled", out object enabled))
            {
                gameObject.SetActive((bool)enabled);
            }

            foreach (var saveable in GetComponents<ISaveable>())
            {
                if (stateDict.TryGetValue(saveable.GetType().FullName, out object value))
                {
                    saveable.LoadState(value);
                }
            }
        }
    }
}