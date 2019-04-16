using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eRoleType
{
    None = 0,
    Player = 1,
    NPC = 2,
    Mon = 3,
    Boss = 4,
    Breakable = 5,

}

public enum ePhtatus
{
    Stop = 0,
    Death = 1,
    RunRight = 2,
    RunLeft = 3,
    RunUp = 4,
    RunDown = 5,
    Jump = 6,
    Lay = 7,
    LayUp = 8,
    HitRecover = 9,
    Atk1 = 10,
}

public class BaseRole : MonoBehaviour {

    public string m_sHandleName = "";
    public eRoleType m_eRoleType = eRoleType.None;
    public MapRole m_mapRole = null;
    public MapWorld m_curMap = null;

    public GameObject m_showObj = null;

    public BaseRoleInfo m_baseInfo = null;
    public MapRoleInfo m_initInfo = null;

    protected int m_iCurHp = 0;
    protected float m_fWidth;
    protected float m_fHeight;

    protected bool m_bStarted = false;

    // 是否Static
    public bool m_bIsStatic
    {
        get
        {
            if (m_mapRole != null)
            {
                return m_mapRole.m_isStatic;
            }
            return true;
        }
        set
        {
            if (m_mapRole != null)
            {
                m_mapRole.m_isStatic = value;
            }
        }
    }

    public bool m_bCanMove
    {
        get
        {
            if (m_mapRole != null)
            {
                return m_mapRole.m_bCanMove;
            }
            return true;
        }
        set
        {
            if (m_mapRole != null)
            {
                m_mapRole.m_bCanMove = value;
            }
        }
    }

    public bool m_bIsStuck
    {
        get
        {
            if (m_mapRole != null)
            {
                return m_mapRole.m_bIsStuck;
            }
            return false;
        }
    }

    protected float m_fPosX = 0.0f;
    protected float m_fPosY = 0.0f;
    protected Vector3 m_pos = new Vector3();

    protected ulong m_ulAction = 0;
    protected ulong m_ulOldAction = 0;
    protected ePhtatus m_curStatus = ePhtatus.Stop;

    protected float m_fHitRecoverTimer = 0.0f;
    protected float m_fLayTimer = 0.0f;
    protected float m_fLayUpTimer = 0.0f;
    protected float m_fJumpTimer = 0.0f;

    public float m_fPositionX
    {
        get
        {
            return m_fPosX;
        }
    }
    public float m_fPositionY
    {
        get
        {
            return m_fPosY;
        }
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        CheckMapRoleChangeMap();
        UpdateAction();
        UpdateMove();
    }

    /// <summary>
    /// 设置Role初始数据 创建gameObject
    /// </summary>
    /// <param name="baseInfo"></param>
    /// <param name="initInfo"></param>
    public bool InitBaseRole(string handleName, BaseRoleInfo baseInfo, MapRoleInfo initInfo)
    {
        if (baseInfo == null || initInfo == null)
        {
            m_bStarted = false;
            return false;
        }
        // 初始化数据
        m_sHandleName = handleName;
        m_eRoleType = (eRoleType)initInfo.RoleType;
        m_baseInfo = baseInfo;
        m_initInfo = initInfo;
        m_fWidth = baseInfo.Width;
        m_fHeight = baseInfo.Height;
        m_fPosX = initInfo.InitPosX;
        m_fPosY = initInfo.InitPosY;
        int curHp = initInfo.InitHP;
        if (curHp == -1)
        {
            m_iCurHp = m_baseInfo.MaxHp;
        }
        else
        {
            m_iCurHp = curHp;
        }
        m_ulAction = (ulong)initInfo.InitStatus;
        m_curStatus = (ePhtatus)m_ulAction;
        m_bIsStatic = initInfo.IsStatic;
        m_bCanMove = initInfo.CanMove;

        // 初始化ShowObject
        GameObject prefabObj = Resource.LoadObj(m_baseInfo.PrefabName) as GameObject;
        if (null == prefabObj)
        {
            m_bStarted = false;
            return false;
        }
        GameObject showObj = GameObject.Instantiate(prefabObj);
        showObj.name = "actor";
        GlobalVarFun.AttachChild(gameObject, showObj);
        m_showObj = showObj;

        // 初始化m_mapRole
        if (m_mapRole == null)
        {
            m_mapRole = new MapRole(this);
        }
        m_mapRole.OnChangeMap(MapManager.Instance.curMapWorld, m_fPosX, m_fPosY, m_fWidth, m_fHeight);
        m_mapRole.SetInitData(m_baseInfo, m_initInfo);


        Clear();
        m_bStarted = true;
        return true;
    }

