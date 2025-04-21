using Rizing.Interface;
using UnityEngine;

namespace Rizing.Developer.Command.Utility {
    
    [ConsoleCommand("quit", "quits the game")]
    public class QuitCommand : IConsoleCommand {
        
        public ConsoleOutput Execute(string[] args) {
            Application.Quit();
            return new ConsoleOutput();
        }
    }
}