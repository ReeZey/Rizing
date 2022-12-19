using Rizing.Interface;

namespace Rizing.Developer.Command.Utility {
    
    [ConsoleCommand("ping", "PONG!")]
    public class PingCommand : IConsoleCommand {
        
        public ConsoleOutput Execute(string[] args) {
            return new ConsoleOutput("PONG!");
        }
    }
}