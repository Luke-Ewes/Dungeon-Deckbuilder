using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Effect))]
public class EffectCDrawer : PropertyDrawer
{
    private Editor effectEditor;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var effectProp = property.FindPropertyRelative("effect");

        // Label and object field
        position.height = EditorGUIUtility.singleLineHeight;

        ScriptableObject oldEffect = effectProp.objectReferenceValue as ScriptableObject;

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, effectProp, label);
        if (EditorGUI.EndChangeCheck())
        {
            ScriptableObject newEffect = effectProp.objectReferenceValue as ScriptableObject;

            // Case 1: A new effect was assigned — clone it
            if (newEffect != null && newEffect != oldEffect && !newEffect.name.Contains("_Instance"))
            {
                var clone = Object.Instantiate(newEffect);
                clone.name = newEffect.name + "_Instance";
                clone.hideFlags = HideFlags.HideInHierarchy;

                if (!Application.isPlaying)
                {
                    var targetObject = property.serializedObject.targetObject;
                    AssetDatabase.AddObjectToAsset(clone, targetObject);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clone));
                }

                effectProp.objectReferenceValue = clone;
            }

            // Case 2: The effect was cleared — destroy the old one
            if (newEffect == null && oldEffect != null && oldEffect.name.EndsWith("_Instance"))
            {
                string path = AssetDatabase.GetAssetPath(oldEffect);
                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.RemoveObjectFromAsset(oldEffect);
                    Object.DestroyImmediate(oldEffect, true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    Object.DestroyImmediate(oldEffect);
                }
            }
        }

        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (effectProp.objectReferenceValue != null)
        {
            if (effectEditor == null || effectEditor.target != effectProp.objectReferenceValue)
            {
                if (effectEditor != null)
                    Object.DestroyImmediate(effectEditor);
                effectEditor = Editor.CreateEditor(effectProp.objectReferenceValue);
            }

            if (effectEditor != null)
            {
                EditorGUI.indentLevel++;
                effectEditor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var effectProp = property.FindPropertyRelative("effect");

        float height = EditorGUIUtility.singleLineHeight;

        if (effectProp.objectReferenceValue != null)
        {
            if (effectEditor == null || effectEditor.target != effectProp.objectReferenceValue)
            {
                if (effectEditor != null)
                    Object.DestroyImmediate(effectEditor);
                effectEditor = Editor.CreateEditor(effectProp.objectReferenceValue);
            }

            if (effectEditor != null)
            {
                height += GetEditorHeight(effectEditor) + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        return height;
    }

    private float GetEditorHeight(Editor editor)
    {
        editor.serializedObject.Update();

        float height = 0f;
        SerializedProperty prop = editor.serializedObject.GetIterator();

        if (prop.NextVisible(true))
        {
            while (prop.propertyPath == "m_Script" && prop.NextVisible(false)) { }

            while (prop != null)
            {
                height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
                if (!prop.NextVisible(false)) break;
            }
        }

        return height;
    }
}
