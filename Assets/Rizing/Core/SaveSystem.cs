using System;
using System.Collections.Generic;
using System.IO;
using Rizing.Abstract;
using Rizing.Save;
using UnityEditor;
using UnityEngine;

namespace Rizing.Core {

    [Serializable]
    public class SaveSystem : SingletonMono<SaveSystem> {
        public string currentSave = "save1";
        private string directoryPath => $"{Application.persistentDataPath}/Saves";
        public string path => savePath();
        
        private GameManager _gameManager;
        private InputParser _inputParser;
        
        private void Start() {
            _gameManager = GameManager.Instance;
            _inputParser = InputParser.Instance;
        }

        private void Update() {
            if (_inputParser.GetKey("Save").WasPressedThisFrame()) Save();
            if (_inputParser.GetKey("Load").WasPressedThisFrame()) Load();
        }

        private string savePath() {
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            return directoryPath;
        }

        private string BuildPath(string fileName) => $"{path}/{fileName}.pog";

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
            Debug.Log($"Saving [{str}]");

            var state = LoadFile(str);
            SaveState(state);
            SaveFile(state, str);
        }


        public void Load(string str) {
            Debug.Log($"Loading [{str}]");
            
            var state = LoadFile(str);
            LoadState(state);
        }

        private void SaveFile(Dictionary<string, object> state, string str) {
            string pathToFile = BuildPath(str);

            if (File.Exists(pathToFile)) {
                File.Copy(pathToFile, pathToFile + ".bak", true);
            }

            var stream = File.Open(pathToFile, FileMode.Create);
            //var deflate = new DeflateStream(stream, CompressionMode.Compress);

            var formatter = CustomBinaryFormatter.BinaryFormatter;
            
            try {
                formatter.Serialize(stream, state);
            }
            catch (Exception e) {
                Debug.LogError(e);

                stream.Close();

                File.Delete(pathToFile);
                File.Move(pathToFile + ".bak", pathToFile);
            }

            stream.Close();
        }

        private Dictionary<string, object> LoadFile(string str) {
            string fullPath = BuildPath(str);

            bool backupexists = File.Exists(fullPath + ".bak");
            bool normalsaveexisst = File.Exists(fullPath);

            if (backupexists && !normalsaveexisst) {
                File.Copy(fullPath + ".bak", fullPath);
            } else if (!backupexists && !normalsaveexisst) {
                return new Dictionary<string, object>();
            }

            var stream = File.Open(fullPath, FileMode.Open);
            //var deflate = new DeflateStream(stream, CompressionMode.Decompress);

            var formatter = CustomBinaryFormatter.BinaryFormatter;

            var dict = new Dictionary<string, object>();

            try {   
                dict = (Dictionary<string, object>) formatter.Deserialize(stream);
            }
            catch (Exception e) {
                Debug.LogError(e);

                stream.Close();

                File.Delete(fullPath);

                if (!File.Exists(fullPath + ".bak")) return dict;

                File.Move(fullPath + ".bak", fullPath);
                LoadFile(str);
            }
            stream.Close();

            return dict;
        }

        private void SaveState(IDictionary<string, object> stateList) {
            foreach (var saveable in _gameManager.GetSaveables()) {
                if (saveable.id == "unset") {
                    Debug.LogWarning($"SAVE WARNING. saveable entity [{saveable.name}] is unset");
                    continue;
                }

                stateList[saveable.id] = saveable.SaveState();
            }
        }

        private void LoadState(IDictionary<string, object> stateList) {
            foreach (var saveable in _gameManager.GetSaveables()) {
                if (saveable.id == "unset") {
                    Debug.LogWarning($"LOAD WARNING. saveable entity [{saveable.name}] is unset");
                    continue;
                }

                if (stateList.TryGetValue(saveable.id, out var state)) {
                    saveable.LoadState(state);
                }
            }
        }
    }
}
