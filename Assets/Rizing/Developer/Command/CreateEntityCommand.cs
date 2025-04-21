using System.Linq;
using System.Text;
using Rizing.Abstract;
using Rizing.Core;
using Rizing.Interface;
using Rizing.Singletons;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Rizing.Developer.Command.Utility {
    [ConsoleCommand("create_entity", "create entity")]
    public class CreateEntityCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            if (args.Length != 2) {
                return new ConsoleOutput("Invalid number of arguments. Usage: create_entity <entity_name>");
            }

            Transform camera = FPSCamera.Instance.transform;
            var position = camera.position + camera.forward * 5;
            string prefab = args[1];

            Addressables.InstantiateAsync(prefab, position, camera.rotation).Completed += handle => {
                var _developerConsole = DeveloperConsole.Instance;

                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    _developerConsole.LogToConsole($"LOAD SUCCESS. Created [{prefab}]", LogPrefix.Info);

                    var found = handle.Result.TryGetComponent<SaveableEntity>(out var saveable);
                    if(found) {
                        saveable.RegenerateGUID();
                    }
                } else {
                    _developerConsole.LogToConsole($"LOAD ERROR:", LogPrefix.Error);
                    _developerConsole.LogToConsole(handle.OperationException.ToString(), LogPrefix.None);
                }
            };
            return new ConsoleOutput();
        }
    }
}