using System.Globalization;
using Rizing.Core;
using Rizing.Interface;

namespace Rizing.Developer.Command {
    
    [ConsoleCommand("sensitivity", "changes the mouse sensitivity")]
    public class SensitivityCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            var instance = InputParser.Instance;
            if (instance == null) return new ConsoleOutput("Could not find InputParser...", LogPrefix.Error);
            if (args.Length < 2) return new ConsoleOutput($"Current Sensitivity: {instance.mouseSpeed}", LogPrefix.Info);
            if (!float.TryParse(args[1], NumberStyles.Any, new CultureInfo("en-US"), out float mouseSpeed)) return new ConsoleOutput($"Could not read number: [{args[1]}]", LogPrefix.Warning);
            
            instance.mouseSpeed = mouseSpeed;
            return new ConsoleOutput($"Sensitivity updated to: {mouseSpeed}", LogPrefix.Info);
        }
    }
}