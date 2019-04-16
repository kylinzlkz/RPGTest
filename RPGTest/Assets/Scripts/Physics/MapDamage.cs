using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using VelcroPhysics.Collision.ContactSystem;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Dynamics.Solver;

/// <summary>
/// 伤害体碰撞体
/// </summary>
public class MapDamage : MapObj {

    public int skillID;
    public MapRole hostMapRole = null;
    public DamageBody damageBody = null;

    public delegate void OnCollisionFunc();
    public OnCollisionFunc onCollisionFunc = null;

    public eDamageBoundType boundType = eDamageBoundType.None;

    /// <summary>
    /// 是否是Bullet
    /// </summary>
    public bool m_bIsBullet
    {
        get
        {
            return false;
        }
        set
        {

        }
    }

    public MapDamage(DamageBody damageBody)
    {
        this.damageBody = damageBody;
        m_strHandle = damageBody.m_sHandleName;
        skillID = damageBody.m_iSkillID;
        hostMapRole = damageBody.m_host.m_mapRole;
        boundType = damageBody.m_eBoundType;
    }

    public override void InitB2Body(MapWorld mapBase, float x, float y, float width, float height, BodyType bodyType = BodyType.Static, short collisionId = 0, bool isActive = true)
    {
        base.InitB2Body(mapBase, x, y, width, height, BodyType.Dynamic);
    }

    public override bool Update(float deltaTime)
    {
        if (null == b2Body)
        {
            return false;
        }
        UpdateMove(deltaTime);

        return base.Update(deltaTime);
    }

    #region GET/SET

    public void SetCurVelocityX(float x)
    {
        m_linearVelocityX = x;
    }

    public void SetCurVelocityY(float y)
    {
        m_linearVelocityY = y;
    }

    #endregion GET/SET

    #region UpdateFuncs
    /// <summary>
    /// 伤害体位移
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateMove(float deltaTime)
    {
        if (false == canMove)
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

    #region BodyFunc
    // 当伤害体碰撞到Collider时：
    // 如果是Destroy类型的则应该设置速度为0，并发送回调（立即销毁/延迟销毁）
    // 如果是Bound类型的则应该根据速度，反弹，发送回调（原方向/法线对称）
    // 如果是Cross类型则应该已经在OnBeforeCollision中处理了

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

        //UnityEngine.Debug.Log("伤害体碰撞 Before");
        MapCollider collider = other.Body.UserData as MapCollider;
        if (collider != null)
        {
            //UnityEngine.Debug.Log("  OnBeforeCollision  collider  ");
            if (boundType == eDamageBoundType.Cross)
            {
                return false; // Cross类型直接通过
            }
            if (collider.m_isSensor == true)
            {
                return false;
            }
        }

        MapRole role = other.Body.UserData as MapRole;
        if (role != null)
        {
            if (role == hostMapRole)
            {
                return false; // 是主人放出来的 不发生碰撞
            }
            return role.OnCanDamage(damageBody); // 角色是否可以受击
        }
        // 场景物体是否可以受击 TODO
        return true;
        //return base.OnBeforeCollision(fixtureA, fixtureB);
    }

    protected override void OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {
        Fixture other = (fixtureA.Body == b2Body) ? fixtureB : fixtureA;

        //UnityEngine.Debug.Log("伤害体碰撞 On");
        MapCollider collider = other.Body.UserData as MapCollider;
        if (collider != null)
        {
            //UnityEngine.Debug.Log("  OnCollision  collider  ");
            // 如果是Destroy类型的则应该设置速度为0，并发送回调（立即销毁/延迟销毁）
            // 如果是Bound类型的则应该根据速度，反弹，发送回调（原方向/法线对称）
            // 如果是Cross类型则应该已经在OnBeforeCollision中处理了
            if (false == collider.m_isSensor)
            {
                damageBody.OnColliderCollision(collider.m_strHandle);
            }
            // 场景物体受击 TODO
        }

        // 能执行到OnCollision说明角色可受击
        MapRole role = other.Body.UserData as MapRole;
        if (role != null)
        {
            //UnityEngine.Debug.Log("  OnCollision  MapRole  ");
            damageBody.AddDamageRole(role.m_strHandle);
        }

        //base.OnCollision(fixtureA, fixtureB, contact);
    }

    protected override void OnAfterCollision(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse)
    {
        if (null == b2Body)
        {
            return;
        }
        Fixture other = (fixtureA.Body == b2Body) ? fixtureB : fixtureA;

        //UnityEngine.Debug.Log("伤害体碰撞 After");
        MapCollider collider = other.Body.UserData as MapCollider;
        if (collider != null)
        {
            //UnityEngine.Debug.Log("  OnAfterCollision  collider  ");
        }
        // 分离受击
        MapRole role = other.Body.UserData as MapRole;
        if (role != null)
        {
            //UnityEngine.Debug.Log("  OnAfterCollision  MapRole  ");
            damageBody.RemoveDamageRole(role.m_strHandle);
        }

        //base.OnAfterCollision(fixtureA, fixtureB, contact, impulse);
    }

    protected override void OnSeparation(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {
        if (null == b2Body)
        {
            return;
        }
        Fixture other = (fixtureA.Body == b2Body) ? fixtureB : fixtureA;

        //UnityEngine.Debug.Log("伤害体碰撞 Separation");
        MapCollider collider = other.Body.UserData as MapCollider;
        if (collider != null)
        {
            //UnityEngine.Debug.Log("  OnSeparation  collider  ");
        }
        // 分离受击
        MapRole role = other.Body.UserData as MapRole;
        if (role != null)
        {
            //UnityEngine.Debug.Log("  OnSeparation  MapRole  ");
            damageBody.RemoveDamageRole(role.m_strHandle);
        }

        //base.OnSeparation(fixtureA, fixtureB, contact);
    }

    #endregion BodyFunc
}
