using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rizing.Save.Surrogates;
using UnityEngine;

namespace Rizing.Save {
    public class CustomBinaryFormatter
    {
        public static BinaryFormatter BinaryFormatter => GetBinaryFormatter();
    
        private static BinaryFormatter GetBinaryFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();
        
            SurrogateSelector selector = new SurrogateSelector();
        
            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Serilization());
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), new QuaternionSerilization());
            
            formatter.SurrogateSelector = selector;
        
            return formatter;
        }
    }
}
