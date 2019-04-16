using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeEventNode
{
    public bool IsRemove = false;
    public string Name = "";
    public virtual bool Update(int intervalTimeMs)
    {
        return false;
    }
}

/// <summary>
/// 到时回调
/// </summary>
public class TimeExpireEventNode : TimeEventNode
{
    public string Name = "";
    private int m_iTotalTimeMs = 0;
    private int m_iPassTimeMs = 0;
    private Action mcb = null;

    public int TotalTimeMs
    {
        get { return m_iTotalTimeMs; }
    }
    public int PassTimeMs
    {
        get { return m_iPassTimeMs; }
    }
    public int RemainTimeMs
    {
        get { return m_iTotalTimeMs - m_iPassTimeMs; }
    }

    public TimeExpireEventNode(string name, float totalTime, Action cb)
    {
        Name = name;
        m_iTotalTimeMs = (int)(totalTime * 1000);
        mcb = cb;
    }

    public override bool Update(int intervalTimeMs)
    {
        if (IsRemove)
        {
            return true;
        }
        if (m_iPassTimeMs >= m_iTotalTimeMs)
        {
            if (mcb != null)
            {
                mcb();
            }
            return true;
        }
        m_iPassTimeMs += intervalTimeMs;
        return false;
    }
}

/// <summary>
/// 间隔时间回调
/// </summary>
public class TimeIntervalEventNode : TimeEventNode
{
    public string Name = "";
    private int m_iIntervalTimeMs = 0;
    private int m_iPassTimeMs = 0;
    private int m_iCount = 0;
    private int m_iPassCount = 0;
    private bool m_bFirstEnable = false;
    private Action mcb = null;

    public int IntervalTimeMs
    {
        get { return m_iIntervalTimeMs; }
    }

    public int PassCount
    {
        get { return m_iPassCount; }
    }

    public TimeIntervalEventNode(string name, float intervalTime, int count, bool firstEnable, Action cb)
    {
        Name = name;
        m_iIntervalTimeMs = (int)(intervalTime * 1000);
        m_iCount = count;
        m_bFirstEnable = firstEnable;
        mcb = cb;
    }

    public override bool Update(int intervalTimeMs)
    {
        if (IsRemove)
        {
            return true;
        }
        if (m_iIntervalTimeMs > 0)
        {
            if(m_iIntervalTimeMs <= m_iPassTimeMs)
            {
                if (m_iPassCount >= m_iCount)
                {
                    return true; // 次数够了
                }
                m_iPassTimeMs += intervalTimeMs;
                m_iPassTimeMs -= m_iIntervalTimeMs;
                m_iPassCount++;
                if(mcb != null)
                {
                    mcb();
                }
            }
            else if (m_iPassTimeMs == 0 && m_bFirstEnable == true){
                m_iPassTimeMs += intervalTimeMs;
                if (mcb != null)
                {
                    mcb();
                }
            }
            else
            {
                m_iPassTimeMs += intervalTimeMs;
            }
        }
        return false;
    }
}


public class TimeEventManager {

    private static TimeEventManager _instance;
    private List<TimeEventNode> m_list = new List<TimeEventNode>();
    private long m_iLastUpdateTimeMs = 0;

    public static TimeEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                new TimeEventManager();
            }
            return _instance;
        }
    }

    private TimeEventManager()
    {
        _instance = this;
    }

    public void AddTimeExpireEvent(string name, float time, Action cb)
    {
        TimeExpireEventNode node = new TimeExpireEventNode(name, time, cb);
        m_list.Add(node);
    }

    public void RemoveTimeExpireEvent(string name)
    {
        for(int i = 0; i < m_list.Count; i++)
        {
            if (m_list[i].Name == name)
            {
                m_list[i].IsRemove = true;
                break;
            }
        }
    }


    public void AddTimeIntervalEvent(string name, float intervalTime, int count, bool firstEnable, Action cb)
    {
        TimeIntervalEventNode node = new TimeIntervalEventNode(name, intervalTime, count, firstEnable, cb);
        m_list.Add(node);
    }

    public void RemoveTimeIntervalEvent(string name)
    {
        for (int i = 0; i < m_list.Count; i++)
        {
            if (m_list[i].Name == name)
            {
                m_list[i].IsRemove = true;
                break;
            }
        }
    }

    void Update () {
        long curTime = GlobalVarFun.CurrentTimeMS;
        int interval = (int)(curTime - m_iLastUpdateTimeMs);
        m_iLastUpdateTimeMs = curTime;
        for (int i = m_list.Count - 1; i >= 0; i--)
        {
            if (m_list[i].Update(interval))
            {
                m_list.RemoveAt(i);
            }
        }
    }
}
