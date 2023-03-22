using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Node))]
public class NodeItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position,label,property))
        {
            EditorGUIUtility.labelWidth = 60;
            position.height = EditorGUIUtility.singleLineHeight;

            Rect nodePosRect = new Rect(position)
            {
                width = position.width-10,
                x = position.x
                
            };
            Rect controledByMouseRect = new Rect(nodePosRect)
            {
                y = nodePosRect.y + EditorGUIUtility.singleLineHeight + 2
            };
            Rect messageRect = new Rect(controledByMouseRect)
            {
                height = EditorGUIUtility.singleLineHeight * 2 + 2,
                y = controledByMouseRect.y + EditorGUIUtility.singleLineHeight + 2
            };
            Rect mouseParmRect = new Rect(controledByMouseRect)
            {
                y = controledByMouseRect.y + EditorGUIUtility.singleLineHeight + 2
            };
            Rect offsetRect = new Rect(mouseParmRect)
            {
                y = mouseParmRect.y + EditorGUIUtility.singleLineHeight + 2
            };

            SerializedProperty nodePosProperty = property.FindPropertyRelative("nodePos");
            SerializedProperty controledByMouseProperty = property.FindPropertyRelative("ControledByMouse");
            SerializedProperty mouseParmProperty = property.FindPropertyRelative("mouseParm");
            SerializedProperty offsetProperty = property.FindPropertyRelative("offset");

            EditorGUI.PropertyField(nodePosRect, nodePosProperty, new GUIContent("�ڵ�λ��"));
            EditorGUI.PropertyField(controledByMouseRect, controledByMouseProperty, new GUIContent("������", "�ڵ�λ�ú����λ�������"));
            if (controledByMouseProperty.boolValue)
            {
                EditorGUI.PropertyField(mouseParmRect, mouseParmProperty, new GUIContent("���ϵ��", "�ڵ�����Ϊ�����xy����ֱ�������ϵ����xyֵ"));
                EditorGUI.PropertyField(offsetRect, offsetProperty, new GUIContent("ƫ��"));
            }
            else
            {
                
                EditorGUI.HelpBox(messageRect,"�ѽ���������",MessageType.Info);
            }
        }
    }
}
