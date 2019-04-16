using System.Collections;
using System.Collections.Generic;
using VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using VelcroPhysics.Collision.Filtering;
using VelcroPhysics.Collision.ContactSystem;
using VelcroPhysics.Dynamics.Solver;


// TODO : 1. 地表变化后 更改摩擦力系数 （可能需要传感器?）

/// <summary>
/// 角色碰撞体
/// </summary>
public class MapRole : MapObj {

    public class CollisionInfo
    {
        public Fixture fixture;
        public int fixtureID;
        public Vector2 impulseNormal;
    }

    public BaseRole m_baseRole = null;
    private int m_iCollisionCount = 0;

    private List<CollisionInfo> collisionInfoList = new List<CollisionInfo>();
    private Dictionary<string, string> m_forceFieldDic = new Dictionary<string, string>();

    public int m_iDirX = 0;
    public int m_iDirY = 0;
    public Vector2 MaxVelocity { get; set; } = Vector2.One;

    public MapRole(BaseRole baseRole)
    {
        m_baseRole = baseRole;
        m_strHandle = baseRole.m_sHandleName;
    }

    public override bool Update(float deltaTime)
    {
        if (null == b2Body)
        {
            return false;
        }

        UpdateMove(deltaTime);
        // UnityEngine.Debug.Log("velocity : " + m_linearVelocity);

        return base.Update(deltaTime);
    }

    public override void Clear()
    {
        m_iCollisionCount = 0;
        ClearCollisionInfo();
        m_forceFieldDic.Clear();
        ClearAction();

        base.Clear();
    }

    #region GET/SET
    public void SetInitData(BaseRoleInfo baseInfo, MapRoleInfo node)
    {
        // 添加Map的速度参数
        MaxVelocity = new Vector2(baseInfo.VelocityX + 0.1f, baseInfo.VelocityY + 0.1f);
        if (m_body != null)
        {
            //MapFunctions.SetCollisionGroup(m_body, 0, node.InitCategories, node.InitCollideCats);
        }
    }

    public void SetCurVelocityX(int dir, float x)
    {
        if (m_bIsStuck)
        {
            return; // 如果stuck了不可以改变速度
        }
        if (Math.Abs(x) > MaxVelocity.X)
        {
            x = MaxVelocity.X;
        }
        m_iDirX = dir;
        m_linearVelocityX = dir * x;
    }

    public void SetCurVelocityY(int dir, float y)
    {
        if (m_bIsStuck)
        {
            return; // 如果stuck了不可以改变速度
        }
        if (Math.Abs(y) > MaxVelocity.Y)
        {
            y = MaxVelocity.Y;
        }
        m_iDirY = dir;
        m_linearVelocityY = dir * y;
    }

    //public void AddCurVelocityX(float x, bool lockDir = false)
    //{
    //    if (m_bIsStuck)
    //    {
    //        return; // 如果stuck了不可以改变速度
    //    }
    //    float curVelX = m_linearVelocityX + x;
    //    // 锁定方向添加速度
    //    if (lockDir == true && curVelX * x < 0)
    //    {
    //        curVelX = 0.0f;
    //    }
    //    m_linearVelocityX = curVelX;
    //    if (Math.Abs(m_linearVelocityX) > MaxVelocity.X)
    //    {
    //        if (m_linearVelocityX < 0)
    //        {
    //            m_linearVelocityX = -MaxVelocity.X;
    //        }
    //        else
    //        {
    //            m_linearVelocityX = MaxVelocity.X;
    //        }
    //    }
    //}

    //public void AddCurVelocityY(float y, bool lockDir = false)
    //{
    //    if (m_bIsStuck)
    //    {
    //        return; // 如果stuck了不可以改变速度
    //    }
    //    float curVelY = m_linearVelocityY + y;
    //    // 锁定方向添加速度
    //    if (lockDir == true && curVelY * y < 0)
    //    {
    //        curVelY = 0.0f;
    //    }
    //    m_linearVelocityY = curVelY;
    //    if (Math.Abs(m_linearVelocityY) > MaxVelocity.Y)
    //    {
    //        if (m_linearVelocityY < 0)
    //        {
    //            m_linearVelocityY = -MaxVelocity.Y;
    //        }
    //        else
    //        {
    //            m_linearVelocityY = MaxVelocity.Y;
    //        }
    //    }
    //}

    #endregion GET/SET

    #region UpdateFuncs

    protected void UpdateMove(float deltaTime)
    {
        if (false == m_bCanMove)
        {
            return;
        }

        Vector2 newPos = new Vector2(m_position.X, m_position.Y);
        newPos += m_linearVelocity * deltaTime;

        Vector2 dist = new Vector2(m_position.X - newPos.X, m_position.Y - newPos.Y);
        float distSqr = dist.LengthSquared();
        //UnityEngine.Debug.Log("Speed : " + m_linearVelocity + " ; " + dist + " ; " + distSqr);
        if (distSqr > GlobalVarFun.MoveDistSqrMaxThreshhold)
        {
            newPos.X = m_position.X;
            newPos.Y = m_position.Y;
        }
        else if (distSqr > GlobalVarFun.MoveDistSqrMinThreshhold)
        {
            //m_curVelocity = m_curVelocity * 0.7f + dist / deltaTime;
            newPos.X = m_position.X;
            newPos.Y = m_position.Y;
            newPos += m_linearVelocity * deltaTime;
        }
        m_position = newPos;
    }

