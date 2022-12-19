using Rizing.Core;
using Rizing.Interface;

namespace Rizing.Developer.Command.Save {
    [ConsoleCommand("save", "saves")]
    public class SaveCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            var instance = SaveSystem.Instance;
            if (instance == null) return new ConsoleOutput($"Could not find ${instance}...", LogPrefix.Error);
            if (args.Length < 2) return new ConsoleOutput("Syntax Error, Usage: save <savename>", LogPrefix.Warning); 
            
            string saveName = args[1];
            
            instance.Save(saveName);
            return new ConsoleOutput($"Game saved: {saveName}");
        }
    }
}