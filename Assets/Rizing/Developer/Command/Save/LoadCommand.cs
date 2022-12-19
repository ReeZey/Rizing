using Rizing.Core;
using Rizing.Interface;

namespace Rizing.Developer.Command.Save {
    [ConsoleCommand("load", "loads")]
    public class LoadCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            var instance = SaveSystem.Instance;
            
            if (instance == null) return new ConsoleOutput($"Could not find ${instance}...", LogPrefix.Error);
            if (args.Length < 2) return new ConsoleOutput("Syntax Error, Usage: load <savename>", LogPrefix.Warning); 
            
            string saveName = args[1];
            
            if (!instance.Exists(saveName)) {
                return new ConsoleOutput($"Save {saveName} was not found", LogPrefix.Warning);
            }
            
            instance.Load(saveName);
            return new ConsoleOutput($"Loaded save: {saveName}", LogPrefix.Info);
        }
    }
}