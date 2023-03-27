using System.Text;
using Rizing.Interface;

namespace Rizing.Developer.Command.Utility {
    [ConsoleCommand("clear", "clears shit")]
    public class ClearCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            return new ConsoleOutput { clear = true };
        }
    }
}