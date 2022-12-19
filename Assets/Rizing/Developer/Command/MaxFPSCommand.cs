using Rizing.Core;
using Rizing.Interface;

namespace Rizing.Developer.Command {
    [ConsoleCommand("maxfps", "set max fps")]
    public class MaxFPSCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            var instance = GameManager.Instance;
            if (instance == null) return new ConsoleOutput($"Could not find ${typeof(GameManager)}...", LogPrefix.Error);
            if (args.Length < 2) return new ConsoleOutput($"Current Max framerate: {instance.lockFPS}", LogPrefix.Info);
            if (!int.TryParse(args[1], out int lockFPS)) return new ConsoleOutput($"Could not read number: [{args[1]}]", LogPrefix.Warning);
            
            instance.lockFPS = lockFPS;
            return new ConsoleOutput($"Max framerate updated to: {lockFPS}", LogPrefix.Info);
        }
    }
}