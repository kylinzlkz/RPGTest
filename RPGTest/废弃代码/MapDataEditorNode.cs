using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataEditorNode : MonoBehaviour {

    public enum eBodyDrawType
    {
        None = 0,
        Box = 1,
        Polygon = 2,
        Circle = 3,
        Edge = 4,
    }

    // 单个Collider数据
    public class MapDataEditorSingleNode
    {
        public string name;
        public eBodyCreateType type;
        public GameObject obj;
        public List<GameObject> m_pointList;
    }

    public static MapDataEditorNode Instance = null;

    public bool m_bInited = false;
    public bool m_bHasData = false;

    // 整个地图的节点
    public GameObject m_mapLowerPosObj;
    public GameObject m_mapUpperPosObj;
    public GameObject m_collidersRootObj;

    public bool m_bIsEditing = false;
    public bool m_bIsErasing = false;

    public int m_iColliderIndex = 0;
    public List<MapDataEditorSingleNode> m_nodeList = null;

    // 当前选中什么Collider类型
    public eBodyDrawType m_eCurDrawType = eBodyDrawType.Box;
    public int m_iCurDrawTypeIndex = 0;
    public int m_iOldDrawTypeIndex = 0;
    public string[] m_eBodyDrawTypeList = new string[]
    {
        "None","Box","Polygon","Circle","Edge"
    };

    public MapDataEditorSingleNode m_curDataNode = null;
    public GameObject m_curObj = null;
    public string m_sCurObjName = "";
    public string m_sOldObjName = "";
    public GameObject m_curPointObj = null;

    // Box
    public GameObject m_curBoxObj = null;
    public GameObject m_oldBoxObj = null;
    public bool m_bIsBoxLowerPoint = true;
    public bool m_bIsBoxUpperPoint = false;
    public GameObject m_nodeLowerPointObj = null;
    public GameObject m_nodeUpperPointObj = null;


    // prefabs
    private GameObject m_pointObjPrefab = null;


    public void Init()
    {
        Instance = this;
        m_nodeList = new List<MapDataEditorSingleNode>();
    }

    #region Clear
    public void ClearAllTempData()
    {
        ResetAllBool();
        ClearBoxData();
    }

    public void ResetAllBool()
    {
        m_bInited = false;
        m_bHasData = false;
        m_bIsEditing = false;
        m_bIsErasing = false;
    }

    public void ClearBoxData()
    {
        m_curBoxObj = null;
        m_bIsBoxLowerPoint = true;
        m_bIsBoxUpperPoint = false;
        m_nodeLowerPointObj = null;
        m_nodeUpperPointObj = null;
    }
    #endregion Clear

    public void SetCurDrawType()
    {
        m_eCurDrawType = (eBodyDrawType)m_iCurDrawTypeIndex;
        m_iOldDrawTypeIndex = m_iCurDrawTypeIndex;
    }

    /// <summary>
    /// 添加新的阻碍
    /// </summary>
    /// <returns></returns>
    public bool AddNewCollider(float posX, float posY)
    {
        if (m_bIsEditing == false)
        {
            return false;
        }

        string objName = "col_" + m_iColliderIndex;
        if (m_pointObjPrefab == null)
        {
            m_pointObjPrefab = (GameObject)Resources.Load("EditorPrefabs/MapPoint");
        }

        m_curDataNode = new MapDataEditorSingleNode();
        m_curObj = new GameObject(objName);
        m_sCurObjName = objName;
        m_curObj.transform.position = new Vector3(posX, posY, -2);
        m_curDataNode.name = objName;
        m_curDataNode.obj = m_curObj;
        m_curDataNode.m_pointList = new List<GameObject>();
        switch (m_eCurDrawType)
        {
            case eBodyDrawType.Box:
                {
                    if (m_bIsBoxLowerPoint == true)
                    {
                        GameObject lowerObj = GameObject.Instantiate(m_pointObjPrefab);
                        lowerObj.name = "Lower";
                        lowerObj.transform.position = new Vector3(posX, posY, 0);
                        GlobalVarFun.AttachChild(m_curObj, lowerObj);
                        m_nodeLowerPointObj = lowerObj;
                    }
                    else
                    {
                        GameObject upperObj = GameObject.Instantiate(m_pointObjPrefab);
                        upperObj.name = "Upper";
                        upperObj.transform.position = new Vector3(posX, posY, 0);
                        GlobalVarFun.AttachChild(m_curObj, upperObj);
                        m_nodeUpperPointObj = upperObj;
                    }
                    return true;
                }
                break;
            case eBodyDrawType.Polygon:
                {

                }
                break;
            case eBodyDrawType.Circle:
                {

                }
                break;
            case eBodyDrawType.Edge:
                {

                }
                break;
        }
        return false;
    }

    public void AddOrChangeColliderPoint(float posX, float posY)
    {
        if (m_bIsEditing == false)
        {
            return;
        }
        if (m_curObj == null || m_curDataNode == null)
        {
            return;
        }

        switch (m_eCurDrawType)
        {
            case eBodyDrawType.Box:
                {
                    if (m_bIsBoxLowerPoint == true)
                    {
                        if (m_nodeLowerPointObj == null)
                        {
                            GameObject lowerObj = GameObject.Instantiate(m_pointObjPrefab);
                            lowerObj.name = "Lower";
                            lowerObj.transform.position = new Vector3(posX, posY, 0);
                            GlobalVarFun.AttachChild(m_curObj, lowerObj);
                            m_nodeLowerPointObj = lowerObj;
                        }
                        else
                        {
                            m_nodeLowerPointObj.transform.position = new Vector3(posX, posY, 0);
                        }
                    }
                    else
                    {
                        if (m_nodeUpperPointObj == null)
                        {
                            GameObject upperObj = GameObject.Instantiate(m_pointObjPrefab);
                            upperObj.name = "Upper";
                            upperObj.transform.position = new Vector3(posX, posY, 0);
                            GlobalVarFun.AttachChild(m_curObj, upperObj);
                            m_nodeUpperPointObj = upperObj;
                        }
                        else
                        {
                            m_nodeUpperPointObj.transform.position = new Vector3(posX, posY, 0);
                        }
                    }
                    if (m_nodeLowerPointObj != null && m_nodeUpperPointObj != null)
                    {
                        if (m_nodeLowerPointObj.transform.position.x > m_nodeUpperPointObj.transform.position.y)
                        {
                            GameObject tempObj = m_nodeLowerPointObj;
                            m_nodeLowerPointObj = m_nodeUpperPointObj;
                            m_nodeUpperPointObj = tempObj;
                        }
                        if (m_nodeLowerPointObj.transform.position.y > m_nodeUpperPointObj.transform.position.y)
                        {
                            float tempY = m_nodeLowerPointObj.transform.position.y;
                            m_nodeLowerPointObj.transform.position = new Vector3(m_nodeLowerPointObj.transform.position.x, m_nodeUpperPointObj.transform.position.y, 0);
                            m_nodeUpperPointObj.transform.position = new Vector3(m_nodeUpperPointObj.transform.position.x, tempY, 0);
                        }
                        if (m_curBoxObj == null)
                        {
                            m_curBoxObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            float sx = Mathf.Abs(m_nodeLowerPointObj.transform.position.x - m_nodeUpperPointObj.transform.position.x);
                            float sy = Mathf.Abs(m_nodeLowerPointObj.transform.position.y - m_nodeUpperPointObj.transform.position.y);
                            m_curBoxObj.transform.localScale = new Vector3(sx, sy, 1);
                            m_curBoxObj.transform.position = new Vector3(sx / 2, sy / 2, 0);
                        }
                        else
                        {
                            float sx = Mathf.Abs(m_nodeLowerPointObj.transform.position.x - m_nodeUpperPointObj.transform.position.x);
                            float sy = Mathf.Abs(m_nodeLowerPointObj.transform.position.y - m_nodeUpperPointObj.transform.position.y);
                            m_curBoxObj.transform.localScale = new Vector3(sx, sy, 1);
                            m_curBoxObj.transform.position = new Vector3(sx / 2, sy / 2, 0);
                        }
                    }
                }
                break;
            case eBodyDrawType.Polygon:
                {

                }
                break;
            case eBodyDrawType.Circle:
                {

                }
                break;
            case eBodyDrawType.Edge:
                {

                }
                break;
        }

        return;
    }
}
