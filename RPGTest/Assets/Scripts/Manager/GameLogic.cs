using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    private static GameLogic _instance;
    public static GameLogic Instance
    {
        get
        {
            if(_instance == null)
            {
                new GameLogic();
            }
            return _instance;
        }
    }

    public int m_iStartMapId = 101;

    private bool m_bHasLoadMap = false;
    private bool m_bCanLoadMap = true;
    private bool m_bHasLoadRole = false;
    private bool m_bCanLoadRole = true;

    private float m_fCurTime = 0.0f;
    private float m_fDeltaTime = 0.0f;

    public GameLogic()
    {
        _instance = this;
    }

    private void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        ResourceLoader u = ResourceLoader.Instance;
        InputHelper inputHelper = new InputHelper();
        if (MapDataManager.Instance != null)
        {
            MapDataManager.Instance.Init();
        }
        if (RoleDataManager.Instance != null)
        {
            RoleDataManager.Instance.Init();
        }
        if (DamageDataManager.Instance != null)
        {
            DamageDataManager.Instance.TestAddDamageInfo(); // 测试初始化一个伤害体
        }
        if (m_bHasLoadMap == false)
        {
            LoadStartMap();
        }
        if (m_bHasLoadRole == false)
        {
            LoadStartRole();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (m_bCanLoadMap == false)
        {
            return;
        }

        m_fCurTime = GlobalVarFun.GetTime();
        m_fDeltaTime = GlobalVarFun.GetSmoothDeltaTime();

        // 地图更新 (包括普通的Collider)
        if (MapManager.Instance != null)
        {
            MapManager.Instance.Update(m_fDeltaTime, m_fCurTime);
        }
        // Role更新 TODO
        if (RoleManager.Instance != null)
        {
            RoleManager.Instance.Update(m_fDeltaTime, m_fCurTime);
        }
        // 时间事件更新 TODO
        // 伤害体更新 TODO
        if (DamageBodyManager.Instance != null)
        {
            DamageBodyManager.Instance.Update(m_fDeltaTime, m_fCurTime);
        }
    }

    #region DrawGizmos
    void OnDrawGizmos()
    {
        MapManager mapMg = MapManager.Instance;
        if (null != mapMg && null != mapMg.curMapWorld && null != mapMg.curMapWorld.b2World)
        {
            DrawBox2DBodyGizmos(mapMg.curMapWorld.b2World);
            DrawBox2DJointsGizmos(mapMg.curMapWorld.b2World);
        }
    }

    protected void DrawBox2DBodyGizmos(VelcroPhysics.Dynamics.World b2World)
    {
        for (int ib = 0, count = b2World.BodyList.Count; ib < count; ++ib)
        {
            VelcroPhysics.Dynamics.Body body = b2World.BodyList[ib];
            if (null == body)
            {
                continue;
            }

            Color fixtureColor;
            if (body.IsStatic)
            {
                fixtureColor = new Color(1, 0, 1);
            }
            else
            {
                if (body.BodyType == VelcroPhysics.Dynamics.BodyType.Kinematic)
                {
                    fixtureColor = Color.yellow;
                }
                else
                {
                    fixtureColor = Color.green;
                }

                if ((body._flags & VelcroPhysics.Dynamics.BodyFlags.AwakeFlag) == 0)
                {
                    fixtureColor = Color.gray * 0.5f + fixtureColor * 0.5f;
                }
            }

            Microsoft.Xna.Framework.Vector2 center = body.WorldCenter;
            for (int ifix = 0, countFix = body.FixtureList.Count; ifix < countFix; ++ifix)
            {
                VelcroPhysics.Dynamics.Fixture fixture = body.FixtureList[ifix];
                if (fixture.Friction > 5f)
                {
                    fixtureColor.b = 1.0f;
                }
                if (fixture.Shape.ShapeType == VelcroPhysics.Collision.Shapes.ShapeType.Circle)
                {
                    float radius = fixture.Shape.Radius * GlobalVarFun.DisplayToPhysicUnitRatio;
                    Vector3 v3Center = GlobalVarFun.GetDisplayPosition(center.X, center.Y);

                    Gizmos.color = fixtureColor;
                    Gizmos.DrawWireSphere(v3Center, radius);
                }
                else if (fixture.Shape.ShapeType == VelcroPhysics.Collision.Shapes.ShapeType.Polygon)
                {
                    var shape = fixture.Shape as VelcroPhysics.Collision.Shapes.PolygonShape;
                    int vertexCount = shape.Vertices.Count;
                    for (int iv = 0; iv < vertexCount; ++iv)
                    {
                        Microsoft.Xna.Framework.Vector2 src = (shape.Vertices[(iv) % vertexCount]);
                        Microsoft.Xna.Framework.Vector2 des = (shape.Vertices[(iv + 1) % vertexCount]);
                        src = ChangeRotation(body, src);
                        des = ChangeRotation(body, des);
                        Vector3 v3Src = GlobalVarFun.GetDisplayPosition(src.X, src.Y);
                        Vector3 v3Des = GlobalVarFun.GetDisplayPosition(des.X, des.Y);

                        v3Src.z = 0;
                        v3Des.z = 0;

                        Gizmos.color = fixtureColor;
                        Gizmos.DrawLine(v3Src, v3Des);
                    }
                }
                else if (fixture.Shape.ShapeType == VelcroPhysics.Collision.Shapes.ShapeType.Edge)
                {
                    var shape = fixture.Shape as VelcroPhysics.Collision.Shapes.EdgeShape;
                    Microsoft.Xna.Framework.Vector2 startPos = shape._vertex1;
                    Microsoft.Xna.Framework.Vector2 endPos = shape._vertex2;

                    Microsoft.Xna.Framework.Vector2 src = startPos;
                    Microsoft.Xna.Framework.Vector2 des = endPos;
                    src = ChangeRotation(body, src);
                    des = ChangeRotation(body, des);
                    Vector3 v3Src = GlobalVarFun.GetDisplayPosition(src.X, src.Y);
                    Vector3 v3Des = GlobalVarFun.GetDisplayPosition(des.X, des.Y);

                    v3Src.z = 0;
                    v3Des.z = 0;

                    Gizmos.color = fixtureColor;
                    Gizmos.DrawLine(v3Src, v3Des);
                }
            }
        }
    }

    protected void DrawBox2DJointsGizmos(VelcroPhysics.Dynamics.World b2World)
    {

    }

    public Microsoft.Xna.Framework.Vector2 ChangeRotation(VelcroPhysics.Dynamics.Body body, Microsoft.Xna.Framework.Vector2 src)
    {
        VelcroPhysics.Shared.Transform xf;
        body.GetTransform(out xf);
        src = VelcroPhysics.Utilities.MathUtils.Mul(ref xf, src);
        return src;
    }

    #endregion DrawGizmos

    private void LoadStartMap()
    {
        if (MapManager.Instance != null && MapManager.Instance.HasCurMap == false)
        {
            bool loaded = MapManager.Instance.LoadNewMap(m_iStartMapId);
            if (loaded == false)
            {
                Debug.Log("加载地图失败。");
                m_bCanLoadMap = false;
                m_bHasLoadMap = false;
            }
            else
            {
                m_bCanLoadMap = true;
                m_bHasLoadMap = true;
            }
        }
    }

    private void LoadStartRole()
    {
        if (RoleManager.Instance != null && RoleManager.Instance.HasLoaded == false)
        {
            bool loaded = RoleManager.Instance.LoadNewMapRole(m_iStartMapId);
            if (loaded == false)
            {
                Debug.Log("加载地图人物失败。");
                m_bCanLoadRole = false;
                m_bHasLoadRole = false;
            }
            else
            {
                m_bCanLoadRole = true;
                m_bHasLoadRole = true;
            }
        }
    }
}
