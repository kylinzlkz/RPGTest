using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;


//自定义Tset脚本
[CustomEditor(typeof(MapDataEditorNode))]
//在编辑模式下执行脚本，这里用处不大可以删除。
[ExecuteInEditMode]
//请继承Editor
public class MapDataEditor : Editor {

    static public MapDataEditorNode m_targetObj;
    static public GameObject mapDataObj = null;

    // 存储读取路径
    private static string m_saveDir;

    // win显示参数
    private static Rect m_winRect = new Rect(0, 0, 600, 850);
    private float m_topBoxHight = 400.0f;
    private string m_focusedName;

    // 数据
    private bool m_bForceEditNode = false;
    private string m_sForceEditNodeName = "";

    private bool m_bIsClick = false;
    private Transform m_curRayHitTrans = null;

    // 提示
    private static bool m_bIsNotice = false;
    private static string m_sErrorNote = "";


    public override void OnInspectorGUI()
    {
        //得到Test对象
        m_targetObj = (MapDataEditorNode)target;
        bool isInEd = false;

        if (GUILayout.Button("初始化节点"))
            InitObject();

        GUILayout.BeginHorizontal();
        m_targetObj.m_bIsEditing = GUILayout.Toggle(m_targetObj.m_bIsEditing, "编辑");
        m_targetObj.m_bIsErasing = GUILayout.Toggle(m_targetObj.m_bIsErasing, "橡皮擦");
        GUILayout.EndHorizontal();

        if (m_targetObj.m_bIsEditing == true && m_targetObj.m_bInited == false)
        {
            m_sErrorNote = "请先初始化节点！";
            return;
        }

        DrawFuncPanel();
        SetToEdit(m_targetObj.m_bIsEditing);

        GUILayout.Space(10);

        if (GUILayout.Button("存储该节点"))
            SaveCurColliderData();
        if (GUILayout.Button("清理缓存"))
            m_targetObj.ClearAllTempData();

        CheckTempData();

    }

    #region 初始化
    private void InitObject()
    {
        m_saveDir = GlobalVarFun.m_streamAssets + "MapAttr/" + GetSceneName() + "/";
        if (!Directory.Exists(m_saveDir))
        {
            Directory.CreateDirectory(m_saveDir);
        }
        mapDataObj = m_targetObj.gameObject;

        m_targetObj.m_mapLowerPosObj = GlobalVarFun.GetChild(mapDataObj, "LowerPos");
        if (m_targetObj.m_mapLowerPosObj == null)
        {
            m_targetObj.m_bInited = false;
            return;
        }
        m_targetObj.m_mapUpperPosObj = GlobalVarFun.GetChild(mapDataObj, "UpperPos");
        if (m_targetObj.m_mapUpperPosObj == null)
        {
            m_targetObj.m_bInited = false;
            return;
        }
        m_targetObj.m_collidersRootObj = GlobalVarFun.GetChild(mapDataObj, "CollidersRoot");
        if (m_targetObj.m_collidersRootObj == null)
        {
            m_targetObj.m_bInited = false;
            return;
        }
        m_targetObj.m_bInited = true;
        m_targetObj.m_iCurDrawTypeIndex = (int)MapDataEditorNode.eBodyDrawType.Box;
    }

    #endregion 初始化

    #region GUI函数
    private void DrawFuncPanel()
    {
        m_targetObj.m_iCurDrawTypeIndex = EditorGUILayout.Popup(m_targetObj.m_iCurDrawTypeIndex, m_targetObj.m_eBodyDrawTypeList, GUI.skin.button, GUILayout.Height(20));

        m_targetObj.m_sCurObjName = EditorGUILayout.TextField("当前节点名", m_targetObj.m_sCurObjName);
        switch (m_targetObj.m_iCurDrawTypeIndex)
        {
            case (int)MapDataEditorNode.eBodyDrawType.Box:
                {
                    GUILayout.BeginHorizontal();
                    m_targetObj.m_bIsBoxLowerPoint = GUILayout.Toggle(m_targetObj.m_bIsBoxLowerPoint, "左下");
                    m_targetObj.m_bIsBoxUpperPoint = GUILayout.Toggle(m_targetObj.m_bIsBoxUpperPoint, "右上");
                    GUILayout.EndHorizontal();
                    EditorGUILayout.ObjectField("当前Obj", m_targetObj.m_curObj, typeof(GameObject), true);
                    EditorGUILayout.ObjectField("当前Box", m_targetObj.m_curBoxObj, typeof(GameObject), true);
                    EditorGUILayout.ObjectField("Box左下", m_targetObj.m_nodeLowerPointObj, typeof(GameObject), true);
                    EditorGUILayout.ObjectField("Box右上", m_targetObj.m_nodeUpperPointObj, typeof(GameObject), true);
                }
                break;
            case (int)MapDataEditorNode.eBodyDrawType.Polygon:
                {

                }
                break;
            case (int)MapDataEditorNode.eBodyDrawType.Circle:
                {

                }
                break;
            case (int)MapDataEditorNode.eBodyDrawType.Edge:
                {

                }
                break;
        }
    }

