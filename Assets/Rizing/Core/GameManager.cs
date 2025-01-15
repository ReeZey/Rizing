using System;
using System.Collections.Generic;
using System.Linq;
using Rizing.Abstract;
using Rizing.Interface;
using UnityEngine;

namespace Rizing.Core
{
    public class GameManager : SingletonMono<GameManager> {
        
        private GameState _currentState = GameState.Paused;
        private GameState lastState = GameState.Running;
        
        private InputParser _inputParser;
        
        private readonly List<IEntity> entities = new();

        private readonly List<IEntity> adding = new();
        private readonly List<IEntity> removing = new();


        [Range(0, 1000)] public int lockFPS;

        public void AddEntity(IEntity entity) {
            if (entities.Contains(entity) || adding.Contains(entity)) return;
            
            if (removing.Contains(entity)) {
                removing.Remove(entity);
                return;
            }

            adding.Add(entity);
        }

        public void RemoveEntity(IEntity entity) {
            if (entities.Contains(entity) && removing.Contains(entity)) return;

            if (adding.Contains(entity)) {
                adding.Remove(entity);
                return;
            }

            removing.Add(entity);
        }

        public GameState GetCurrentState() {
            return _currentState;
        }

        protected override void Awake() {
            base.Awake();

            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.timeScale * 0.0078125f;
            QualitySettings.vSyncCount = 0;
        }

        
        private void OnEnable() {
            ReAddEntities();
        }
        
        public void ReAddEntities() {
            entities.Clear();
            entities.AddRange(FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None).OfType<IEntity>());
        }

        private void Start() {
            _inputParser = InputParser.Instance;
        }
        

        private void Update() {
            //Debug.Log(entities.Count);


            Application.targetFrameRate = lockFPS == 0 ? -1 : lockFPS;
            
            while (adding.Count > 0) {
                entities.Add(adding[0]);
                adding.RemoveAt(0);
            }
            
            if (_inputParser.GetKey("Pause").WasPressedThisFrame()) {
                if (_currentState == GameState.Paused) {
                    Play();
                } else {
                    Pause();
                }
            }

            switch (_currentState) {
                case GameState.Running:
                    
                    float time = Time.deltaTime;
                    foreach (var entity in entities) {
                        entity.Process(time);
                    }
                    
                    break;
                case GameState.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FixedUpdate() {
            switch (_currentState) {
                case GameState.Running:
                    Physics.Simulate(Time.fixedDeltaTime);

                    float time = Time.fixedDeltaTime;
                    foreach (var entity in entities) {
                        entity.FixedProcess(time);
                    }
                    
                    foreach (var entity in entities) {
                        entity.LateFixedProcess(time);
                    }

                    break;
                case GameState.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LateUpdate() {
            switch (_currentState) {
                case GameState.Running:
                    float time = Time.deltaTime;
                    foreach (var entity in entities) {
                        entity.LateProcess(time);
                    }

                    break;
                case GameState.Paused:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            while (removing.Count > 0) {
                entities.Remove(removing[0]);
                removing.RemoveAt(0);
            }
        }

        private void Play() {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _currentState = lastState;

            foreach (var entity in entities) {
                entity.Play();
            }
        }
        
        private void Pause() {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            lastState = _currentState;
            _currentState = GameState.Paused;
            
            foreach (var entity in entities) {
                entity.Pause();
            }
        }

        public IEnumerable<SaveableEntity> GetSaveables() {
            if (entities.Count == 0) return Enumerable.Empty<SaveableEntity>();
            
            return entities.FindAll(entity => entity is SaveableEntity).Cast<SaveableEntity>();
        }

        public enum GameState {
            Paused,
            Running
        }
    }
}