using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 伤害体数据
/// </summary>
public class DamageBodyInfo
{
    public int ID { get; set; }
    public int DamageType { get; set; } // 伤害类型
    public int MoveType { get; set; } // 移动类型
    public string Name { get; set; }
    public string PrefabName { get; set; }
    public float Width { get; set; } // 宽
    public float Height { get; set; } // 高
    public float OffsetX { get; set; } // 初始偏移量X
    public float OffsetY { get; set; } // 初始偏移量Y
    public int DamageValue { get; set; } // 伤害值
    public float EsTime { get; set; } // 持续时间
    public int DamageCount { get; set; } // 可造成伤害次数
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public InfoNode BaseInfo { get; set; }

    public DamageBodyInfo()
    {

    }

    public DamageBodyInfo(InfoNode vi)
    {
        ID = 1;
        DamageType = (int)eDamageBodyType.Damage;
        MoveType = (int)eDamageMoveType.Linear;
        Name = "123";
        PrefabName = "Damage/Damage1";
        Width = 1;
        Height = 1;
        OffsetX = 0;
        OffsetY = 0;
        DamageValue = 10;
        EsTime = -1;
        VelocityX = 5;
        VelocityY = 0;
    }
}

public class DamageDataManager {
    private static DamageDataManager _instance;
    static public DamageDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                new DamageDataManager();
            }
            return _instance;
        }
    }

    public bool m_bIsInited = false;
    public Dictionary<int, DamageBodyInfo> m_damageDataDic = new Dictionary<int, DamageBodyInfo>();

    /// <summary>
    /// 人物数据管理
    /// </summary>
    private DamageDataManager()
    {
        _instance = this;
    }

    /// <summary>
    /// 初始化DamageDataManager 其实是加载DamageData文件
    /// </summary>
    public void Init()
    {
        // 读取数据
        InfoNodeList viList = InfoNodeHelper.ReadFromCSV(GlobalVarFun.m_streamAssets + "Battle/DamageBodyData.csv");
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
                DamageBodyInfo dbData = new DamageBodyInfo(vi);
                m_damageDataDic.Add(dbData.ID, dbData);

                UnityEngine.Debug.Log("DamageDataManager.cs  ---  LoadAllDamageBodyData : " + dbData.ID + " : " + dbData.Name);
            }
        }
        m_bIsInited = true;
    }

    /// <summary>
    /// 得到伤害体基础数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public DamageBodyInfo GetDamageBodyInfo(int id)
    {
        if (null == m_damageDataDic)
        {
            return null;
        }
        if (m_damageDataDic.ContainsKey(id))
        {
            return m_damageDataDic[id];
        }
        return null;
    }


    public void TestAddDamageInfo()
    {
        DamageBodyInfo dbData = new DamageBodyInfo(null);
        m_damageDataDic.Add(dbData.ID, dbData);
    }
}
