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
/// 碰撞体虚拟接口
/// </summary>
public interface PhysicNode {
    Vector2 m_position { get; set; }
    //short m_collisionPhase { set; }
    void Clear();
    void InitB2Body(MapWorld world, float x, float y, float width, float height, BodyType bodyType = BodyType.Static, short id = 0, bool isActive = true);
}

/// <summary>
/// 碰撞体基础类(默认Static)
/// </summary>
public class MapObj : PhysicNode {

    /// <summary>
    /// HandleName
    /// </summary>
    public string m_strHandle;

    #region BODY
    protected Body b2Body;
    public Body m_body
    {
        get { return b2Body; }
    }

    public int m_bodyId
    {
        get
        {
            int id = (null != b2Body) ? b2Body.BodyId : -1;
            return id;
        }
    }

    public BodyType m_bodyType
    {
        get
        {
            if (null != b2Body)
            {
                return b2Body.BodyType;
            }
            else
            {
                return BodyType.Static;
            }
        }
        set
        {
            if (null != b2Body)
            {
                b2Body.BodyType = value;
            }
        }
    }

    /// <summary>
    /// Body Position
    /// </summary>
    public Vector2 m_position
    {
        get
        {
            if (null == b2Body)
            {
                return Vector2.Zero;
            }
            else
            {
                return b2Body.Position;
            }
        }
        set
        {
            if (null != b2Body)
            {
                b2Body.Position = value;
            }
        }
    }

    protected float velocityRatio = 1.0f;
    /// <summary>
    /// 移动速度参数
    /// </summary>
    public float m_velocityRatio
    {
        get
        {
            return velocityRatio;
        }
        set
        {
            velocityRatio = value;
        }
    }

    protected Vector2 lineVelocity = new Vector2(0, 0);
    /// <summary>
    /// 移动速度
    /// </summary>
    public Vector2 m_linearVelocity
    {
        get
        {
            if (null != b2Body)
            {
                return lineVelocity;
                // return b2Body.LinearVelocity;
            }
            else
            {
                return Vector2.Zero;
            }
        }
        set
        {
            if (null != b2Body)
            {
                lineVelocity = value * m_velocityRatio;
                // b2Body.LinearVelocity = value * m_velocityRatio;
            }
        }
    }

    /// <summary>
    /// X轴移动速度
    /// </summary>
    public float m_linearVelocityX
    {
        get
        {
            if (null != b2Body)
            {
                return lineVelocity.X;
                // return b2Body.LinearVelocity.X;
            }
            else
            {
                return 0.0f;
            }
        }
        set
        {
            if (null != b2Body)
            {
                lineVelocity = new Vector2(value * m_velocityRatio, lineVelocity.Y);
                // b2Body.LinearVelocity = new Vector2(value * m_velocityRatio, b2Body.LinearVelocity.Y);
            }
        }
    }

    /// <summary>
    /// Y轴移动速度
    /// </summary>
    public float m_linearVelocityY
    {
        get
        {
            if (null != b2Body)
            {
                return lineVelocity.Y;
                // return b2Body.LinearVelocity.Y;
            }
            else
            {
                return 0.0f;
            }
        }
        set
        {
            if (null != b2Body)
            {
                lineVelocity = new Vector2(lineVelocity.X, value * m_velocityRatio);
                // b2Body.LinearVelocity = new Vector2(b2Body.LinearVelocity.X, value * m_velocityRatio);
            }
        }
    }

    /// <summary>
    /// 摩擦力
    /// </summary>
    public float m_friction
    {
        set
        {
            if (null != b2Body)
            {
                b2Body.Friction = value;
            }
        }
    }

    //public short m_collisionPhase
    //{
    //    set
    //    {
    //        if (null != b2Body)
    //        {
    //            b2Body.CollisionPhase = value;
    //        }
    //    }
    //}

    protected bool isSensor = false;
    /// <summary>
    /// 是否是传感器
    /// </summary>
    public bool m_isSensor
    {
        get
        {
            return isSensor;
        }
        set
        {
            if (null != b2Body)
            {
                b2Body.IsSensor = value;
                isSensor = value;
            }
        }
    }

