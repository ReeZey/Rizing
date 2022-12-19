using Rizing.Developer;

namespace Rizing.Interface {
    public interface IConsoleCommand {
        ConsoleOutput Execute(string[] args);
    }
}