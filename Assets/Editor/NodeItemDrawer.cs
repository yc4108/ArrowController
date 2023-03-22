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

            EditorGUI.PropertyField(nodePosRect, nodePosProperty, new GUIContent("节点位置"));
            EditorGUI.PropertyField(controledByMouseRect, controledByMouseProperty, new GUIContent("鼠标控制", "节点位置和鼠标位置相关联"));
            if (controledByMouseProperty.boolValue)
            {
                EditorGUI.PropertyField(mouseParmRect, mouseParmProperty, new GUIContent("鼠标系数", "节点坐标为：鼠标xy坐标分别乘以鼠标系数的xy值"));
                EditorGUI.PropertyField(offsetRect, offsetProperty, new GUIContent("偏移"));
            }
            else
            {
                
                EditorGUI.HelpBox(messageRect,"已禁用鼠标控制",MessageType.Info);
            }
        }
    }
}
