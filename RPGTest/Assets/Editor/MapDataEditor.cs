using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//自定义Tset脚本
[CustomEditor(typeof(MapDataEditorNode))]
//在编辑模式下执行脚本，这里用处不大可以删除。
[ExecuteInEditMode]
//请继承Editor
public class MapDataEditor : Editor
{
    static public MapDataEditorNode m_targetObj;
    static public GameObject mapDataObj = null;

    // 存储读取路径
    private static string m_saveDir;


    public override void OnInspectorGUI()
    {
        //得到Test对象
        m_targetObj = (MapDataEditorNode)target;

        m_targetObj.m_rootObj = EditorGUILayout.ObjectField("当前Obj", m_targetObj.m_rootObj, typeof(GameObject), true) as GameObject;
        m_targetObj.m_sMapName = EditorGUILayout.TextField("地图名", m_targetObj.m_sMapName);

        if (GUILayout.Button("读取地图节点"))
            LoadDataFromObj();

        if (GUILayout.Button("保存地图节点"))
            SaveDataToCSV(m_targetObj.m_sMapName);
    }

    private void LoadDataFromObj()
    {
        if (m_targetObj == null)
        {
            return;
        }

        for (int i = m_targetObj.transform.childCount - 1; i >= 0; i--)
        {
            GameObject tempChild = m_targetObj.transform.GetChild(i).gameObject;
            if (tempChild)
            {
                string objName = tempChild.name;
                Vector3 objPos = tempChild.transform.position;
                float scaleX = tempChild.transform.lossyScale.x;
                float scaleY = tempChild.transform.lossyScale.y;

                if (objName.Contains("Cube"))
                {
                    m_targetObj.AddBox(tempChild);
                }
                else if (objName.Contains("Sphere"))
                {
                    m_targetObj.AddCircle(tempChild);
                }
                else if (objName.Contains("Polygon"))
                {
                    m_targetObj.AddPolygon(tempChild);
                }
                else if (objName.Contains("Edge"))
                {
                    m_targetObj.AddEdge(tempChild);
                }
            }
        }
    }

    private void SaveDataToCSV(string fileName)
    {
        m_targetObj.SaveData(fileName);
        AssetDatabase.Refresh();
    }
}
