using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Collision.Filtering;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Shared;

/// <summary>
/// 创建body的类型
/// </summary>
public enum eBodyCreateType
{
    None = 0,
    Rectangle = 1,
    Circle = 2,
    Polygon = 3,
    Edge = 4,
    ChainShape = 5,
    LoopShape = 6,
    RoundedRectangle = 7,
    BreakableBody = 8,
    LineArc = 9,
    SolidArc = 10,
}

/// <summary>
/// 碰撞过滤Enum
/// </summary>
public enum eColliderFilterType
{
    All = 0,
    Player = 1,
    Monster = 2,
}

public enum eMapAreaType
{
    None = 0,
    SoftArea = 1, //舒适环境
    ToughArea = 2, //艰苦环境
    WorkArea = 3, //工作环境
    FieldArea = 4,//野外环境
}

/// <summary>
/// 地图读表数值数据
/// </summary>
public class MapDataNode
{
    public bool HasLoaded { get; set; } = false;
    public int ID { get; set; }
    public string Name { get; set; }
    public eMapAreaType Type { get; set; }
    public float ScaleRatio { get; set; }
    public string SmallMapName { get; set; }
    public string MapMusicName { get; set; }
    public float MapAreaMinX { get; set; }
    public float MapAreaMaxX { get; set; }
    public float MapAreaMinY { get; set; }
    public float MapAreaMaxY { get; set; }
    public float MoodDecrease { get; set; } //情绪减少基础值
    public float InitPosX { get; set; } // 进入地图初始点
    public float InitPosY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public List<ColliderDataNode> m_colliderDatas = null;

    public MapDataNode()
    {
    }

    public MapDataNode(InfoNode node)
    {
        ID = node.GetValueInt("ID");
        Name = node.GetValue("NAME");
        Type = (eMapAreaType)node.GetValueInt("TYPE");
        ScaleRatio = node.GetValueFloat("SCALERATIO");
        SmallMapName = node.GetValue("SMALLMAP");
        MapMusicName = node.GetValue("BGM");
        MapAreaMinX = node.GetValueFloat("MINX");
        MapAreaMaxX = node.GetValueFloat("MAXX");
        MapAreaMinY = node.GetValueFloat("MINY");
        MapAreaMaxY = node.GetValueFloat("MAXY");
        MoodDecrease = node.GetValueFloat("MOODDEC");
        InitPosX = node.GetValueFloat("INITPOSX");
        InitPosY = node.GetValueFloat("INITPOSY");
        Width = (int)(MapAreaMaxX - MapAreaMinX) + 1;
        Height = (int)(MapAreaMaxY - MapAreaMinY) + 1;
    }

    public void CreateCollider(InfoNode node)
    {
        ColliderDataNode colliderdata = new ColliderDataNode(node);
        if (m_colliderDatas == null)
        {
            m_colliderDatas = new List<ColliderDataNode>();
        }
        m_colliderDatas.Add(colliderdata);
    }

}

/// <summary>
/// 碰撞体数值数据
/// </summary>
public class ColliderDataNode
{
    public string name; // 类型名称
    public int bodyType; // Body类型 static/dynamic/kinematic
    public int bodyCreateType = (int)eBodyCreateType.None; // body创建类型
    public Vector2 position = Vector2.zero;
    public float sizeX;
    public float sizeY;
    public float radius;
    public List<Vector2> vertices;
    public Vector2 startPos;
    public Vector2 endPos;
    public bool isBreakable = false;  // 是否可破坏
    public string breakMainName = "";   // 可破坏物件的主物件名
    public int filterType = (int)eColliderFilterType.All;// 过滤类型
    public bool isActive = true;

    public bool isSensor = false;
    public float speedRatio = 1.0f;
    public bool isOneWay = false;
    public Vector2 oneWayDir = new Vector2(); // 单向阻挡的方向
    public bool isStuckDir = false; // 是否锁定移动方向
    public Vector2 forceDir = new Vector2(); // 外加力方向

    public ColliderDataNode()
    {

    }

