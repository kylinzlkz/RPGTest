using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBodyManager {

    private static DamageBodyManager _instance;
    public static DamageBodyManager Instance
    {
        get
        {
            if (null == _instance)
            {
                new DamageBodyManager();
            }
            return _instance;
        }
    }

    public GameObject m_rootObj = null;
    public Dictionary<string, DamageBody> m_damageBodyDic = new Dictionary<string, DamageBody>();

    private int m_iHandleIndex = 0;
    private bool m_bLoaded = false;
    public bool HasLoaded
    {
        get
        {
            if (m_rootObj == null || m_damageBodyDic.Count <= 0)
            {
                m_bLoaded = false;
            }
            return m_bLoaded;
        }
    }

    public DamageBodyManager()
    {
        _instance = this;
        m_rootObj = GlobalVarFun.GetGlobalObj("DamageBodyRoot");
        if (m_rootObj == null)
        {
            m_bLoaded = false;
            return;
        }
        m_bLoaded = true;
    }

    public bool Update(float deltaTime, double time)
    {
        if (m_damageBodyDic == null)
        {
            return false;
        }
        foreach (KeyValuePair<string, DamageBody> kv in m_damageBodyDic)
        {
            kv.Value.UpdatePhysic(deltaTime);
        }
        return true;
    }

    public void Clear()
    {
        RemoveAllDamageBody();
        m_damageBodyDic.Clear();
        m_bLoaded = false;
    }

    /// <summary>
    /// 移除全部伤害体
    /// </summary>
    public void RemoveAllDamageBody()
    {

    }

    /// <summary>
    /// 添加伤害体 TODO
    /// </summary>
    /// <param name="damageBodyObj"></param>
    /// <param name="skillId"></param>
    /// <param name="damageId"></param>
    /// <returns></returns>
    public DamageBody AddDamageBody(BaseRole host, int skillId, int damageId)
    {
        if (MapManager.Instance == null || MapManager.Instance.HasCurMap == false)
        {
            return null;
        }
        DamageBodyInfo dbInfo = DamageDataManager.Instance.GetDamageBodyInfo(damageId);
        if (dbInfo == null)
        {
            return null;
        }

        GameObject obj = new GameObject();
        if (obj == null)
        {
            return null;
        }
        string handleName = skillId + "_" + GetHandleIndex(); // 没想好
        obj.name = handleName;
        GlobalVarFun.AttachChild(m_rootObj, obj);
        DamageBody db = obj.AddComponent<DamageBody>();
        db.InitDamage(host, handleName, skillId, dbInfo);
        m_damageBodyDic.Add(handleName, db);
        return db;
    }

    public void RemoveDamageBody(string handleName)
    {
        if (m_damageBodyDic == null)
        {
            return;
        }
        DamageBody db = null;
        if (m_damageBodyDic.TryGetValue(handleName, out db))
        {
            m_damageBodyDic.Remove(handleName);
        }
    }

    private int GetHandleIndex()
    {
        return m_iHandleIndex++;
    }


    public void TestAddDamageBody(BaseRole host)
    {
        AddDamageBody(host, 1, 1);
    }
}
