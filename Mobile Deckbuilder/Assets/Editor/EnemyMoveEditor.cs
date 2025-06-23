/*using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyMove))]
public class EnemyMoveEditor : PropertyDrawer
{
    SerializedProperty MoveTypeProp;
    SerializedProperty CustomMoveProp;
    SerializedProperty DamageProp;
    SerializedProperty AmountOfAttacksProp;
    SerializedProperty DefenseAmountProp;




    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        MoveTypeProp = property.FindPropertyRelative("MoveType");
        CustomMoveProp = property.FindPropertyRelative("CustomMove");
        DamageProp = property.FindPropertyRelative("Damage");
        AmountOfAttacksProp = property.FindPropertyRelative("AmountOfAttacks");
        DefenseAmountProp = property.FindPropertyRelative("DefenseAmount");

        EditorGUILayout.PropertyField(MoveTypeProp);
        EditorGUILayout.PropertyField(CustomMoveProp);
        

        if (MoveTypeProp.enumValueIndex >= 0 && MoveTypeProp.enumValueIndex < System.Enum.GetValues(typeof(MoveType)).Length)
        {
            switch ((MoveType)MoveTypeProp.enumValueIndex)
            {
                case MoveType.Attack:
                    EditorGUILayout.LabelField("Attack Settings", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(DamageProp);
                    EditorGUILayout.PropertyField(AmountOfAttacksProp);
                    EditorGUI.indentLevel--;
                    break;
                case MoveType.Defend:
                    EditorGUILayout.LabelField("Defend Settings", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(DefenseAmountProp);
                    EditorGUI.indentLevel--;
                    break;
            }
        }

        EditorGUI.EndProperty();
    }



}*/