    #endregion UpdateFuncs

    public void OnChangeMap(MapWorld mapworld, float posX, float posY, float width, float height, short collisionId = 0)
    {
        InitB2Rectangle(mapworld, width, height, 10, new Vector2(posX, posY), 0, BodyType.Dynamic, collisionId);

        m_body.SleepingAllowed = false;
        m_iCollisionCount = 0;
        ClearCollisionInfo();
        m_forceFieldDic.Clear();
        ClearAction();
    }

    public void ClearCollisionInfo()
    {
        collisionInfoList.Clear();
    }

    public void SetAction(int action)
    {

    }

    public void ClearAction()
    {
    }


    #region BodyFunc
    protected override bool OnBeforeCollision(Fixture fixtureA, Fixture fixtureB)
    {
        if (null == b2Body)
        {
            return false;
        }
        if (!m_isActive)
        {
            return false;
        }
        Fixture other = (fixtureA.Body == b2Body) ? fixtureB : fixtureA;
        MapObj obj = other.Body.UserData as MapObj;
        if (obj == null)
        {
            return false;
        }

        //UnityEngine.Debug.Log("人物碰撞 Before");
        MapCollider collider = other.Body.UserData as MapCollider;
        // 是单向通过
        if (collider != null && collider.isOneWay)
        {
            float vDot = Vector2.Dot(m_linearVelocity, collider.oneWayDir);
            if (vDot > 0.5f)
            {
                return false; // 移动方向和可通行方向相同 可通过
            }
        }

        return true;
    }

    protected override void OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {
        //UnityEngine.Debug.Log("OnCollision : ");
        Fixture other = (fixtureA.Body == b2Body) ? fixtureB : fixtureA;
        MapObj obj = other.Body.UserData as MapObj;
        if (obj == null)
        {
            return;
        }
        //UnityEngine.Debug.Log("人物碰撞 On");
        //MapCollider collider = other.Body.UserData as MapCollider;
        //if (collider != null)
        //{
        //    // 是传感器 特殊处理
        //    if (collider.m_isSensor)
        //    {
        //        // 速度改变
        //        collider.SaveOldSpeedRatio(m_velocityRatio);
        //        m_velocityRatio = collider.speedRatio;
        //        // 移动方向固定
        //        if (collider.isStuckDir == true)
        //        {
        //            m_bIsStuck = true;
        //        }
        //        // 外加力
        //        if (collider.forceDir.LengthSquared() > 0.1f)
        //        {
        //            m_force = m_force + collider.forceDir;
        //        }
        //    }
        //}
        
    }

    protected override void OnAfterCollision(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse)
    {
        //UnityEngine.Debug.Log("OnAfterCollision : ");
        if (null == b2Body)
        {
            return;
        }
        Fixture other = (fixtureA.Body == b2Body) ? fixtureB : fixtureA;
        MapObj obj = other.Body.UserData as MapObj;
        if (obj == null)
        {
            return;
        }
        //UnityEngine.Debug.Log("人物碰撞 After");
    }

    protected override void OnSeparation(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {
        //UnityEngine.Debug.Log("OnSeparation : ");
        if (null == b2Body)
        {
            return;
        }
        Fixture other = (fixtureA.Body == b2Body) ? fixtureB : fixtureA;
        MapObj obj = other.Body.UserData as MapObj;
        if (obj == null)
        {
            return;
        }
        ////UnityEngine.Debug.Log("人物碰撞 Separation");
        //MapCollider collider = other.Body.UserData as MapCollider;
        //if (collider != null)
        //{
        //    // 是传感器 特殊处理
        //    if (collider.m_isSensor)
        //    {
        //        // 恢复速度参数
        //        m_velocityRatio = collider.GetOldSpeedRatio();
        //        // 移动方向固定
        //        if (collider.isStuckDir == true)
        //        {
        //            m_bIsStuck = false;
        //        }
        //        // 外加力
        //        if (collider.forceDir.LengthSquared() > 0.1f)
        //        {
        //            obj.m_force = obj.m_force - collider.forceDir;
        //        }
        //    }
        //}
    }

    #endregion BodyFunc


    #region CollisionFuncs

    /// <summary>
    /// 设置不可动 并在time秒后设置为可动 TODO
    /// </summary>
    public void DelayCanMove(float time)
    {
        m_bCanMove = false;

    }

    /// <summary>
    /// 判断该伤害体是否可以造成伤害
    /// </summary>
    /// <param name="damageBody"></param>
    /// <returns></returns>
    public bool OnCanDamage(DamageBody damageBody)
    {
        // 如不伤害友军？等等
        return m_baseRole.OnCanDamage(damageBody); ;
    }

    /// <summary>
    /// Role受击
    /// </summary>
    /// <param name="damageBody"></param>
    public void OnDamage(DamageBody damageBody)
    {
        m_baseRole.OnDamage(damageBody);
    }

    public void OnAfterDamage(DamageBody damageBody)
    {
        m_baseRole.OnAfterDamage(damageBody);
    }

    public void OnSeparationDamage(DamageBody damageBody)
    {
        m_baseRole.OnSeparationDamage(damageBody);
    }


    #endregion CollisionFuncs
}
