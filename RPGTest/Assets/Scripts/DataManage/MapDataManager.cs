using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// 地图数据管理类
/// </summary>
public class MapDataManager {
    private static MapDataManager _instance;
    static public MapDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                new MapDataManager();
            }
            return _instance;
        }
    }

    public bool m_bIsInited = false;
    public Dictionary<int, MapDataNode> m_mapDataDic = new Dictionary<int, MapDataNode>();

    /// <summary>
    /// 地图数据管理
    /// </summary>
    private MapDataManager()
    {
        _instance = this;
    }

    /// <summary>
    /// 初始化MapDataManage 其实是加载AllMapData文件(暂不用)
    /// </summary>
    public void Init()
    {
        // 读取数据
        InfoNodeList viList = InfoNodeHelper.ReadFromCSV(GlobalVarFun.m_streamAssets + "Map/AllMapData.csv");

        string[] arrKeys = viList.m_nodeList.Keys.ToArray();
        for (int i = 0; i < viList.Count; i++)
        {
            InfoNode vi = viList.m_nodeList[arrKeys[i]];
            if (vi != null)
            {
                MapDataNode mapdata = new MapDataNode(vi);
                m_mapDataDic.Add(mapdata.ID, mapdata);

                //UnityEngine.Debug.Log("MapDataManage.cs  ---  LoadAllMapData : " + mapdata.ID + " : " + mapdata.Name);
            }
        }
        m_bIsInited = true;
    }

    /// <summary>
    /// 加载单个地图数据
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public MapDataNode LoadCurrentMapData(int mapId)
    {
        if (!m_mapDataDic.ContainsKey(mapId))
        {
            UnityEngine.Debug.Log("MapDataManage.cs  ---  !m_mapDataDic.ContainsKey(mapId)");
            return null;
        }
        MapDataNode mapdata = m_mapDataDic[mapId];
        UnityEngine.Debug.Log("MapDataManage.cs  ---  mapdata 11 : " + mapId + " : " + mapdata);
        if (mapdata != null)
        {
            UnityEngine.Debug.Log("MapDataManage.cs  ---  mapdata != null");
            if (mapdata.HasLoaded == false)
            {
                UnityEngine.Debug.Log("MapDataManage.cs  ---  mapdata.HasLoaded == false");
                mapdata = LoadMapCollider(mapId);
                return mapdata;
            }
            else
            {
                return mapdata;
            }
        }
        return null;
    }

    /// <summary>
    /// 加载地图碰撞数据
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public MapDataNode LoadMapCollider(int mapId)
    {
        if (!m_mapDataDic.ContainsKey(mapId))
        {
            UnityEngine.Debug.Log("MapDataManage.cs  --- 22 !m_mapDataDic.ContainsKey(mapId)");
            return null;
        }
        MapDataNode mapdata = m_mapDataDic[mapId];
        UnityEngine.Debug.Log("MapDataManage.cs  ---  mapdata" + mapId + ":" + mapdata.Name);
        // 读地图Collider
        InfoNodeList viList = InfoNodeHelper.ReadFromCSV(GlobalVarFun.m_streamAssets + "MAP/" + mapId + ".csv");
        string[] arrKeys = viList.m_nodeList.Keys.ToArray();
        for (int i = 0; i < viList.Count; i++)
        {
            InfoNode vi = viList.m_nodeList[arrKeys[i]];
            if (vi != null)
            {
                mapdata.CreateCollider(vi);
            }
        }
        mapdata.HasLoaded = true;
        return mapdata;
    }

}