    public ColliderDataNode(InfoNode node)
    {
        name = node.GetValue("NAME");
        bodyType = node.GetValueInt("BODYTTYPE");
        bodyCreateType = node.GetValueInt("CREATETYPE");
        string posStr = node.GetValue("POSITION");
        if (!string.IsNullOrEmpty(posStr))
        {
            string[] posArr = posStr.Split(';');
            if (posArr.Length > 1)
            {
                position = new Vector2(float.Parse(posArr[0]), float.Parse(posArr[1]));
            }
        }
        sizeX = node.GetValueFloat("SIZEX");
        sizeY = node.GetValueFloat("SIZEY");
        radius = node.GetValueFloat("RADIUS");
        if (bodyCreateType == (int)eBodyCreateType.Polygon)
        {
            string verticesStr = node.GetValue("VERTICES");
            if (!string.IsNullOrEmpty(verticesStr))
            {
                string[] verticesArr = verticesStr.Split('=');
                if (verticesArr.Length > 0)
                {
                    vertices = new List<Vector2>();
                    for (int i = 0; i < verticesArr.Length; i++)
                    {
                        string verticeStr = verticesArr[i];
                        string[] verticeArr = verticeStr.Split(';');
                        if (verticeArr.Length > 1)
                        {
                            Vector2 vertice = new Vector2(float.Parse(verticeArr[0]), float.Parse(verticeArr[1]));
                            vertices.Add(vertice);
                        }
                    }
                }
            }
        }
        else if(bodyCreateType == (int)eBodyCreateType.Edge)
        {
            string startPosStr = node.GetValue("STARTPOS");
            if (!string.IsNullOrEmpty(startPosStr))
            {
                string[] startPosArr = startPosStr.Split(';');
                if (startPosArr.Length > 1)
                {
                    startPos = new Vector2(float.Parse(startPosArr[0]), float.Parse(startPosArr[1]));
                }
            }
            string endPosStr = node.GetValue("ENDPOS");
            if (!string.IsNullOrEmpty(endPosStr))
            {
                string[] endPosArr = endPosStr.Split(';');
                if (endPosArr.Length > 1)
                {
                    endPos = new Vector2(float.Parse(endPosArr[0]), float.Parse(endPosArr[1]));
                }
            }
        }

        isBreakable = node.GetValueBool("ISBREAKABLE");
        breakMainName = node.GetValue("BREAKMAINNAME");
        filterType = node.GetValueInt("FILTERTYPE");
        isActive = node.GetValueBool("ISACTIVE");

        isSensor = node.GetValueBool("ISSENSOR");
        speedRatio = node.GetValueFloat("SPEEDRATIO");
        isOneWay = node.GetValueBool("ISONEWAY");
        string oneWayDirStr = node.GetValue("ONEWAYDIR");
        if (!string.IsNullOrEmpty(oneWayDirStr))
        {
            string[] arr = oneWayDirStr.Split(';');
            if (arr.Length > 1)
            {
                oneWayDir = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
            }
        }
        isStuckDir = node.GetValueBool("ISSTUCKDIR");
        string forceDirStr = node.GetValue("FORCEDOR");
        if (!string.IsNullOrEmpty(forceDirStr))
        {
            string[] arr = forceDirStr.Split(';');
            if (arr.Length > 1)
            {
                forceDir = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
            }
        }
    }
}


/// <summary>
/// 地图static函数
/// </summary>
public class MapFunctions
{
    public static Vertices MakeBoxVertices(float hWidth, float hHeight)
    {
        Vector2 vertex = new Vector2();
        Vertices vertices = new Vertices();
        vertex.X = -hWidth; vertex.Y = -hHeight + hHeight * 0.05f;
        vertices.Add(vertex);
        vertex.X = -hWidth + hWidth * 0.05f; vertex.Y = -hHeight;
        vertices.Add(vertex);
        vertex.X = hWidth - hWidth * 0.05f; vertex.Y = -hHeight;
        vertices.Add(vertex);
        vertex.X = hWidth; vertex.Y = -hHeight + hHeight * 0.05f;
        vertices.Add(vertex);
        vertex.X = hWidth; vertex.Y = hHeight;
        vertices.Add(vertex);
        vertex.X = -hWidth; vertex.Y = hHeight;
        vertices.Add(vertex);
        return vertices;
    }

    public static void SetCollisionGroup(Body body, short group, Category category, Category collidesWith)
    {
        if (null != body)
        {
            body.CollisionGroup = group;
            body.CollisionCategories = category;
            body.CollidesWith = collidesWith;
        }
    }
}