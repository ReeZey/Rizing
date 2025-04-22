using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Rizing.Interface;
using UnityEngine;
using Rizing.Other;
using Rizing.Save;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Rizing.Abstract {

    [RequireComponent(typeof(TransformSaver))]
    public class SaveableEntity : BaseEntity
    {
        public string id => ID;

        public bool SpawnableObject = true;
        
        [SerializeField]
        #if UNITY_EDITOR 
        [Disabled] 
        #endif 
        private string PrefabPath;
        
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

            if(SpawnableObject) {
                stateDict["prefab"] = PrefabPath;
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
            if(PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.Regular){
                return;
            }

            if (PrefabUtility.GetCorrespondingObjectFromSource(gameObject) != null) {
                return;
            }

            if(!SpawnableObject) {
                PrefabPath = "unspawnable object";
                return;
            }
            
            PrefabPath = AssetDatabase.GetAssetPath(gameObject);

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            if(settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(PrefabPath)) != null) {
                return;
            }

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(PrefabPath), settings.DefaultGroup);
            
            if(entry == null) {
                return;
            }
            
            Debug.Log($"Created Addressable Asset Entry: {gameObject.name}");
            entry.SetAddress(PrefabPath);
            entry.SetLabel("default", true, true);
        }
    }

#endif
    }
}