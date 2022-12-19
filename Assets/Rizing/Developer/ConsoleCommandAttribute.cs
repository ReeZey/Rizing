using System;

namespace Rizing.Developer {
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ConsoleCommandAttribute : Attribute {
        public readonly string Command;
        public readonly string Description;
        public readonly bool Hidden;

        public ConsoleCommandAttribute(string Command, string Description)
        {
            this.Command = Command;
            this.Description = Description;
            
            Hidden = false;
        }
        
        public ConsoleCommandAttribute(string Command, string Description, bool Hidden)
        {
            this.Command = Command;
            this.Description = Description;
            this.Hidden = Hidden;
        }
    }
}