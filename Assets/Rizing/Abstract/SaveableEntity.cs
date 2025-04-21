using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Rizing.Interface;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Rizing.Abstract {
    public class SaveableEntity : BaseEntity
    {
        public string id => ID;

        public bool spawnable_object = true;
        [SerializeField] private string prefab_path;

        [SerializeField] private string ID = "unset";

        [ContextMenu("Generate new ID")]
        public string RegenerateGUID()
        {
            ID = Guid.NewGuid().ToString();
    
            //ConsoleProDebug.Watch("Last generated GUID: ", $"{gameObject.name}, new guid [{ID}]");
            return ID;
        }

        // this method should never be called except for creating a new entity
        public void ForceUpdateID(string id)
        {
            ID = id;
        }
    
        public object SaveState()
        {
            var stateDict = new Dictionary<string, object> {
                { "enabled", isActiveAndEnabled }
            };

            if(spawnable_object) {
                stateDict["prefab"] = prefab_path;
            }

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

#if UNITY_EDITOR

    private void OnValidate ()
    {
        if (!Application.isPlaying)
        {
            if(!spawnable_object) {
                prefab_path = "unspawnable object";
                return;
            }

            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject) ?? gameObject;
            prefab_path = AssetDatabase.GetAssetPath(prefab);

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            if(settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(prefab_path)) == null) {
                return;
            }

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(prefab_path), settings.DefaultGroup);
            entry.SetAddress(prefab_path);
        }
    }

#endif
    }
}