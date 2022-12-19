using Rizing.Interface;

namespace Rizing.Developer {
    public class ConsoleCommand {
        public readonly ConsoleCommandAttribute Attribute;
        public readonly IConsoleCommand Command;

        public ConsoleCommand(ConsoleCommandAttribute Attribute, IConsoleCommand Command) {
            this.Attribute = Attribute;
            this.Command = Command;
        }
    }
}