using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    #region Property
    [Header("起始位置")]
    public RectTransform rootPos;
    [Header("终点位置")]
    public RectTransform endPos;
    [Header("节点位置")]
    public GameObject nodeRoot;
    [Header("点图案")]
    public Sprite sp;
    [Header("画线物体数量"), Range(0, 100)]
    public int objNum = 10;
    [Header("控制按钮")]
    public Button btn;
    private bool isActive = false;//是否开启画线
    private List<GameObject> pointList = new List<GameObject>();//节点列表
    private GameObject lineRoot;//画线根节点
    private int previousNum;//上一帧点数量
    private List<Vector3> nodeTransformLocalPos = new List<Vector3>();//节点位置列表
    [HideInInspector]
    public List<Node> nodeList = new List<Node>(); //这个在Editor里画

    #endregion
    #region MonoBehaviour functions
    void Start()
    {
        previousNum = objNum;                                                           //存放上一帧画线物体数量
        lineRoot = new GameObject("line",typeof(RectTransform));                        //画线物体在line节点下
        lineRoot.transform.SetParent(rootPos.transform.parent);
        lineRoot.transform.SetSiblingIndex(1);
        lineRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        btn.onClick.AddListener(delegate () {                                           //注册事件
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



    //生成点
    public void CreatPoints()
    {
        endPos.gameObject.SetActive(true);//显示箭头
        for(int i=0; i < objNum; i++)
        {
            GameObject go=new GameObject("p"+i,typeof(Image));
            go.GetComponent<Image>().sprite = sp;
            go.transform.SetParent(lineRoot.transform);
            pointList.Add(go);
        }
        
    }
    //删除点
    public void DestroyPoints()
    {
        endPos.gameObject.SetActive(false);//隐藏箭头
        for (int i = 0; i < pointList.Count; i++)
        {
            Destroy(pointList[i]);
            
        }
        pointList= new List<GameObject>();
    }
    //更新点位置
    public void UpdatePointsPos()
    {
        if (pointList.Count == 0)
        {
            return;
        }

        //存节点位置
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
    //按钮事件
    public void ChangeIsActive()
    {
        if (isActive)
        {
            //关闭画线
            isActive = false;
            DestroyPoints();
            btn.transform.GetChild(0).GetComponent<Text>().text = "开始";
        }
        else
        {
            //开启画线
            isActive = true;
            CreatPoints();
            btn.transform.GetChild(0).GetComponent<Text>().text = "停止";
        }
    }

    //多阶贝塞尔曲线算法
    public void BezierNew(GameObject go,List<Vector3> ndLst , float k)
    {
        if(ndLst.Count == 2)
        {
            Vector3 pos1 = ndLst[0];
            Vector3 pos2 = ndLst[1];
            go.transform.localPosition = Vector3.Lerp(pos1, pos2, k);
            //设置旋转
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


    //箭头跟随鼠标
    public void ArrowFollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = mousePos - new Vector2(rootPos.parent.transform.position.x, rootPos.parent.transform.position.y);
        endPos.transform.localPosition = new Vector3(targetPos.x,targetPos.y,0);
        //箭头旋转
        Vector3 prevNodePos = new Vector3();
        if (nodeList.Count == 0)//没有节点画直线
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
    //运行时改变物体数量
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
    //物体数量改变
    public void OnNumChange()
    {
        DestroyPoints();
        CreatPoints();
        //UpdatePointsPos();
        previousNum = objNum;
    }
    //更新节点位置
    public void UpdateNodePos()
    {
        if (nodeList.Count == 0) return;
        for(int i = 0; i < nodeList.Count; i++)
        {
            //开启鼠标控制
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
    //点加号自动创建节点
    public Node AddNode()
    {
        var addNode = new Node();
        GameObject go = new GameObject("p" + nodeList.Count, typeof(RectTransform));
        go.transform.SetParent(nodeRoot.transform);
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        addNode.nodePos = go.GetComponent<RectTransform>();
        return addNode;
    }
    //点减号删除节点
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
    //重命名节点
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

//贝塞尔曲线节点
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