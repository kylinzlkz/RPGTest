using System.Collections;
using System.Collections.Generic;


public class InfoNode
{
    public Dictionary<string, string> m_infos = new Dictionary<string, string>();

    public string GetValue(string key)
    {
        if (m_infos.ContainsKey(key))
        {
            return m_infos[key];
        }
        return "";
    }

    public int GetValueInt(string key)
    {
        if (m_infos.ContainsKey(key))
        {
            string value = m_infos[key];
            if (!string.IsNullOrEmpty(value))
            {
                int reta = 0;
                if (int.TryParse(value, out reta) == false)
                {
                    float fValue = 0.0f;
                    if (float.TryParse(value, out fValue))
                    {
                        reta = (int)fValue;
                    }
                }
                return reta;
            }
        }
        return 0;
    }

    public float GetValueFloat(string key)
    {
        if (m_infos.ContainsKey(key))
        {
            string value = m_infos[key];
            if (!string.IsNullOrEmpty(value))
            {
                float fValue = 0.0f;
                if (float.TryParse(value, out fValue))
                    return fValue;
            }
        }
        return 0.0f;
    }

    public bool GetValueBool(string key)
    {
        if (m_infos.ContainsKey(key))
        {
            string value = m_infos[key];
            if (!string.IsNullOrEmpty(value))
            {
                int ret = 0;
                if (int.TryParse(value, out ret))
                {
                    if (ret == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return bool.Parse(value);
                }
            }
        }
        return false;
    }

    public short GetValueShort(string key)
    {
        if (m_infos.ContainsKey(key))
        {
            string value = m_infos[key];
            if (!string.IsNullOrEmpty(value))
            {
                short reta = 0;
                if (short.TryParse(value, out reta) == false)
                {
                    float fValue = 0.0f;
                    if (float.TryParse(value, out fValue))
                    {
                        reta = (short)fValue;
                    }
                }
                return reta;
            }
        }
        return 0;
    }

    public bool SetValue(string key, string value)
    {
        string oldValue = GetValue(key);
        if (oldValue == value)
            return false;
        if (m_infos.ContainsKey(key))
            m_infos[key] = value;
        else
        {
            m_infos.Add(key, value);
        }
        return true;
    }

    public void Clear()
    {
        m_infos.Clear();
    }
}

public class InfoNodeList
{
    public Dictionary<string, InfoNode> m_nodeList = new Dictionary<string, InfoNode>();

    public int Count { get { return m_nodeList.Count; } }
}

/// <summary>
/// 读数据工具类
/// </summary>
public class InfoNodeHelper {

    static public InfoNodeList ReadFromCSV(string fileName)
    {
        //
        string content = LoadCSVToString(fileName);
        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        string[] arrLine = content.Split('\n');
        if (null == arrLine)
        {
            return null;
        }

        InfoNodeList reta = new InfoNodeList();
        string[] arrKey = arrLine[1].Remove(arrLine[1].Length - 1).Split(',');
        for (int i = 2; i < arrLine.Length; i++)
        {
            string strLine = arrLine[i];
            if (string.IsNullOrEmpty(strLine))
            {
                continue;
            }
            strLine = strLine.Remove(arrLine[i].Length - 1);
            string[] arrWord = strLine.Split(',');

            InfoNode node = new InfoNode();
            for (int j = 0; j < arrKey.Length; j++)
            {
                node.SetValue(arrKey[j], arrWord[j]);
            }
            reta.m_nodeList.Add(arrWord[0], node); // 默认第一列为key
        }
        return reta;
    }

    static public string LoadCSVToString(string fileName)
    {
        if (!Resource.ExistData(fileName))
        {
            return null;
        }
        return Resource.DecodeUTF8(Resource.LoadData(fileName));
    }

    static public byte[] LoadFileToByte(string fullname)
    {
        if (!Resource.ExistData(fullname))
        {
            return null;
        }
        return Resource.LoadData(fullname);
    }
}
