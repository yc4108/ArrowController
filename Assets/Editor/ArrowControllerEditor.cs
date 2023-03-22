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
        //�б����
        nodeReorderList.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "���������߽ڵ�", EditorStyles.boldLabel);
        };
        nodeReorderList.elementHeight = 90;
        //���Ԫ��
        nodeReorderList.onAddCallback = (ReorderableList list) =>
        {
            var addNode = arrowController.AddNode();
            arrowController.nodeList.Add(addNode);
        };
        //ɾ��Ԫ��
        nodeReorderList.onRemoveCallback = (ReorderableList list) =>
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            arrowController.DeleteNode();
            arrowController.RenameNode();
        };
        //����Ԫ��
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
        //EditorGUILayout.LabelField("���������߽ڵ�", EditorStyles.boldLabel);
        //showNode = EditorGUILayout.Foldout(showNode, "Node");
        //if (showNode)
        //{
        //    arrowController.node.nodePos = EditorGUILayout.ObjectField("�ڵ�λ��", arrowController.node.nodePos, typeof(RectTransform), true) as RectTransform;
        //    arrowController.node.ControledByMouse = EditorGUILayout.Toggle("�ڵ���������", arrowController.node.ControledByMouse);
        //    if (arrowController.node.ControledByMouse)
        //    {
        //        arrowController.node.mouseParm = EditorGUILayout.Vector2Field(new GUIContent("���ϵ��", "�ڵ�����Ϊ�����xy����ֱ�������ϵ����xyֵ"), arrowController.node.mouseParm);
        //        arrowController.node.offset = EditorGUILayout.Vector2Field("ƫ��", arrowController.node.offset);
        //    }
        //}

    }
}