    public virtual void Clear()
    {
        m_ulAction = 0;
        m_ulOldAction = 0;
        m_curStatus = ePhtatus.Stop;

        m_fHitRecoverTimer = 0.0f;
        m_fLayTimer = 0.0f;
        m_fLayUpTimer = 0.0f;
        m_fJumpTimer = 0.0f;
    }

    #region GET/SET

    public bool GetCanMove()
    {
        return m_bCanMove;
    }

    public void SetCanMove(bool value)
    {
        m_bCanMove = value;
    }

    #endregion GET/SET

    #region UpdateFuncs
    #region Update Physics
    /// <summary>
    /// 检查Role是否换了地图
    /// </summary>
    private void CheckMapRoleChangeMap()
    {
        if (m_mapRole != null && MapManager.Instance != null && MapManager.Instance.curMapWorld != null)
        {
            if (m_mapRole.m_curMap == null || MapManager.Instance.curMapWorld != m_mapRole.m_curMap)
            {
                m_mapRole.OnChangeMap(MapManager.Instance.curMapWorld, m_fPosX, m_fPosY, m_fWidth, m_fHeight);
                m_mapRole.SetInitData(m_baseInfo, m_initInfo);
            }
        }
    }

    /// <summary>
    /// 物理状态更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void UpdatePhysic(float deltaTime)
    {
        if (m_bStarted == false)
        {
            return;
        }
        if (m_bIsStatic == true)
        {
            return;
        }

        UpdatePhysicNode(deltaTime);
    }

    /// <summary>
    /// 更新MapRole
    /// </summary>
    /// <param name="deltaTime"></param>
    protected void UpdatePhysicNode(float deltaTime)
    {
        if (m_mapRole == null)
            return;

        m_mapRole.Update(deltaTime);
    }

    #endregion Update Physics

    protected virtual void UpdateMove()
    {
        if (m_mapRole == null)
        {
            return;
        }
        m_fPosX = m_mapRole.m_position.X;
        m_fPosY = m_mapRole.m_position.Y;
        m_pos.x = m_fPosX;
        m_pos.y = m_fPosY;
        transform.position = m_pos;
    }

    protected virtual void UpdateAction()
    {
        m_ulOldAction = m_ulAction;
        m_ulAction = GlobalVarFun.ACTION_STOP;
    }

    #endregion UpdateFuncs

    #region Actions
    /// <summary>
    /// 检测除了移动之外的一些动作是否可以做
    /// </summary>
    /// <param name="actionId"></param>
    /// <returns></returns>
    public bool TryAction(ref ulong actionId)
    {
        if (null == m_mapRole || null == m_mapRole.m_body || null == m_mapRole.m_curMap)
        {
            return false;
        }

        //ulong retAct = 0;
        //bool isAct = false;

        if ((actionId & GlobalVarFun.ACTION_DEATH) != 0)
        {
            actionId = GlobalVarFun.ACTION_DEATH;
            return true;
        }
        if (IsBeControlled())
        {
            return false;
        }
        
        //actionId = retAct;
        return true;
    }

    /// <summary>
    /// 实现除了移动之外的一些动作
    /// </summary>
    /// <param name="actionId"></param>
    /// <returns></returns>
    public bool DoAction(ulong actionId)
    {
        //ulong retact = 0;
        //bool isAct = false;
        if (m_curStatus == ePhtatus.HitRecover && m_fHitRecoverTimer > 0.0f)
        {
            return false;
        }
        if (m_curStatus == ePhtatus.Lay && m_fLayTimer > 0.0f)
        {
            return false;
        }
        if (m_curStatus == ePhtatus.LayUp && m_fLayUpTimer > 0.0f)
        {
            return false;
        }
        if (m_curStatus == ePhtatus.Jump && m_fJumpTimer > 0.0f)
        {
            return false;
        }


        if ((actionId & GlobalVarFun.ACTION_ATK1) != 0)
        {
            if (Attack1())
            {
            }
        }

        //if (ShouldRemoveStopAction(actionId))
        //{
        //    actionId &= ~GlobalVarFun.ACTION_STOP;
        //}

        //if ((actionId & GlobalVarFun.ACTION_RUN_RIGHT) != 0)
        //{
        //    if (RunRight())
        //    {
        //        retact = actionId;
        //        isAct = true;
        //    }
        //}
        //if ((actionId & GlobalVarFun.ACTION_RUN_LEFT) != 0)
        //{
        //    if (RunLeft())
        //    {
        //        retact = actionId;
        //        isAct = true;
        //    }
        //}
        //if ((actionId & GlobalVarFun.ACTION_RUN_UP) != 0)
        //{
        //    if (RunUp())
        //    {
        //        retact = actionId;
        //        isAct = true;
        //    }
        //}
        //if ((actionId & GlobalVarFun.ACTION_RUN_DOWN) != 0)
        //{
        //    if (RunDown())
        //    {
        //        retact = actionId;
        //        isAct = true;
        //    }
        //}
        //if ((actionId & GlobalVarFun.ACTION_STOP) != 0)
        //{
        //    if (Stop())
        //    {
        //        retact = actionId;
        //        isAct = true;
        //    }
        //}

        //actionId = retact;
        //return isAct;
        return true;
    }

    public bool IsBeControlled()
    {
        return false;
    }

    public ePhtatus GetPhStatus()
    {
        return m_curStatus;
    }

    #region Move
    public bool RunRight()
    {
        m_mapRole.SetCurVelocityX(m_baseInfo.VelocityX);
        return true;
    }

    public bool RunLeft()
    {
        m_mapRole.SetCurVelocityX(-m_baseInfo.VelocityX);
        return true;
    }

    public bool StopHorizontal()
    {
        m_mapRole.SetCurVelocityX(0);
        return true;
    }
    public bool RunUp()
    {
        m_mapRole.SetCurVelocityY(m_baseInfo.VelocityY);
        return true;
    }

    public bool RunDown()
    {
        m_mapRole.SetCurVelocityY(-m_baseInfo.VelocityY);
        return true;
    }

    public bool StopVertical()
    {
        m_mapRole.SetCurVelocityY(0);
        return true;
    }

    public bool Stop()
    {
        m_mapRole.SetCurVelocityX(0);
        m_mapRole.SetCurVelocityY(0);
        return true;
    }
    #endregion Move

    #region SetActions
    public void SetStopAction()
    {
        m_ulAction |= GlobalVarFun.ACTION_STOP;
    }

    public void SetJumpAction()
    {
        m_ulAction |= GlobalVarFun.ACTION_JUMP;
    }

    public void SetLayAction()
    {
        m_ulAction |= GlobalVarFun.ACTION_LAY;
    }

    public void SetLayUpAction()
    {
        m_ulAction |= GlobalVarFun.ACTION_LAYUP;
    }

    public void SetAtk1Action()
    {
        m_ulAction |= GlobalVarFun.ACTION_ATK1;
    }

    #endregion SetActions

    #region DoActions

    public bool Attack1()
    {
        if (DamageBodyManager.Instance != null)
        {
            DamageBodyManager.Instance.TestAddDamageBody(this);
        }
        return true;
    }

    #endregion DoActions

    #endregion Actions

    #region Transform 

    public void SetTransformZ(float z)
    {
        m_pos.z = z;
        transform.position = m_pos;
    }

    #endregion Transform

    #region Battle

    /// <summary>
    /// 判断人物是否可以受击
    /// </summary>
    /// <param name="damageBody"></param>
    /// <returns></returns>
    public virtual bool OnCanDamage(DamageBody damageBody)
    {
        return true;
    }

    /// <summary>
    /// 人物受击
    /// </summary>
    /// <param name="damageBody"></param>
    public virtual void OnDamage(DamageBody damageBody)
    {

    }

    public virtual void OnAfterDamage(DamageBody damageBody)
    {

    }

    public virtual void OnSeparationDamage(DamageBody damageBody)
    {

    }
    #endregion Battle
}
