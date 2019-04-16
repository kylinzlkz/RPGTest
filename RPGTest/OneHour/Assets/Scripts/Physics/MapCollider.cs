using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using VelcroPhysics.Collision.ContactSystem;
using VelcroPhysics.Collision.Filtering;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Dynamics.Solver;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

/// <summary>
/// Static碰撞体
/// </summary>
public class MapCollider : MapObj
{

    public BaseCollider m_baseCollider = null;

    public bool isBreakable = false;  // 是否可破坏
    public string breakMainName = "";   // 可破坏物件的主物件名
    public float speedRatio = 1.0f;
    public bool isOneWay = false;
    public Vector2 oneWayDir = new Vector2(); // 单向阻挡的方向
    public bool isStuckDir = false; // 是否锁定移动方向
    public Vector2 forceDir = new Vector2(); // 外加力方向
    public ColliderDataNode m_dataNode { get; set; } = null;

    private float oldObjSpeedRatio = 1.0f;

    // 动态碰撞过滤
    //public delegate bool DynamicFilter(MapObj role, MapCollider collider);
    //public DynamicFilter dynamicFilter = null;

    public MapCollider()
    {

    }

    public MapCollider(BaseCollider collider)
    {
        m_baseCollider = collider;
    }

    public void InitData(ColliderDataNode data)
    {
        isBreakable = data.isBreakable;
        breakMainName = data.breakMainName;
        speedRatio = data.speedRatio;
        isOneWay = data.isOneWay;
        oneWayDir = data.oneWayDir;
        isStuckDir = data.isStuckDir;
        forceDir = data.forceDir;
        m_dataNode = data;
    }

    public override void Clear()
    {

        //dynamicFilter = null;
        curMap.RemoveMapCollider(m_strHandle);

        base.Clear();
    }


    #region BodyFunc

    protected override bool OnBeforeCollision(Fixture fixtureA, Fixture fixtureB)
    {
        //UnityEngine.Debug.Log("OnBeforeCollision : ");
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
        // 是传感器 特殊处理
        //if (m_isSensor)
        //{
        //    // 传感器不影响Damage移动
        //    MapDamage damage = other.Body.UserData as MapDamage;
        //    if (damage != null)
        //    {
        //        return;
        //    }

        //    // 速度改变
        //    oldObjSpeedRatio = obj.m_velocityRatio;
        //    obj.m_velocityRatio = m_dataNode.speedRatio;
        //    // 移动方向固定
        //    if (m_dataNode.isStuckDir == true)
        //    {
        //        stuckVelocity = obj.m_linearVelocity;
        //        obj.m_bIsStuck = true;
        //    }
        //    // 外加力
        //    if (m_dataNode.forceDir.LengthSquared() > 0.1f)
        //    {
        //        obj.m_force = obj.m_force + m_dataNode.forceDir;
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
        // 是传感器 特殊处理
        //if (isSensor)
        //{
        //    // 传感器不影响Damage移动
        //    MapDamage damage = other.Body.UserData as MapDamage;
        //    if (damage != null)
        //    {
        //        return;
        //    }
        //    // 恢复速度参数
        //    obj.m_velocityRatio = oldObjSpeedRatio;
        //    // 移动方向固定
        //    if (m_dataNode.isStuckDir == true)
        //    {
        //        obj.m_bIsStuck = false;
        //    }// 外加力
        //    if (m_dataNode.forceDir.LengthSquared() > 0.1f)
        //    {
        //        obj.m_force = obj.m_force - m_dataNode.forceDir;
        //    }
        //}
    }

    #endregion BodyFunc

    #region InteractFunc
    public void SaveOldSpeedRatio(float ratio)
    {
        oldObjSpeedRatio = ratio;
    }

    public float GetOldSpeedRatio()
    {
        return oldObjSpeedRatio;
    }


    #endregion InteractFunc
}
