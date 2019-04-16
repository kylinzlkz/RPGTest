using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager {

    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                new MapManager();
            }
            return _instance;
        }
    }

    private bool m_bHasCurMap = false;
    public bool HasCurMap
    {
        get
        {
            if (curMapWorld == null || curMapData == null)
            {
                m_bHasCurMap = false;
            }
            return m_bHasCurMap;
        }
    }

    public MapWorld curMapWorld = null; // 当前World
    public MapDataNode curMapData = null; // 当前地图数据
    public float scaleRatio = 1.0f;

    // 单个MapObj/MapCollider的更新函数
    public delegate void UpdateFunc(float deltaTime);
    private readonly Dictionary<int, UpdateFunc> updateFuncDic = new Dictionary<int, UpdateFunc>();
    private List<int> removeUpdateFuncIds = new List<int>();

    public MapManager()
    {
        _instance = this;
    }

    public void Update(float deltaTime, double time)
    {
        if (curMapWorld != null)
        {
            curMapWorld.Update(deltaTime, time);
        }

        foreach (var pair in updateFuncDic)
        {
            pair.Value(deltaTime);
        }

        if (removeUpdateFuncIds.Count > 0)
        {
            foreach (var id in removeUpdateFuncIds)
            {
                updateFuncDic.Remove(id);
            }
            removeUpdateFuncIds.Clear();
        }
    }

    public void Clear()
    {
        updateFuncDic.Clear();
        removeUpdateFuncIds.Clear();
    }

    /// <summary>
    /// 清除当前地图数据
    /// </summary>
    public void ClearCurMap()
    {
        updateFuncDic.Clear();
        removeUpdateFuncIds.Clear();
        curMapData = null;
        curMapWorld.Dispose();
        m_bHasCurMap = false;
    }

    public void RegisterUpdateFunc(int id, UpdateFunc func)
    {
        if (null == func)
        {
            return;
        }
        if (!updateFuncDic.ContainsKey(id))
        {
            updateFuncDic.Add(id, func);
        }
    }

    public void UnregisterUpdateFunc(int id)
    {
        removeUpdateFuncIds.Add(id);
    }


    public bool LoadNewMap(int mapId)
    {
        if (MapDataManager.Instance == null)
        {
            Debug.Log("MapManage.cs  ---  MapDataManager.Instance == null");
            return false;
        }

        curMapData = MapDataManager.Instance.LoadCurrentMapData(mapId);
        if (curMapData == null)
        {
            Debug.Log("MapManage.cs  ---  mapData == null");
            return false;
        }
        else
        {
            CreateNewWorld();
        }

        m_bHasCurMap = true;
        return true;
    }

    public void CreateNewWorld()
    {
        if (curMapData == null)
        {
            return;
        }
        if (curMapWorld == null)
        {
            curMapWorld = new MapWorld();
        }
        curMapWorld.Dispose();
        curMapWorld.Init(curMapData);
        curMapWorld.CreateColliders(curMapData.m_colliderDatas);
    }
}
