using System.Runtime.Serialization;
using UnityEngine;

namespace Rizing.Save.Surrogates {
    public class QuaternionSerilization : ISerializationSurrogate {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
            Quaternion quat = (Quaternion) obj;
            info.AddValue("x", quat.x);
            info.AddValue("y", quat.y);
            info.AddValue("z", quat.z);
            info.AddValue("w", quat.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector) {
            Quaternion quat = (Quaternion) obj;
            quat.x = info.GetSingle("x");
            quat.y = info.GetSingle("y");
            quat.z = info.GetSingle("z");
            quat.w = info.GetSingle("w");
            return quat;
        }
    }
}