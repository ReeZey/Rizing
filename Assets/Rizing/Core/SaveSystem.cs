using System;
using System.Collections.Generic;
using System.IO;
using Rizing.Abstract;
using Rizing.Developer;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Collections;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Rizing.Core {

    [Serializable]
    public class SaveSystem : SingletonMono<SaveSystem> {
        public string currentSave = "save1";
        private string directoryPath => $"{Application.persistentDataPath}/Saves";
        public string path => savePath();
        
        private GameManager _gameManager;
        private InputParser _inputParser;
        private DeveloperConsole _developerConsole;
        
        private void Start() {
            _gameManager = GameManager.Instance;
            _inputParser = InputParser.Instance;
            _developerConsole = DeveloperConsole.Instance;
        }

        private void Update() {
            if (_inputParser.GetKey("Save").WasPressedThisFrame()) Save();
            if (_inputParser.GetKey("Load").WasPressedThisFrame()) Load();
        }

        private string savePath() {
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            return directoryPath;
        }

        private string BuildPath(string fileName) => $"{path}/{fileName}.json";

        [ContextMenu("generate all unset ids")]
        private void GenerateIds() {
            Debug.Log("Generating new ids...");
            
            if (_gameManager == null) _gameManager = GameManager.Instance;
            _gameManager.ReAddEntities();
            
            foreach (var saveableEntity in _gameManager.GetSaveables()) {
                if (saveableEntity.id != "unset") {
                    continue;
                }

#if UNITY_EDITOR
                Undo.RecordObject(saveableEntity, "update guid");
#endif
                
                saveableEntity.RegenerateGUID();
                Debug.Log("Updated GUID for " + saveableEntity);
            }

            Debug.Log("Generation done.");
        }



        [ContextMenu("save")]
        private void Save() {
            Save(currentSave);
        }

        [ContextMenu("load")]
        private void Load() {
            Load(currentSave);
        }

        public bool Exists(string str) {
            return File.Exists(BuildPath(str));
        }

        public void Save(string str) {
            if (!Application.isPlaying) {
                Debug.LogWarning("Currently only playmode saving is allowed");
                return;
            }
            
            _developerConsole.LogToConsole($"Saving [{str}]");

            var state = new Dictionary<string, object>();
            SaveState(state);
            SaveFile(state, str);
        }


        public void Load(string str) {
            if (!Application.isPlaying) {
                Debug.LogWarning("Currently only playmode saving is allowed");
                return;
            }
            
            _developerConsole.LogToConsole($"Loading [{str}]");
            
            var state = LoadFile(str);
            LoadState(state);
        }

        private void SaveFile(Dictionary<string, object> state, string str) {
            string pathToFile = BuildPath(str);

            if (File.Exists(pathToFile)) {
                File.Copy(pathToFile, pathToFile + ".bak", true);
            }

            var stream = File.Open(pathToFile, FileMode.Create);

            //var compressor = new DeflateStream(stream, CompressionMode.Compress);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(state, settings);
            stream.Write(System.Text.Encoding.UTF8.GetBytes(json), 0, json.Length);
            
            //compressor.Close();
            stream.Close();
        }

        private Dictionary<string, object> LoadFile(string str) {
            for(int i = 0; i < 2; i++) {
                string fullPath = BuildPath(str);

                bool normalSaveExist = File.Exists(fullPath);
                bool backupExist = File.Exists(fullPath + ".bak");

                if (!backupExist && !normalSaveExist) {
                    return new Dictionary<string, object>();
                }

                if (backupExist && !normalSaveExist) {
                    File.Move(fullPath + ".bak", fullPath);
                    continue;
                }

                Dictionary<string, object> dict = new Dictionary<string, object>();

                var stream = File.Open(fullPath, FileMode.Open);
                try {
                    //var deflate = new DeflateStream(stream, CompressionMode.Decompress);

                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Formatting = Formatting.Indented
                    };
                    dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(new StreamReader(stream).ReadToEnd(), settings);

                    //deflate.Close();
                    stream.Close();
                } catch (Exception e) {
                    stream.Close();
                    Debug.LogError("Error loading save file: " + e);

                    if (File.Exists(fullPath)) {
                        File.Delete(fullPath);
                        continue;
                    }
                }

                return dict;
            }

            return new Dictionary<string, object>();
        }

        private void SaveState(Dictionary<string, object> stateList) {
            foreach (SaveableEntity saveable in _gameManager.GetSaveables()) {
                if (saveable.id == "unset") {
                    _developerConsole.LogToConsole($"SAVE WARNING. saveable entity [{saveable.name}] is unset, developer issue", LogPrefix.Warning);
                    continue;
                }

                stateList[saveable.id] = saveable.SaveState();
            }
        }

        private void LoadState(IDictionary<string, object> stateList) {
            SaveableEntity[] all_objects = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);

            //Remove old objects
            foreach (SaveableEntity saveable_entity in all_objects) {
                if(!stateList.ContainsKey(saveable_entity.id)) {
                    Destroy(saveable_entity.gameObject);
                    continue;
                }
            }


            //Add new objects
            foreach (var (save_id, state) in stateList) {
                if (!all_objects.Any(x => x.id == save_id)) {
                    var stateDict = JObject.FromObject(state).ToObject<Dictionary<string, object>>();

                    if(stateDict.TryGetValue("prefab", out object prefab)) {
                        Addressables.InstantiateAsync(prefab, Vector3.zero, Quaternion.identity).Completed += handle => {
                            if (handle.Status == AsyncOperationStatus.Succeeded) {
                                SaveableEntity saveableEntity = handle.Result.GetComponent<SaveableEntity>();
                                saveableEntity.ForceUpdateID(save_id);
                                saveableEntity.LoadState(state);
                            } else {
                                _developerConsole.LogToConsole($"LOAD WARNING. saveable entity [{save_id}] prefab error", LogPrefix.Warning);
                            }
                        };
                        continue;
                    }

                    _developerConsole.LogToConsole($"LOAD WARNING. saveable entity [{save_id}] not found in scene, and is not a prefab", LogPrefix.Warning);
                    continue;
                }
            }

            //Load states
            foreach(var (save_id, state) in stateList){
                foreach (SaveableEntity saveable_entity in all_objects) {
                    if(saveable_entity.id != save_id) {
                        continue;
                    }

                    saveable_entity.LoadState(state);
                }
            }
        }
    }
}
