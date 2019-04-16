using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDamageBodyType
{
    None = 0,
    Damage = 1,
    Heal = 2,
}

public enum eDamageMoveType
{
    None = 0,
    Follow = 1, // 跟踪弹
    Linear = 2, // 线性运动
    RoleBound = 3, // 在角色中反弹
}

/// <summary>
/// 发生碰撞后的反弹
/// </summary>
public enum eDamageBoundType
{
    None = 0,
    DestroyImm = 1, // 碰撞即消失
    DestroyDelay = 2, // 碰撞延迟消失
    BoundOrigin = 3, // 原方向反弹
    BoundNormal = 4, // 按法线对称反弹
    Cross = 5, // 直接穿过无视碰撞
}

/// <summary>
/// 伤害体
/// </summary>
public class DamageBody : MonoBehaviour {

    public BaseRole m_host = null;
    public MapDamage m_mapDamage = null;
    public MapWorld m_curMap = null;

    public GameObject m_showObj = null;

    public DamageBodyInfo m_damageBodyInfo = null;

    public string m_sHandleName;
    public int m_iSkillID;
    public eDamageBodyType m_eDamageType = eDamageBodyType.None;
    public eDamageMoveType m_eMoveType = eDamageMoveType.None;
    public eDamageBoundType m_eBoundType = eDamageBoundType.None;
    public int m_iDamage = 0;
    public float m_fEsTime = 0.0f;
    public int m_iDamageCount = 1;
    public float m_fSingleDamageTime = 0.0f;
    public int m_iBeDestroyLevel = -1; // 被抵消级别 -1为不可抵消 否则可以被级别高的伤害体抵消

    protected float m_fWidth;
    protected float m_fHeight;

    protected bool m_bStarted = false;
    protected float m_fBeginTime = 0.0f;
    protected float m_fEsTimer = 0.0f;
    protected int m_iDamageCounter = 0;
    protected float m_fSingleDamageTimer = 0.0f;

    protected float m_fPosX = 0.0f;
    protected float m_fPosY = 0.0f;
    protected Vector3 m_pos = new Vector3();
    protected int m_iDirX = 0;
    protected int m_iDirY = 0;
    protected float m_fVelocityX = 0.0f;
    protected float m_fVelocityY = 0.0f;
    protected Vector2 m_velocity = new Vector2();

    protected int m_iCurDamageRoleIndex = 0;
    protected List<BaseRole> m_damageRoles = new List<BaseRole>();
    protected BaseRole m_curTargetRole = null;

    public bool m_bIsDamage
    {
        get
        {
            return m_eDamageType == eDamageBodyType.Damage;
        }
    }

    /// <summary>
    /// 是否是移动的伤害体
    /// </summary>
    public bool m_bCanMove
    {
        get
        {
            return (m_eMoveType == eDamageMoveType.Follow || m_eMoveType == eDamageMoveType.Linear);
        }
    }

    /// <summary>
    /// 是否跟踪
    /// </summary>
    public bool m_bIsFollow
    {
        get
        {
            return m_eMoveType == eDamageMoveType.Follow;
        }
    }

