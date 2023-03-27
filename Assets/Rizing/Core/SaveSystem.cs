using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Rizing.Abstract;
using Rizing.Developer;
using Rizing.Save;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

        private string BuildPath(string fileName) => $"{path}/{fileName}.bin";

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

            var state = LoadFile(str);
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
            var formatter = CustomBinaryFormatter.BinaryFormatter;
            
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, state);

            memoryStream.Position = 0;
            
            var compressor = new DeflateStream(stream, CompressionMode.Compress);
            memoryStream.CopyTo(compressor);

            compressor.Close();
            stream.Close();
        }

        private Dictionary<string, object> LoadFile(string str) {
            while (true) {
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

                var stream = File.Open(fullPath, FileMode.Open);
                var deflate = new DeflateStream(stream, CompressionMode.Decompress);
                var formatter = CustomBinaryFormatter.BinaryFormatter;

                var failed = false;
                Dictionary<string, object> dict = null;
                try {
                    dict = formatter.Deserialize(deflate) as Dictionary<string, object>;
                }
                catch (Exception e) {
                    Debug.LogError(e);
                    _developerConsole.LogToConsole(e.ToString(), LogPrefix.Error);
                    failed = true;
                }

                deflate.Close();
                stream.Close();

                if (!failed) return dict;

                File.Delete(fullPath);
                if (File.Exists(fullPath + ".bak")) File.Move(fullPath + ".bak", fullPath);
            }
        }

        private void SaveState(Dictionary<string, object> stateList) {
            foreach (var saveable in _gameManager.GetSaveables()) {
                if (saveable.id == "unset") {
                    _developerConsole.LogToConsole($"SAVE WARNING. saveable entity [{saveable.name}] is unset, developer issue", LogPrefix.Warning);
                    continue;
                }

                stateList[saveable.id] = saveable.SaveState();
            }
        }

        private void LoadState(IDictionary<string, object> stateList) {
            foreach (var saveable in _gameManager.GetSaveables()) {
                if (saveable.id == "unset") {
                    _developerConsole.LogToConsole($"LOAD WARNING. saveable entity [{saveable.name}] is unset, developer issue", LogPrefix.Warning);
                    continue;
                }

                if (stateList.TryGetValue(saveable.id, out object state)) {
                    saveable.LoadState(state);
                }
            }
        }
    }
}
