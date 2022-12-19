using Rizing.Interface;

namespace Rizing.Developer.Command.Hidden {
    
    [ConsoleCommand("notfound", "command used when no command found", true)]
    public class NotFoundCommand : IConsoleCommand {
        public ConsoleOutput Execute(string[] args) {
            return new ConsoleOutput($"command {args[0]} not found, use /help to get all commands");
        }
    }
}