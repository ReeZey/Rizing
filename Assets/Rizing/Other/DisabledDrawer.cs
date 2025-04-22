#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Rizing.Other {
    [CustomPropertyDrawer(typeof(DisabledAttribute))]
    public class DisabledDrawer : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif