using System.Collections.Generic;
using Rizing.Core;
using Rizing.Interface;

namespace Rizing.Developer.Command.Utility {
    [ConsoleCommand("help", "this command")]
    public class HelpCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            var commands = new List<string>();
            foreach (ConsoleCommand VARIABLE in DeveloperConsole.Instance.GetCommands()) {
                if(!VARIABLE.Attribute.Hidden) commands.Add($"{VARIABLE.Attribute.Command}, {VARIABLE.Attribute.Description}");
            }
            return new ConsoleOutput(string.Join("\n", commands), LogPrefix.None);
        }
    }
}