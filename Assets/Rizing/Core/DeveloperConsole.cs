using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Rizing.Abstract;
using Rizing.Developer;
using Rizing.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Console = Rizing.Developer.Console;

namespace Rizing.Core 
{
    public class DeveloperConsole : SingletonMono<DeveloperConsole>, IEntity {
        private readonly Console _console = new Console();

        private InputParser _inputParser;
        [SerializeField] private InputField _inputBox;
        [SerializeField] private TextMeshProUGUI _outputBox;
        [SerializeField] private ScrollRect _scrollRect;

        private const string prefix = "[RizingConsole]";

        private void ExecuteCommand() {
            if (_inputBox.text.Length == 0) return;
            
            LogToConsole("> " + _inputBox.text);
            
            foreach (string commands in _inputBox.text.Split(';')) {
                string[] split = commands.TrimStart().Split();

                var command = _console.GetCommand(split[0]);
                var output = command.Execute(split);
            
                if (output.Text.Length == 0) return;
            
                LogToConsole(output);
            }
        }

        public IEnumerable<ConsoleCommand> GetCommands() {
            return _console.GetCommands().Values;
        }

        private void Start() {
            GameManager.Instance.AddEntity(this);
            _inputParser = InputParser.Instance;
        }

        private void Update() {
            if (!_inputParser.GetKey("Submit").WasPressedThisFrame()) return;
            
            ExecuteCommand();
            _inputBox.text = "";
            _inputBox.ActivateInputField();
        }

        public void LogToConsole(string str) {
            LogToConsole(new ConsoleOutput(str));
        }

        private void LogToConsole(string str, LogPrefix logPrefix) {
            LogToConsole(new ConsoleOutput(str, logPrefix));
        }
        
        private void LogToConsole(ConsoleOutput output) {
            StringBuilder builder = new StringBuilder("\n");
            
            switch (output.LogPrefix) {
                case LogPrefix.None:
                    Debug.Log(output.Text);
                    break;
                case LogPrefix.Info:
                    builder.Append("[Info] ");
                    Debug.Log($"{prefix} [Info] {output.Text}");
                    break;
                case LogPrefix.Warning:
                    builder.Append("[WARNING] ");
                    Debug.LogWarning($"{prefix} [Warning] {output.Text}");
                    break;
                case LogPrefix.Error:
                    builder.Append("[ERROR] ");
                    Debug.LogError($"{prefix} [Error] {output.Text}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            builder.Append(output.Text);
            
            _outputBox.text += builder.ToString();
            
            StopCoroutine(ScrollDown());
            
            if(Math.Abs(_scrollRect.verticalScrollbar.value) < 0.1f) StartCoroutine(ScrollDown());
        }

        private IEnumerator ScrollDown() {
            yield return new WaitForSeconds(0.1f);
            _scrollRect.verticalScrollbar.value = 0;
        }

        public void Play() {
            gameObject.SetActive(false);
        }

        public void Pause() {
            gameObject.SetActive(true);
            _inputBox.ActivateInputField();
        }

        public void Process(float deltaTime) {
            
        }

        public void FixedProcess(float deltaTime) {
            
        }

        public void LateProcess(float deltaTime) {
            
        }
    }
}