    public bool m_bIsCrossCollider
    {
        get
        {
            return m_eBoundType == eDamageBoundType.Cross;
        }
    }

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
	    if (m_bStarted == false)
        {
            return;
        }
        if (m_fEsTime != -1)
        {
            m_fEsTimer = m_fEsTimer + GlobalVarFun.GetSmoothDeltaTime();
            if ((m_fEsTimer - m_fEsTime) < GlobalVarFun.FloatCompareRatio)
            {
                Destroy();
            }
        }
        m_fSingleDamageTimer = m_fSingleDamageTimer + GlobalVarFun.GetSmoothDeltaTime();
        if ((m_fSingleDamageTimer - m_fSingleDamageTime) < GlobalVarFun.FloatCompareRatio)
        {
            m_fSingleDamageTimer = 0;
            // 造成Damage
            OnDamage();
        }
        UpdateMove();
    }

    /// <summary>
    /// 设置伤害体初始数据
    /// </summary>
    /// <returns></returns>
    public bool InitDamage(BaseRole host, string handleName, int dirX, int dirY, int skillId, DamageBodyInfo node)
    {
        if (host == null || node == null)
        {
            m_bStarted = false;
            return false;
        }
        m_host = host;
        m_sHandleName = handleName;
        m_iSkillID = skillId;
        m_eDamageType = (eDamageBodyType)node.DamageType;
        m_eMoveType = (eDamageMoveType)node.MoveType;
        m_damageBodyInfo = node;
        // TODO 可能有很多个？
        GameObject prefabObj = Resource.LoadObj(m_damageBodyInfo.PrefabName) as GameObject;
        if (null == prefabObj)
        {
            m_bStarted = false;
            return false;
        }
        GameObject showObj = GameObject.Instantiate(prefabObj);
        showObj.name = "node";
        GlobalVarFun.AttachChild(gameObject, showObj);
        m_fWidth = m_damageBodyInfo.Width;
        m_fHeight = m_damageBodyInfo.Height;
        m_fEsTime = m_damageBodyInfo.EsTime;
        m_iDamageCount = m_damageBodyInfo.DamageCount;
        m_iDamage = m_damageBodyInfo.DamageValue;
        m_fPosX = host.m_fPositionX + m_damageBodyInfo.OffsetX;
        m_fPosY = host.m_fPositionY + m_damageBodyInfo.OffsetY;
        m_iDirX = dirX;
        m_iDirY = dirY;
        m_fVelocityX = dirX * m_damageBodyInfo.MoveVelocity;
        m_fVelocityY = dirY * m_damageBodyInfo.MoveVelocity;
        m_velocity.x = m_fVelocityX;
        m_velocity.y = m_fVelocityY;

        if (m_mapDamage == null)
        {
            m_mapDamage = new MapDamage(this);
        }
        m_mapDamage.InitB2Body(MapManager.Instance.curMapWorld, m_fPosX, m_fPosY, m_fWidth, m_fHeight);
        m_mapDamage.SetCurVelocityX(m_fVelocityX);
        m_mapDamage.SetCurVelocityY(m_fVelocityY);

        Clear();
        m_bStarted = true;
        m_fBeginTime = GlobalVarFun.GetTime();
        return true;
    }

    public void Clear()
    {
        m_fBeginTime = 0.0f;
        m_fEsTimer = 0.0f;
        m_iCurDamageRoleIndex = 0;
        m_damageRoles.Clear();
    }

    public void Destroy()
    {
        m_damageRoles.Clear();
        DamageBodyManager.Instance.RemoveDamageBody(m_sHandleName);
        GameObject.Destroy(gameObject);
    }

    #region UpdateFuncs

    /// <summary>
    /// 更新移动速度
    /// </summary>
    protected void UpdateVelocity()
    {
        if (m_mapDamage == null)
        {
            return;
        }
        // 其实是Follow就跟踪target，Linaer不用管，RoleBound就等OnDamage之后更新target，
        // 所以相当于follow和bound是同等原理，而Linear不需要做处理
        if (m_eMoveType == eDamageMoveType.Follow)
        {
            // 得到target的位置 更新velocity的大小（改变方向）
        }
        else if (m_eMoveType == eDamageMoveType.Linear)
        {
            // 无
        }
        else if (m_eMoveType == eDamageMoveType.RoleBound)
        {
            
        }
    }

    /// <summary>
    /// 更新移动
    /// </summary>
    protected void UpdateMove()
    {
        if (m_mapDamage == null)
        {
            return;
        }
        // TODO 改成纯数据计算
        //Vector2 newPos = new Vector2(m_pos.x, m_pos.y);
        //newPos += m_velocity * GlobalVarFun.GetSmoothDeltaTime();

        //Vector2 dist = new Vector2(m_pos.x - newPos.x, m_pos.y - newPos.y);
        //float distSqr = dist.SqrMagnitude();
        ////UnityEngine.Debug.Log("Speed : " + m_linearVelocity + " ; " + dist + " ; " + distSqr);
        //if (distSqr > GlobalVarFun.MoveDistSqrMaxThreshhold)
        //{
        //    newPos.x = m_pos.x;
        //    newPos.y = m_pos.y;
        //}
        //else if (distSqr > GlobalVarFun.MoveDistSqrMinThreshhold)
        //{
        //    //m_curVelocity = m_curVelocity * 0.7f + dist / deltaTime;
        //    newPos.x = m_pos.x;
        //    newPos.y = m_pos.y;
        //    newPos += m_velocity * GlobalVarFun.GetSmoothDeltaTime();
        //}
        //m_pos = newPos;

        m_fPosX = m_mapDamage.m_position.X;
        m_fPosY = m_mapDamage.m_position.Y;
        m_pos.x = m_fPosX;
        m_pos.y = m_fPosY;
        transform.position = m_pos;
    }


    /// <summary>
    /// 物理状态更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdatePhysic(float deltaTime)
    {
        if (m_bStarted == false || m_bCanMove == false)
        {
            return;
        }

        UpdatePhysicNode(deltaTime);
    }

    /// <summary>
    /// 更新MapDamage
    /// </summary>
    /// <param name="deltaTime"></param>
    protected void UpdatePhysicNode(float deltaTime)
    {
        if (m_mapDamage == null)
            return;

        m_mapDamage.Update(deltaTime);
    }

    #endregion UpdateFuncs

    #region Transform 

    public void SetTransformZ(float z)
    {
        m_pos.z = z;
        transform.position = m_pos;
    }

    public void SetRotation(Vector3 rotation)
    {

    }

    #endregion Transform

    #region Collision
    public void AddDamageRole(string roleHandle)
    {
        if (false == IsInDamageRoleList(roleHandle))
        {
            BaseRole role = RoleManager.Instance.GetBaseRole(roleHandle);
            if (role != null)
            {
                m_damageRoles.Add(role);
            }
        }
    }

    public void RemoveDamageRole(string roleHandle)
    {
        BaseRole role = null;
        for(int i = m_damageRoles.Count - 1; i >= 0; i--)
        {
            role = m_damageRoles[i];
            if (role != null && role.m_sHandleName == roleHandle)
            {
                m_damageRoles.RemoveAt(i);
            }
        }
    }

    public bool OnDamage()
    {
        if (m_damageRoles == null || m_damageRoles.Count <= 0)
        {
            return false;
        }

        for (int i = 0; i < m_damageRoles.Count; i++)
        {
            m_damageRoles[i].OnDamage(this); // 受击
            m_iDamageCounter += 1;

        }
        return true;
    }


    public void OnColliderCollision(string colliderHandle)
    {
        // TODO body设置弹力属性为0或者是1？？？
        // 先假设都是停止然后消失吧
        // TODO 如何判断拦截方向？？
        if (m_mapDamage == null)
        {
            return;
        }
        m_mapDamage.SetCurVelocityX(0);
        m_mapDamage.SetCurVelocityY(0);
    }

    #endregion Collision

    public bool IsInDamageRoleList(string roleHandle)
    {
        if (m_damageRoles == null || m_damageRoles.Count <= 0)
        {
            return false;
        }
        BaseRole role = null;
        for (int i = 0; i < m_damageRoles.Count; i++)
        {
            role = m_damageRoles[i];
            if (role != null && role.m_sHandleName == roleHandle)
            {
                return true;
            }
        }
        return false;
    }
}
