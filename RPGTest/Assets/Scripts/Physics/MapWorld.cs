using System;
using System.Collections;
using System.Collections.Generic;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Shared;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics.Joints;
using VelcroPhysics.Collision.ContactSystem;
using VelcroPhysics.Collision.Narrowphase;
using VelcroPhysics.Dynamics.Solver;

/// <summary>
/// world基础类
/// </summary>
public class MapWorld : IDisposable {

    public const int MaxQuerySize = 50;

    internal World b2World;
    public World world
    {
        get
        {
            return b2World;
        }
        set
        {
            b2World = value;
        }
    }

    public double curTime { get; private set; } = 0;

    // 当前地图数值数据
    private MapDataNode mapData = null;

    public MapCollider baseCollision { get; private set; } = null;

    //private AABB b2Bound = new AABB();

    public Dictionary<string, MapCollider> colliderDic = new Dictionary<string, MapCollider>();
    public Dictionary<string, MapCollider> speColliderDic = new Dictionary<string, MapCollider>();

    public virtual bool Init(MapDataNode mapData)
    {
        if (mapData == null)
        {
            return false;
        }

        this.mapData = mapData;

        b2World = new World(new Vector2(0f, 0f)); // 2.5D 不需要重力

        b2World.JointRemoved += OnB2JointRemoved;
        b2World.ContactManager.PreSolve += OnB2PreSolve;
        b2World.ContactManager.PostSolve += OnB2PostSolve;
        b2World.ContactManager.BeginContact += OnB2BeginContact;
        b2World.ContactManager.EndContact += OnB2EndContact;

        baseCollision = new MapCollider();
        baseCollision.InitB2Body(this, 0, 0, 1, 1);
        MapFunctions.SetCollisionGroup(baseCollision.m_body, 0, VelcroPhysics.Collision.Filtering.Category.None, VelcroPhysics.Collision.Filtering.Category.None); // 不与任何碰撞

        colliderDic.Clear();
        speColliderDic.Clear();

        return true;
    }

    /// <summary>
    /// 读取数据生成配置的Colliders
    /// </summary>
    /// <param name="mapData"></param>
    public void CreateColliders(List<ColliderDataNode> mapData)
    {
        if (mapData == null)
            return;
        for (int i = 0; i < mapData.Count; i++)
        {
            MapCollider col = CreateCollider(mapData[i]);
            if (col != null)
            {
                colliderDic.Add(col.m_strHandle, col);
            }
        }
    }

    /// <summary>
    /// 创建单个Collider，Collider不会移动，一定会发生碰撞，如果不发生碰撞则创建特殊的碰撞体
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public MapCollider CreateCollider(ColliderDataNode data)
    {
        if (data == null)
        {
            return null;
        }
        MapCollider col = new MapCollider();
        col.InitData(data);
        col.m_strHandle = data.name; // GetColliderStrHandle(data.name);
        if (data.bodyCreateType == (int)eBodyCreateType.Rectangle)
        {
            col.InitB2Rectangle(this, data.sizeX, data.sizeY, 0.0f, data.position, 0, (BodyType)data.bodyType, 0, data.isActive);
        }
        else if (data.bodyCreateType == (int)eBodyCreateType.Circle)
        {
            col.InitB2Circle(this, data.radius, 0.0f, data.position, (BodyType)data.bodyType, 0, data.isActive);
        }
        else if (data.bodyCreateType == (int)eBodyCreateType.Polygon)
        {
            col.InitB2Polygon(this, data.vertices, 0.0f, data.position, 0, (BodyType)data.bodyType, 0, data.isActive);
        }
        else if (data.bodyCreateType == (int)eBodyCreateType.Edge)
        {
            col.InitB2Edge(this, data.startPos, data.endPos, 0, data.isActive);
        }
        col.m_isSensor = data.isSensor;

        return col;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="strhandle"></param>
    /// <returns></returns>
    public bool RemoveMapCollider(string strhandle)
    {
        if (strhandle == null)
        {
            return false;
        }
        return true;
    }

    public virtual void Update(float deltaTime, double time)
    {
        curTime = time;
        b2World.Step(deltaTime);
    }

    public void Dispose()
    {
        if (b2World != null)
        {
            b2World.Clear();
        }
        colliderDic.Clear();
        speColliderDic.Clear();
        b2World = null;
    }


    protected virtual void OnB2JointRemoved(Joint joint)
    {

    }

    protected virtual void OnB2PreSolve(Contact contact, ref Manifold oldManifold)
    {
        //UnityEngine.Debug.Log("OnB2PreSolve : ");
        // 特殊处理一下爬梯子的状态
        //MapRole mapRole = null;
        //MapCollider mapCollider = null;
        //if (contact.FixtureA.Body.UserData is MapCollider)
        //{
        //    mapRole = contact.FixtureB.Body.UserData as MapRole;
        //    mapCollider = contact.FixtureA.Body.UserData as MapCollider;
        //}
        //else if (contact.FixtureB.Body.UserData is MapCollider)
        //{
        //    mapRole = contact.FixtureA.Body.UserData as MapRole;
        //    mapCollider = contact.FixtureB.Body.UserData as MapCollider;
        //}

        //if (null != mapRole && null != mapCollider)
        //{
        //    if (mapCollider.m_dataNode.isOneWay)
        //    {
        //        float vDot = Vector2.Dot(mapRole.m_linearVelocity, mapCollider.m_dataNode.oneWayDir);
        //        if (vDot > 0.5f)
        //        {
        //            contact.Enabled = false;
        //            UnityEngine.Debug.Log("WORLD CONTACT FALSE : ");
        //        }
        //        else
        //        {
        //            contact.Enabled = true;
        //        }
        //    }
        //    else
        //    {
        //        contact.Enabled = true;
        //    }
        //    if (mapCollider.m_dataNode.isStuckDir)
        //    {
        //        contact.Enabled = true;
        //    }
        //}
        //contact.Enabled = true;
    }

    protected virtual void OnB2PostSolve(Contact acontact, ContactVelocityConstraint impuse)
    {
        //UnityEngine.Debug.Log("OnB2PostSolve : ");

    }

    protected virtual bool OnB2BeginContact(Contact contact)
    {
        //UnityEngine.Debug.Log("OnB2BeginContact : ");
        return true;
    }

    protected virtual void OnB2EndContact(Contact contact)
    {
        //UnityEngine.Debug.Log("OnB2EndContact : ");

    }

}