    /// <summary>
    /// 是否是StaticBody
    /// </summary>
    public bool m_isStatic
    {
        set
        {
            if (null != b2Body)
            {
                b2Body.BodyType = value ? BodyType.Static : BodyType.Dynamic;
            }
        }
        get
        {
            if (null != b2Body)
            {
                return b2Body.IsStatic;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Body是否可休眠
    /// </summary>
    public bool SleepingAllowed
    {
        get
        {
            if (b2Body == null)
            {
                return false;
            }
            return b2Body.SleepingAllowed;
        }
        private set
        {
            if (b2Body == null)
            {
                return;
            }
            b2Body.SleepingAllowed = value;
        }
    }

    protected bool canMove = true;
    /// <summary>
    /// 是否可以移动
    /// </summary>
    public bool m_bCanMove
    {
        set
        {
            canMove = value;
        }
        get
        {
            return canMove;
        }
    }

    // 是否stuck了
    protected bool isStuck = false;
    /// <summary>
    /// 是否stuck了
    /// </summary>
    public bool m_bIsStuck
    {
        set
        {
            isStuck = value;
            if (isStuck == true)
            {
                m_linearVelocity = m_linearVelocity;
            }
        }
        get
        {
            return isStuck;
        }
    }

    protected Vector2 force = new Vector2();
    /// <summary>
    /// 外加力
    /// </summary>
    public Vector2 m_force { get { return force; } set { force = value; } }

    #endregion BODY

    protected MapWorld curMap = null;
    /// <summary>
    /// 当前World
    /// </summary>
    public MapWorld m_curMap
    {
        get { return curMap; }
    }

    protected object userData = null;

    public Vector2 m_cellCenter = Vector2.zero;

    public bool m_isValid
    {
        get { return null != b2Body && null != curMap; }
    }
    public bool m_isActive = true;

    public MapObj()
    {
    }

    public virtual bool IsPlayer { get { return false; } }

    #region Init
    public virtual void InitB2Body(MapWorld mapBase, float x, float y, float width, float height, BodyType bodyType = BodyType.Static, short collisionId = 0, bool isActive = true)
    {
        curMap = mapBase;
        //b2Body = BodyFactory.CreateBody(mapBase.world);
        //b2Body.BodyType = bodyType;
        //b2Body.Position = new Vector2(x, y);
        //b2Body.SleepingAllowed = false;
        //b2Body.Mass = 1;
        //b2Body.CollisionPhase = collisionId;
        //b2Body.LinearDamping = 1.0f;
        //b2Body.UserData = this;
        //Vertices vertices = MapFunctions.MakeBoxVertices(width * 0.5f, height * 0.5f);
        //PolygonShape shape = new PolygonShape(vertices, 1f);
        //Fixture fixture = b2Body.CreateFixture(shape);

        Vector2 position = new Vector2(x, y);
        b2Body = BodyFactory.CreateRectangle(mapBase.b2World, width, height, 0, position, 0, bodyType, this);
        m_isActive = isActive;

        InitCollisionEvent(b2Body.FixtureList[0]);
        //InitCollisionEvent(fixture);
        //InitDefaultGroup(b2Body);
    }

    public virtual void InitB2Rectangle(MapWorld mapBase, float width, float height, float density = 0, Vector2 position = new Vector2(), float rotation = 0, BodyType bodyType = BodyType.Static, short collisionId = 0, bool isActive = true)
    {
        curMap = mapBase;
        b2Body = BodyFactory.CreateRectangle(mapBase.b2World, width, height, density, position, rotation, bodyType, this);
        b2Body.FixedRotation = true;
        m_isActive = isActive;

        InitCollisionEvent(b2Body.FixtureList[0]);
        //InitDefaultGroup(b2Body);
    }

    public virtual void InitB2Circle(MapWorld mapBase, float radius, float density = 0, Vector2 position = new Vector2(), BodyType bodyType = BodyType.Static, short collisionId = 0, bool isActive = true)
    {
        curMap = mapBase;
        b2Body = BodyFactory.CreateCircle(mapBase.b2World, radius, density, position, bodyType, this);
        m_isActive = isActive;

        InitCollisionEvent(b2Body.FixtureList[0]);
        //InitDefaultGroup(b2Body);
    }

    public virtual void InitB2Polygon(MapWorld mapBase, List<Vector2> points, float density = 0, Vector2 position = new Vector2(), float rotation = 0, BodyType bodyType = BodyType.Static, short collisionId = 0, bool isActive = true)
    {
        curMap = mapBase;
        if (points == null)
        {
            return;
        }
        Vertices vertices = new Vertices(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(points[i]);
        }
        b2Body = BodyFactory.CreatePolygon(mapBase.b2World, vertices, density, position, rotation, bodyType, this);
        m_isActive = isActive;

        InitCollisionEvent(b2Body.FixtureList[0]);
    }

    public virtual void InitB2Edge(MapWorld mapBase, Vector2 start, Vector2 end, short collisionId = 0, bool isActive = true)
    {
        curMap = mapBase;
        b2Body = BodyFactory.CreateEdge(mapBase.b2World, start, end, this);
        m_isActive = isActive;

        InitCollisionEvent(b2Body.FixtureList[0]);
        //InitDefaultGroup(b2Body);
    }

    public virtual void Clear()
    {
        userData = null;
        if (null != b2Body && null != curMap && null != curMap.world)
        {
            curMap.world.RemoveBody(b2Body);
            b2Body.UserData = null;
            b2Body = null;
            curMap = null;
        }
    }

    public T GetUserData<T>() where T : class
    {
        T value = userData as T;
        return value;
    }

    public void SetUserData(object data)
    {
        userData = data;
    }
    #endregion Init

    public virtual bool Update(float deltaTime)
    {
        return true;
    }

    #region BodyFuncs
    public static void InitDefaultGroup(Body body)
    {
        MapFunctions.SetCollisionGroup(body, 0, Category.Cat1, Category.Cat31);
    }

    public virtual void InitCollisionEvent(Fixture fixture)
    {
        fixture.BeforeCollision += OnBeforeCollision;
        fixture.AfterCollision += OnAfterCollision;
        fixture.OnCollision += OnCollision;
        fixture.OnSeparation += OnSeparation;
    }

    protected virtual bool OnBeforeCollision(Fixture fixtureA, Fixture fixtureB)
    {
        return true;
    }

    protected virtual void OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {

    }

    protected virtual void OnAfterCollision(Fixture fixtureA, Fixture fixtureB, Contact contact, ContactVelocityConstraint impulse)
    {

    }

    protected virtual void OnSeparation(Fixture fixtureA, Fixture fixtureB, Contact contact)
    {

    }

    #endregion BodyFuncs

}