using System;
using Rizing.Abstract;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rizing.Core
{
    public class InputParser : SingletonMono<InputParser>
    {
        [SerializeField] private PlayerInput playerInput;

        [Range(0.1f, 20f)] public float mouseSpeed = 10f;

        public InputAction GetKey(string key) => playerInput.actions[key];
        
        private void Rebind(InputAction action) {
            if(action.enabled) action.Disable();
            var rebindOperation = action
                .PerformInteractiveRebinding()
                .WithExpectedControlType("axis")
                .Start();

            rebindOperation.OnComplete(operation => {
                action.Enable();
                rebindOperation.Dispose();
            });
        }
    }
}
