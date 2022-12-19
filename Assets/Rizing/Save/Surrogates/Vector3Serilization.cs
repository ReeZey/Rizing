using System.Runtime.Serialization;
using UnityEngine;

namespace Rizing.Save.Surrogates {
    public class Vector3Serilization : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            Vector3 vec = (Vector3) obj;
            info.AddValue("x", vec.x);
            info.AddValue("y", vec.y);
            info.AddValue("z", vec.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector) {
            Vector3 vec = (Vector3) obj;
            vec.x = info.GetSingle("x");
            vec.y = info.GetSingle("y");
            vec.z = info.GetSingle("z");
            return vec;
        }
    }
}