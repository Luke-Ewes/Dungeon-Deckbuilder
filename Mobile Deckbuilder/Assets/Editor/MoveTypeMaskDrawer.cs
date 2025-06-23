// Editor/CardTypeMaskDrawer.cs
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MoveType))]
public class MoveTypeMaskDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the enum value
        property.intValue = (int)(MoveType)EditorGUI.EnumFlagsField(position, label, (MoveType)property.intValue);
    }
}
