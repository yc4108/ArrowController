using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;


[CanEditMultipleObjects, CustomEditor(typeof(ArrowController))]
public class ArrowControllerEditor : Editor
{
    private ArrowController arrowController;
    private bool showNode;
    private ReorderableList nodeReorderList;

    private void OnEnable()
    {
        arrowController = (ArrowController)target;
        SerializedProperty nodeListProp = serializedObject.FindProperty("nodeList");
        nodeReorderList = new ReorderableList(serializedObject, serializedObject.FindProperty("nodeList"),true,true,true,true);
        //列表标题
        nodeReorderList.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "贝塞尔曲线节点", EditorStyles.boldLabel);
        };
        nodeReorderList.elementHeight = 90;
        //添加元素
        nodeReorderList.onAddCallback = (ReorderableList list) =>
        {
            var addNode = arrowController.AddNode();
            arrowController.nodeList.Add(addNode);
        };
        //删除元素
        nodeReorderList.onRemoveCallback = (ReorderableList list) =>
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            arrowController.DeleteNode();
            arrowController.RenameNode();
        };
        //绘制元素
        nodeReorderList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty item = nodeReorderList.serializedProperty.GetArrayElementAtIndex(index);
            rect.height -= 130;
            rect.x += 10;
            EditorGUI.PropertyField(rect, item, new GUIContent("Node" + index));

        };

    }
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        serializedObject.Update();
        EditorGUILayout.Space();
        nodeReorderList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("贝塞尔曲线节点", EditorStyles.boldLabel);
        //showNode = EditorGUILayout.Foldout(showNode, "Node");
        //if (showNode)
        //{
        //    arrowController.node.nodePos = EditorGUILayout.ObjectField("节点位置", arrowController.node.nodePos, typeof(RectTransform), true) as RectTransform;
        //    arrowController.node.ControledByMouse = EditorGUILayout.Toggle("节点由鼠标控制", arrowController.node.ControledByMouse);
        //    if (arrowController.node.ControledByMouse)
        //    {
        //        arrowController.node.mouseParm = EditorGUILayout.Vector2Field(new GUIContent("鼠标系数", "节点坐标为：鼠标xy坐标分别乘以鼠标系数的xy值"), arrowController.node.mouseParm);
        //        arrowController.node.offset = EditorGUILayout.Vector2Field("偏移", arrowController.node.offset);
        //    }
        //}

    }
}
