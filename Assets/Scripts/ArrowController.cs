using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    #region Property
    [Header("��ʼλ��")]
    public RectTransform rootPos;
    [Header("�յ�λ��")]
    public RectTransform endPos;
    [Header("�ڵ�λ��")]
    public GameObject nodeRoot;
    [Header("��ͼ��")]
    public Sprite sp;
    [Header("������������"), Range(0, 100)]
    public int objNum = 10;
    [Header("���ư�ť")]
    public Button btn;
    private bool isActive = false;//�Ƿ�������
    private List<GameObject> pointList = new List<GameObject>();//�ڵ��б�
    private GameObject lineRoot;//���߸��ڵ�
    private int previousNum;//��һ֡������
    private List<Vector3> nodeTransformLocalPos = new List<Vector3>();//�ڵ�λ���б�
    [HideInInspector]
    public List<Node> nodeList = new List<Node>(); //�����Editor�ﻭ

    #endregion
    #region MonoBehaviour functions
    void Start()
    {
        previousNum = objNum;                                                           //�����һ֡������������
        lineRoot = new GameObject("line",typeof(RectTransform));                        //����������line�ڵ���
        lineRoot.transform.SetParent(rootPos.transform.parent);
        lineRoot.transform.SetSiblingIndex(1);
        lineRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        btn.onClick.AddListener(delegate () {                                           //ע���¼�
            ChangeIsActive();
        });
        
    }

    void Update()
    {

        if (isActive)
        {
            UpdateNodePos();
            ArrowFollowMouse();
            CheckPointNum();
            UpdatePointsPos();
            
        }
        
    }



    //���ɵ�
    public void CreatPoints()
    {
        endPos.gameObject.SetActive(true);//��ʾ��ͷ
        for(int i=0; i < objNum; i++)
        {
            GameObject go=new GameObject("p"+i,typeof(Image));
            go.GetComponent<Image>().sprite = sp;
            go.transform.SetParent(lineRoot.transform);
            pointList.Add(go);
        }
        
    }
    //ɾ����
    public void DestroyPoints()
    {
        endPos.gameObject.SetActive(false);//���ؼ�ͷ
        for (int i = 0; i < pointList.Count; i++)
        {
            Destroy(pointList[i]);
            
        }
        pointList= new List<GameObject>();
    }
    //���µ�λ��
    public void UpdatePointsPos()
    {
        if (pointList.Count == 0)
        {
            return;
        }

        //��ڵ�λ��
        nodeTransformLocalPos = new List<Vector3>();
        nodeTransformLocalPos.Add(rootPos.localPosition);
        for (int i = 0; i < nodeList.Count; i++)
        {
            nodeTransformLocalPos.Add(nodeList[i].nodePos.localPosition);
        }
        nodeTransformLocalPos.Add(endPos.localPosition);
        for (int i = 0; i < pointList.Count; i++)
        {
            BezierNew(pointList[i],nodeTransformLocalPos ,(float)(i + 1) / (pointList.Count + 2));
        }

    }
    //��ť�¼�
    public void ChangeIsActive()
    {
        if (isActive)
        {
            //�رջ���
            isActive = false;
            DestroyPoints();
            btn.transform.GetChild(0).GetComponent<Text>().text = "��ʼ";
        }
        else
        {
            //��������
            isActive = true;
            CreatPoints();
            btn.transform.GetChild(0).GetComponent<Text>().text = "ֹͣ";
        }
    }

    //��ױ����������㷨
    public void BezierNew(GameObject go,List<Vector3> ndLst , float k)
    {
        if(ndLst.Count == 2)
        {
            Vector3 pos1 = ndLst[0];
            Vector3 pos2 = ndLst[1];
            go.transform.localPosition = Vector3.Lerp(pos1, pos2, k);
            //������ת
            var angle = Vector3.Angle(Vector3.up, pos2 - pos1);
            var normal = Vector3.Cross(Vector3.up, pos2 - pos1);
            angle *= Mathf.Sign(Vector3.Dot(normal, Vector3.forward));
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            List<Vector3> tmp = new List<Vector3>();
            for (int i = 0; i < ndLst.Count-1; i++)
            {
                Vector3 pos = Vector3.Lerp(ndLst[i], ndLst[i+1], k);
                tmp.Add(pos);
            }
            BezierNew(go, tmp, k);
        }
    }


    //��ͷ�������
    public void ArrowFollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = mousePos - new Vector2(rootPos.parent.transform.position.x, rootPos.parent.transform.position.y);
        endPos.transform.localPosition = new Vector3(targetPos.x,targetPos.y,0);
        //��ͷ��ת
        Vector3 prevNodePos = new Vector3();
        if (nodeList.Count == 0)//û�нڵ㻭ֱ��
        {
            prevNodePos = rootPos.localPosition;
        }
        else
        {
            prevNodePos = nodeList[nodeList.Count - 1].nodePos.localPosition;
        }

        var angle = Vector3.Angle(Vector3.up, endPos.transform.localPosition - prevNodePos);
        var normal = Vector3.Cross(Vector3.up, endPos.transform.localPosition - prevNodePos);
        angle *= Mathf.Sign(Vector3.Dot(normal, Vector3.forward));
        endPos.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    //����ʱ�ı���������
    public void CheckPointNum()
    {
        if (objNum == previousNum)
        {
            return;
        }
        else
        {
            OnNumChange();
        }
    }
    //���������ı�
    public void OnNumChange()
    {
        DestroyPoints();
        CreatPoints();
        //UpdatePointsPos();
        previousNum = objNum;
    }
    //���½ڵ�λ��
    public void UpdateNodePos()
    {
        if (nodeList.Count == 0) return;
        for(int i = 0; i < nodeList.Count; i++)
        {
            //����������
            if (nodeList[i].ControledByMouse)
            {
                var targetPos = new Vector3(0, 0, 0);
                targetPos.x = endPos.localPosition.x * nodeList[i].mouseParm.x + nodeList[i].offset.x;
                targetPos.y = endPos.localPosition.y * nodeList[i].mouseParm.y + nodeList[i].offset.y;
                nodeList[i].nodePos.localPosition = targetPos;
            }
        }

    }
    #endregion
    #region Interface Functions
    //��Ӻ��Զ������ڵ�
    public Node AddNode()
    {
        var addNode = new Node();
        GameObject go = new GameObject("p" + nodeList.Count, typeof(RectTransform));
        go.transform.SetParent(nodeRoot.transform);
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        addNode.nodePos = go.GetComponent<RectTransform>();
        return addNode;
    }
    //�����ɾ���ڵ�
    public void DeleteNode()
    {
        int childCount = nodeRoot.transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            var curNode = nodeRoot.transform.GetChild(i).gameObject;
            bool canDelete = true;
            for(int j = 0; j < nodeList.Count; j++)
            {
                if(nodeList[j].nodePos.gameObject == curNode)
                {
                    canDelete = false;
                }
            }

            if (canDelete)
            {
                DestroyImmediate(curNode);
                return;
            }
        }
    }
    //�������ڵ�
    public void RenameNode()
    {
        for (int j = 0; j < nodeList.Count; j++)
        {
            nodeList[j].nodePos.gameObject.name = "p" + j;

        }
    }
    #endregion
}


#region Class Define

//���������߽ڵ�
[Serializable]
public class Node
{
    [SerializeField]
    public RectTransform nodePos;
    [SerializeField]
    public bool ControledByMouse = true;
    [SerializeField]
    public Vector2 mouseParm = new Vector2((float)-0.5, 1);
    [SerializeField]
    public Vector2 offset = new Vector2(0, 60);
}
#endregion