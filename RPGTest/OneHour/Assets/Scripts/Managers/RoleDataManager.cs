using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// role基础数据结构
/// </summary>
public class BaseRoleInfo
{
    public int RoleID { get; private set; }
    public string Name { get; private set; }
    public string PrefabName { get; private set; }
    public int Sex { get; private set; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public float VelocityX { get; private set; }
    public float VelocityY { get; private set; }
    public InfoNode BaseInfo { get; private set; }

    public BaseRoleInfo()
    {

    }

    public BaseRoleInfo(InfoNode vi)
    {
        RoleID = vi.GetValueInt("ID");
        Name = vi.GetValue("NAME");
        PrefabName = vi.GetValue("PREFABNAME");
        Sex = vi.GetValueInt("SEX");
        MaxHp = vi.GetValueInt("MAXHP");
        Attack = vi.GetValueInt("ATTACK");
        Width = vi.GetValueFloat("WIDTH");
        Height = vi.GetValueFloat("HEIGHT");
        VelocityX = vi.GetValueFloat("VELX");
        VelocityY = vi.GetValueFloat("VELY");
        BaseInfo = vi;
    }
}

public class MapRoleInfo
{
    public int ID { get; private set; }
    public int RoleID { get; private set; }
    public int RoleType { get; private set; }
    public float InitPosX { get; private set; }
    public float InitPosY { get; private set; }
    public int InitHP { get; private set; }
    public int InitStatus { get; private set; }
    public bool IsStatic { get; private set; }
    public bool CanMove { get; private set; }
    public bool CanControl { get; private set; }
    public VelcroPhysics.Collision.Filtering.Category InitCategories { get; private set; } // TODO
    public VelcroPhysics.Collision.Filtering.Category InitCollideCats { get; private set; } // TODO
    public string AITreeName { get; private set; } // TODO
    public InfoNode BaseInfo { get; private set; }

    public MapRoleInfo()
    {

    }

    public MapRoleInfo(InfoNode vi)
    {
        ID = vi.GetValueInt("ID");
        RoleID = vi.GetValueInt("ROLEID");
        RoleType = vi.GetValueInt("ROLETYPE");
        InitPosX = vi.GetValueFloat("INITPOSX");
        InitPosY = vi.GetValueFloat("INITPOSY");
        InitHP = vi.GetValueInt("CURHP");
        InitStatus = vi.GetValueInt("STATUS");
        IsStatic = vi.GetValueBool("ISSTATIC");
        CanMove = vi.GetValueBool("CANMOVE");
        CanControl = vi.GetValueBool("CANCONTROL");
        BaseInfo = vi;
    }

}

/// <summary>
/// 人物数据管理类
/// </summary>
public class RoleDataManager {
    private static RoleDataManager _instance;
    static public RoleDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                new RoleDataManager();
            }
            return _instance;
        }
    }

    public bool m_bIsInited = false;
    public Dictionary<int, BaseRoleInfo> m_roleDataDic = new Dictionary<int, BaseRoleInfo>();

    /// <summary>
    /// 人物数据管理
    /// </summary>
    private RoleDataManager()
    {
        _instance = this;
    }

    /// <summary>
    /// 初始化RoleDataManage 其实是加载RoleData文件
    /// </summary>
    public void Init()
    {
        // 读取数据
        InfoNodeList viList = InfoNodeHelper.ReadFromCSV(GlobalVarFun.m_streamAssets + "Role/RoleData.csv");
        if (null == viList)
        {
            m_bIsInited = false;
            return;
        }
        string[] arrKeys = viList.m_nodeList.Keys.ToArray();
        for (int i = 0; i < viList.Count; i++)
        {
            InfoNode vi = viList.m_nodeList[arrKeys[i]];
            if (vi != null)
            {
                BaseRoleInfo roleData = new BaseRoleInfo(vi);
                m_roleDataDic.Add(roleData.RoleID, roleData);

                UnityEngine.Debug.Log("RoleDataManager.cs  ---  LoadAllRoleData : " + roleData.RoleID + " : " + roleData.Name);
            }
        }
        m_bIsInited = true;
    }

    /// <summary>
    /// 加载当前Map的所有Role数据
    /// </summary>
    /// <param name="mapId"></param>
    public List<MapRoleInfo> GetMapRoleData(int mapId)
    {
        // 读取数据
        InfoNodeList viList = InfoNodeHelper.ReadFromCSV(GlobalVarFun.m_streamAssets + "Role/" + mapId + ".csv");
        if (null == viList)
        {
            return null;
        }

        List<MapRoleInfo> mapRoleInfo = new List<MapRoleInfo>();
        string[] arrKeys = viList.m_nodeList.Keys.ToArray();
        for (int i = 0; i < viList.Count; i++)
        {
            InfoNode vi = viList.m_nodeList[arrKeys[i]];
            if (vi != null)
            {
                MapRoleInfo roleData = new MapRoleInfo(vi);
                mapRoleInfo.Add(roleData);
            }
        }
        return mapRoleInfo;
    }

    public BaseRoleInfo GetRoleBaseInfo(int roleId)
    {
        if (null == m_roleDataDic)
        {
            return null;
        }
        if (m_roleDataDic.ContainsKey(roleId))
        {
            return m_roleDataDic[roleId];
        }
        return null;
    }
}