    void ShowNotice()
    {
        if (m_bIsNotice)
        {
            if (m_sErrorNote != "")
            {
                // 报错
                //ShowNotification(new GUIContent(m_sErrorNote));
                m_bIsNotice = false;
            }
        }
        else
        {
            m_sErrorNote = "";
        }
    }

    #endregion GUI函数

    #region Update
    private void CheckTempData()
    {
        if (m_targetObj.m_iOldDrawTypeIndex != m_targetObj.m_iCurDrawTypeIndex)
        {
            m_targetObj.SetCurDrawType();
        }
        switch (m_targetObj.m_iCurDrawTypeIndex)
        {
            case (int)MapDataEditorNode.eBodyDrawType.Box:
                {
                    if (m_targetObj.m_bIsBoxLowerPoint == true)
                    {
                        m_targetObj.m_bIsBoxUpperPoint = false;
                    }
                    else
                    {
                        m_targetObj.m_bIsBoxUpperPoint = true;
                    }

                    if (m_targetObj.m_curBoxObj != null && m_targetObj.m_curBoxObj != m_targetObj.m_oldBoxObj)
                    {
                        // 更新BoxObject的左下右上等
                        m_targetObj.m_nodeLowerPointObj = GlobalVarFun.GetChild(m_targetObj.m_curBoxObj, "Lower");
                        m_targetObj.m_nodeUpperPointObj = GlobalVarFun.GetChild(m_targetObj.m_curBoxObj, "Upper");
                    }
                }
                break;
            case (int)MapDataEditorNode.eBodyDrawType.Polygon:
                {

                }
                break;
            case (int)MapDataEditorNode.eBodyDrawType.Circle:
                {

                }
                break;
            case (int)MapDataEditorNode.eBodyDrawType.Edge:
                {

                }
                break;
        }
    }

    public void SetToEdit(bool toEditor)
    {
        m_targetObj.m_bIsEditing = toEditor;
        if (m_targetObj.m_bIsEditing)
        {
            SceneView.onSceneGUIDelegate = UpdateGUI;
        }
        else
        {
            SceneView.onSceneGUIDelegate = null;
        }
    }

    private void UpdateGUI(SceneView sceneView)
    {
        if (m_targetObj == null)
            return;

        Event e = Event.current;
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        if (m_targetObj.m_bIsEditing)
        {
            OnEditorMode(e, sceneView);
        }
        else
        {

        }
    }

    private void OnEditorMode(Event e, SceneView sceneView)
    {
        if (e.alt)
            return;
        Debug.Log("e.button : " + e.button);
        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {
                m_bIsClick = true;
                OnHitCell(sceneView, e, e.shift);
                e.Use();
            }
        }
    }

    private void OnHitCell(SceneView sceneview, Event e, bool bIsShift)
    {
        Vector3 mousePt = new Vector3(e.mousePosition.x, e.mousePosition.y, 0.0f);
        Ray ray = sceneview.camera.ScreenPointToRay(mousePt);
        Vector3 mousePos = ray.origin;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            m_curRayHitTrans = hit.transform;
        }

        if (m_curRayHitTrans)
        {
            if (m_targetObj.m_bIsErasing == false)
            {

            }
        }
        else
        {
            // 没选中Obj 但curObj已有 添加point
            if (m_targetObj.m_curObj)
            {
                m_targetObj.AddOrChangeColliderPoint(mousePos.x, mousePos.y);
            }
            else
            {
                // 否则 添加collider
                m_targetObj.AddNewCollider(mousePos.x, mousePos.y);
            }
        }
    }

    #endregion Update

    #region 功能函数
    static private string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    static private void SetNoticeInfo(string info)
    {
        m_sErrorNote = info;
        m_bIsNotice = true;
    }

    private void ClearAll()
    {
        m_targetObj.m_bInited = false;
        GlobalVarFun.RemoveAllChildImmediate(m_targetObj.m_collidersRootObj);
    }

    private void ResetAllBool()
    {
        m_bForceEditNode = false;
        m_targetObj.ResetAllBool();
        m_bIsNotice = false;
        m_bIsClick = false;
    }

    #endregion 功能函数

    #region 存储/删除
    private bool SaveCurColliderData()
    {
        return false;
    }


    #endregion 存储/删除
}
