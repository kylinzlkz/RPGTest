using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleManager {

    private static RoleManager _instance;
    public static RoleManager Instance
    {
        get
        {
            if (null == _instance)
            {
                new RoleManager();
            }
            return _instance;
        }
    }

    public GameObject m_rootObj = null;
    public List<GameObject> m_sideList = new List<GameObject>();

    public Dictionary<string, BaseRole> m_roleDic = new Dictionary<string, BaseRole>();
    public PlayerControl m_player = null;

    private int m_iHandleIndex = 0;
    private bool m_bLoaded = false;
    public bool HasLoaded
    {
        get
        {
            if (m_rootObj == null || m_roleDic.Count <= 0)
            {
                m_bLoaded = false;
            }
            return m_bLoaded;
        }
    }

    public RoleManager()
    {
        _instance = this;
        m_rootObj = GlobalVarFun.GetGlobalObj("RoleRoot");
        if (m_rootObj == null)
        {
            m_bLoaded = false;
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject side = new GameObject(); // GlobalVarFun.GetChild(m_rootObj, "side_" + i);
            if (side == null)
            {
                break;
            }
            side.name = "side_" + i;
            GlobalVarFun.AttachChild(m_rootObj, side);
            m_sideList.Add(side);
        }
        m_bLoaded = true;
    }

    public bool Update(float deltaTime, double time)
    {
        if (m_roleDic == null)
        {
            return false;
        }
        foreach (KeyValuePair<string, BaseRole> kv in m_roleDic)
        {
            kv.Value.UpdatePhysic(deltaTime);
        }
        return true;
    }

    public void Clear()
    {
        RemoveAllRole();
        m_roleDic.Clear();
        m_bLoaded = false;
    }

    /// <summary>
    /// 移除全部Role
    /// </summary>
    public void RemoveAllRole()
    {

    }

    /// <summary>
    /// 加载新地图的Role
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public bool LoadNewMapRole(int mapId)
    {
        if (RoleDataManager.Instance == null || m_rootObj == null)
        {
            Debug.LogError("RoleManager.cs  ---  RoleDataManager.Instance == null || m_rootObj == null");
            Clear();
            return false;
        }
        if (MapManager.Instance == null || MapManager.Instance.HasCurMap == false)
        {
            Debug.LogError("RoleManager.cs  ---  MapManager.Instance == null || MapManager.Instance.hasCurMap == false");
            Clear();
            return false;
        }

        List<MapRoleInfo> nodeList = RoleDataManager.Instance.GetMapRoleData(mapId);
        if (nodeList == null)
        {
            Debug.LogError("RoleManager.cs  ---  MapRoleData == null");
            Clear();
            return false;
        }

        // 创建Role
        for (int i = 0; i < nodeList.Count; i++)
        {
            MapRoleInfo node = nodeList[i];
            string handleName = node.ID + "_" + GetHandleIndex();
            GameObject roleObj = new GameObject();
            roleObj.name = handleName;
            bool addRole = AddRoleObj(handleName, roleObj, node);
        }

        if (m_rootObj == null || m_roleDic.Count <= 0)
        {
            m_bLoaded = false;
        }
        else
        {
            m_bLoaded = true;
        }
        return m_bLoaded;
    }

    #region Add
    public BaseRole CreateBaseRole(GameObject roleObj)
    {
        if (MapManager.Instance == null || MapManager.Instance.HasCurMap == false)
        {
            return null;
        }
        if (roleObj == null)
        {
            return null;
        }
        BaseRole role = roleObj.AddComponent<BaseRole>();
        return role;
    }

    public BaseRole CreatePlayer(GameObject roleObj)
    {
        if (MapManager.Instance == null || MapManager.Instance.HasCurMap == false)
        {
            return null;
        }
        if (m_player != null)
        {
            // 如果已经有了player 销毁
            GameObject.Destroy(m_player);
        }
        m_player = roleObj.AddComponent<PlayerControl>();
        return m_player;
    }

    public bool AddRoleObj(string handleName, GameObject roleObj, MapRoleInfo initInfo)
    {
        BaseRole baseRole = null;
        if (true == initInfo.CanControl)
        {
            baseRole = CreatePlayer(roleObj);
        }
        else
        {
            baseRole = CreateBaseRole(roleObj);
        }
        if (baseRole == null)
        {
            return false;
        }

        BaseRoleInfo baseInfo = RoleDataManager.Instance.GetRoleBaseInfo(initInfo.RoleID);
        if (baseInfo == null)
        {
            GameObject.Destroy(roleObj);
            return false;
        }
        bool setSuc = baseRole.InitBaseRole(handleName, baseInfo, initInfo);
        if (setSuc)
        {
            GameObject sideObj = GetSideObj(initInfo.RoleType);
            if (sideObj == null)
            {
                sideObj = m_rootObj;
            }
            GlobalVarFun.AttachChild(sideObj, roleObj);
            m_roleDic.Add(handleName, baseRole);
        }
        else
        {
            GameObject.Destroy(roleObj);
            return false;
        }
        return true;
    }

    public BaseRole GetBaseRole(string handleName)
    {
        BaseRole role = null;
        if (m_roleDic.TryGetValue(handleName, out role))
        {
            return role;
        }
        return null;
    }

    private int GetHandleIndex()
    {
        return m_iHandleIndex++;
    }
    #endregion

    public GameObject GetSideObj(int type)
    {
        if (m_sideList == null || m_sideList.Count <= 0)
        {
            return m_rootObj;
        }
        if (type < 0 || type > m_sideList.Count)
        {
            return m_rootObj;
        }
        return m_sideList[type];
    }
}
