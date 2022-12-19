using System;
using System.Collections.Generic;
using System.Reflection;
using Rizing.Interface;

namespace Rizing.Developer {
    public class Console {
        private Dictionary<string, ConsoleCommand> _commands = new Dictionary<string, ConsoleCommand>();

        public Console(){
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes()) {
                    if (!type.IsDefined(typeof(ConsoleCommandAttribute), false) || type.IsSubclassOf(typeof(IConsoleCommand))) continue;
                    
                    IConsoleCommand command = Activator.CreateInstance(type) as IConsoleCommand;
                    ConsoleCommandAttribute commandAttribute = type.GetCustomAttribute<ConsoleCommandAttribute>();

                    _commands.Add(commandAttribute.Command, new ConsoleCommand(commandAttribute, command));
                }
            }
        }

        public IConsoleCommand GetCommand(string str) {
            return _commands.ContainsKey(str) ? _commands[str].Command : _commands["notfound"].Command;
        }
        
        public Dictionary<string, ConsoleCommand> GetCommands() {
            return _commands;
        }
    }
}