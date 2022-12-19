namespace Rizing.Developer {
    public class ConsoleOutput {
        public readonly string Text;
        public readonly LogPrefix LogPrefix;
        
        public ConsoleOutput() {
            Text = "";
            LogPrefix = LogPrefix.None;
        }
        
        public ConsoleOutput(string text) {
            Text = text;
            LogPrefix = LogPrefix.None;
        }
        
        public ConsoleOutput(string text, LogPrefix logPrefix) {
            Text = text;
            LogPrefix = logPrefix;
        }

        public override string ToString() {
            return Text;
        }
    }
}