using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalVarFun {

    public static float DisplayToPhysicUnitRatio = 1f;
    public static float FloatCompareRatio = 0.0001f;

    #region ActionDefine
    public static readonly float SmoothVelocityThreshhold = 0.0001f;
    public static readonly float MoveDistSqrMaxThreshhold = 16.0f;
    public static readonly float MoveDistSqrMinThreshhold = 0.0001f;

    public const ulong ACTION_STOP = ((ulong)1 << 0); // 停止
    public const ulong ACTION_DEATH = ((ulong)1 << 1); // 死亡
    public const ulong ACTION_RUN_RIGHT = ((ulong)1 << 2); // 向右走
    public const ulong ACTION_RUN_LEFT = ((ulong)1 << 3); // 向左走
    public const ulong ACTION_RUN_UP = ((ulong)1 << 4); // 向上走
    public const ulong ACTION_RUN_DOWN = ((ulong)1 << 5); // 向下走
    public const ulong ACTION_JUMP = ((ulong)1 << 6); // 跳
    public const ulong ACTION_LAY = ((ulong)1 << 7); // 躺下
    public const ulong ACTION_LAYUP = ((ulong)1 << 8); // 躺下站起
    public const ulong ACTION_HITRECOVER = ((ulong)1 << 9); // 受击硬直
    public const ulong ACTION_ATK1 = ((ulong)1 << 10);// 普通攻击

    // 之后再考虑组合攻击等

    #endregion ActionDefine

    #region Position
    public static Vector2 GetDisplayPosition(Vector2 phyPos)
    {
        if (MapManager.Instance != null && MapManager.Instance.HasCurMap)
        {
            return phyPos * MapManager.Instance.scaleRatio;
        }
        return phyPos * DisplayToPhysicUnitRatio;
    }

    public static Vector2 GetDisplayPosition(float phyX, float phyY)
    {
        Vector2 phyPos = new Vector2(phyX, phyY);
        if (MapManager.Instance != null && MapManager.Instance.HasCurMap)
        {
            return phyPos * MapManager.Instance.scaleRatio;
        }
        return phyPos * DisplayToPhysicUnitRatio;
    }

    #endregion Position

    #region I/O

    private static bool useTestRes = true;
    private static string LOCAL_EDIT_PATH = Application.dataPath + "/../../RPGTestUI";

    //"file://" + Application.streamingAssetsPath;
    static public readonly string m_streamAssets = GetResPath("Data");

    public static string GetResPath (string folder)
    {
        string path = "";
        path = LOCAL_EDIT_PATH + "/" + folder + "/";
        if (useTestRes)
        {
            path = LOCAL_EDIT_PATH + "/" + folder + "/";
        }
        else
        {
            path = folder + "/";
        }
        return path;
    }

    static public void SaveStringToFile(string txt, string filename)
    {
        FileStream aFile = new FileStream(filename, FileMode.Create);
        StreamWriter sw = new StreamWriter(aFile, Encoding.UTF8);
        sw.Write(txt);
        sw.Close();
        sw.Dispose();
    }

    #endregion I/O

    #region Time

    public static long CurrentTimeMS
    {
        get { return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond; }
    }

    public static float GetTime()
    {
        return Time.time;
    }

    public static float GetSmoothDeltaTime()
    {
        return Time.smoothDeltaTime;
    }

    #endregion Time

    #region GameObject
    public static GameObject GetGlobalObj(string name)
    {
        return GameObject.Find(name);
    }

    public static GameObject GetChild(GameObject parent, string name)
    {
        if (parent == null)
        {
            return null;
        }
        Transform trans = parent.transform.Find(name);
        if (trans == null)
        {
            return null;
        }
        return trans.gameObject;
    }

    public static void AttachChild(GameObject parent, GameObject child)
    {
        if (parent != null && child != null)
        {
            child.transform.SetParent(parent.transform);
            //MakeAllIdentity(child);
        }
    }

    public static void RemoveAllChildImmediate(GameObject parent)
    {
        if (parent != null)
        {
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {
                GameObject tempChild = parent.transform.GetChild(i).gameObject;
                if (tempChild)
                {
                    tempChild.transform.SetParent(null);
                    GameObject.DestroyImmediate(tempChild);
                }
            }
        }
    }

    static public void MakeAllIdentity(GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
    }

    static public T GetOrAddComponent<T>(Component child, bool set_enable = false) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        var bcomp = result as Behaviour;
        if (set_enable)
        {
            if (bcomp != null) bcomp.enabled = true;
        }
        return result;
    }

    static public T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T result = go.transform.GetComponent<T>();
        if (result == null)
        {
            result = go.AddComponent<T>();
        }
        var bcomp = result as Behaviour;
        if (bcomp != null)
        {
            bcomp.enabled = true;
        }
        return result;
    }

    #endregion GameObject

    public static void ExecFuncOnNode(GameObject go, System.Action<GameObject> func)
    {
        func(go);
        int childCount = go.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            ExecFuncOnNode(go.transform.GetChild(i).gameObject, func);
        }
    }

}
