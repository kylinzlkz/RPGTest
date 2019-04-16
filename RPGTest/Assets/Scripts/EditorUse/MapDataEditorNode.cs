using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataEditorNode : MonoBehaviour {

    public enum eMapEditorNodeType
    {
        None = 0,
        Box = 1,
        Circle = 2,
        Polygon = 3,
        Edge = 4,
        Ellipse = 5,
    }

    public GameObject m_rootObj;
    public string m_sMapName = "";

    public class SingleNode
    {
        public eMapEditorNodeType type;
        public bool isSensor;
        public Vector3 position;
        public float sizeX;
        public float sizeY;
        public float radius;
        public List<Vector2> vertices;
        public Vector2 startPos;
        public Vector2 endPos;
        public SensorDataNode sensorNode;
    }

    public List<SingleNode> m_nodes = new List<SingleNode>();

    #region ADD NODE
    public void AddBox(GameObject go)
    {
        SingleNode node = new SingleNode();
        node.type = eMapEditorNodeType.Box;
        node.position = go.transform.position;
        node.sizeX = go.transform.lossyScale.x;
        node.sizeY = go.transform.lossyScale.y;

        SensorDataNode sensorNode = go.GetComponent<SensorDataNode>();
        if (sensorNode != null)
        {
            //if (sensorNode.FaceNormal.SqrMagnitude() > 0.5f)
            //{
            //    sensorNode.IsSensor = false;
            //}
            //else
            //{
            //    sensorNode.IsSensor = true;
            //}
            node.isSensor = sensorNode.IsSensor;
        }
        else
        {
            node.isSensor = false;
        }
        node.sensorNode = sensorNode;
        m_nodes.Add(node);
    }

    public void AddCircle(GameObject go)
    {
        SingleNode node = new SingleNode();
        node.type = eMapEditorNodeType.Circle;
        node.position = go.transform.position;
        node.sizeX = go.transform.lossyScale.x;
        node.sizeY = go.transform.lossyScale.y;
        if (node.sizeX != node.sizeY)
        {
            node.type = eMapEditorNodeType.Ellipse;
        }
        node.radius = go.transform.lossyScale.x * 0.5f;

        SensorDataNode sensorNode = go.GetComponent<SensorDataNode>();
        if (sensorNode != null)
        {
            //if (sensorNode.FaceNormal.SqrMagnitude() > 0.5f)
            //{
            //    sensorNode.IsSensor = false;
            //}
            //else
            //{
            //    sensorNode.IsSensor = true;
            //}
            node.isSensor = sensorNode.IsSensor;
        }
        else
        {
            node.isSensor = false;
        }
        node.sensorNode = sensorNode;
        m_nodes.Add(node);
    }

    public void AddPolygon(GameObject go)
    {
        SingleNode node = new SingleNode();
        node.type = eMapEditorNodeType.Polygon;
        node.position = go.transform.position;
        node.vertices = new List<Vector2>();
        node.isSensor = false;
        int childCount = go.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            Vector2 vector = new Vector2(child.transform.localPosition.x, child.transform.localPosition.y);
            node.vertices.Add(vector);
        }
        SensorDataNode sensorNode = go.GetComponent<SensorDataNode>();
        if (sensorNode != null)
        {
            //if (sensorNode.FaceNormal.SqrMagnitude() > 0.5f)
            //{
            //    sensorNode.IsSensor = false;
            //}
            //else
            //{
            //    sensorNode.IsSensor = true;
            //}
            node.isSensor = sensorNode.IsSensor;
        }
        else
        {
            node.isSensor = false;
        }
        node.sensorNode = sensorNode;
        m_nodes.Add(node);
    }

    public void AddEdge(GameObject go)
    {
        SingleNode node = new SingleNode();
        node.type = eMapEditorNodeType.Edge;
        node.position = go.transform.position;
        Transform start = go.transform.Find("Start");
        Transform end = go.transform.Find("End");
        if (start != null && end != null)
        {
            node.startPos = start.position;
            node.endPos = end.position;
        }
        SensorDataNode sensorNode = go.GetComponent<SensorDataNode>();
        if (sensorNode != null)
        {
            //if (sensorNode.FaceNormal.SqrMagnitude() > 0.5f)
            //{
            //    sensorNode.IsSensor = false;
            //}
            //else
            //{
            //    sensorNode.IsSensor = true;
            //}
            node.isSensor = sensorNode.IsSensor;
        }
        else
        {
            node.isSensor = false;
        }
        node.sensorNode = sensorNode;
        m_nodes.Add(node);
    }
    #endregion ADD NODE

    public bool SaveData(string fileName)
    {
        if (m_nodes == null || m_nodes.Count <= 0)
        {
            return false;
        }

        string reta = "ID,名字,Body类型,创建类型,位置,X大小(BOX),Y大小(BOX),半径,vertices点,Edge起点,Edge终点,IsBreakable,Break物体主物体名,过滤类型,默认是否激活,是否传感器,Senesor速度参数,是否单向通行,单项通行方向,Sensor是否固定,Sensor外加力方向";
        reta += "\r\n";
        reta += "ID,NAME,BODYTYPE,CREATETYPE,COLLISIONID,POSITION,SIZEX,SIZEY,RADIUS,VERTICES,STARTPOS,ENDPOS,ISBREAKABLE,BREAKMAINNAME,FILTERTYPE,ISACTIVE,ISSENSOR,SPEEDRATIO,ISONEWAY,ONEWAYDIR,ISSTUCKDIR,FORCEDOR";
        reta += "\r\n";

        for (int i = 0; i < m_nodes.Count; i++)
        {
            SingleNode nd = m_nodes[i];
            if (nd != null)
            {
                reta += i.ToString();
                reta += ",";
                reta += "Collider_" + i.ToString();
                reta += ",";
                reta += "0";
                reta += ",";
                reta += ((int)nd.type).ToString();
                reta += ",";
                reta += "0";
                reta += ",";
                reta += nd.position.x + ";" + nd.position.y;
                reta += ",";
                reta += nd.sizeX.ToString();
                reta += ",";
                reta += nd.sizeY.ToString();
                reta += ",";
                reta += nd.radius.ToString();
                reta += ",";
                if (nd.vertices != null)
                {
                    for (int j = 0; j < nd.vertices.Count; j++)
                    {
                        Vector2 vertice = nd.vertices[j];
                        reta += vertice.x + ";" + vertice.y;
                        if (j != nd.vertices.Count - 1)
                        {
                            reta += "=";
                        }
                    }
                }
                reta += ",";
                reta += nd.startPos.x + ";" + nd.startPos.y;
                reta += ",";
                reta += nd.endPos.x + ";" + nd.endPos.y;
                reta += ",";
                reta += "FALSE";
                reta += ",";
                reta += "0";
                reta += ",";
                reta += "0";
                reta += ",";
                reta += "TRUE";
                reta += ",";
                reta += nd.isSensor;
                reta += ",";
                if (nd.sensorNode != null)
                {
                    reta += nd.sensorNode.SpeedRatio;
                    reta += ",";
                    reta += nd.sensorNode.IsOneWay;
                    reta += ",";
                    reta += nd.sensorNode.OneWayDir.x + ";" + nd.sensorNode.OneWayDir.y;
                    reta += ",";
                    reta += nd.sensorNode.IsStuckDir;
                    reta += ",";
                    reta += nd.sensorNode.ForceDir.x + ";" + nd.sensorNode.ForceDir.y; 
                }
                else{
                    reta += ",";
                    reta += ",";
                    reta += ",";
                    reta += ",";
                }
                reta += "\r\n";
            }
        }
        string filePath = GlobalVarFun.m_streamAssets + "Map/" + fileName + ".csv";
        GlobalVarFun.SaveStringToFile(reta, filePath);

        return true;
    }

}
