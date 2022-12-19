using System.Text;
using Rizing.Interface;

namespace Rizing.Developer.Command.Utility {
    [ConsoleCommand("echo", "echoes args you give it")]
    public class EchoCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            StringBuilder stringBuilder = new StringBuilder();
            for (var i = 1; i < args.Length; i++) {
                stringBuilder.Append($"{args[i]} ");
            }
            return new ConsoleOutput(stringBuilder.ToString());
        }
    }
}