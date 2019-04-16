using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public interface IResourceLoader
{
    // 判断文件是否存在
    bool ExistData(string path);

    // 加载文件
    byte[] LoadData(string path);

    T LoadObject<T>(string path) where T : UnityEngine.Object;
}


public class ResourceLoader : IResourceLoader
{
    private static ResourceLoader instance;

    public static ResourceLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourceLoader();
            }
            return instance;
        }
    }

    private ResourceLoader()
    {
        Resource.SetLoader(this);
    }

    public bool ExistData(string path)
    {
        bool rlt = false;
        if (path.StartsWith(Resource.PREFIX_FILE))
        {
            rlt = TryExistDataFromFileSystem(path.Substring(Resource.PREFIX_FILE.Length));
            return rlt;
        }

        rlt = TryExistDataFromFileSystem(path);
        return rlt;
    }

    public byte[] LoadData(string path)
    {
        byte[] ret = null;
        if (path.StartsWith(Resource.PREFIX_FILE))
        {
            TryLoadFromFileSystem(path.Substring(Resource.PREFIX_FILE.Length), ref ret);
            return ret;
        }
        if (path.StartsWith(Resource.PREFIX_RES))
        {
            TryLoadFromResources(path.Substring(Resource.PREFIX_RES.Length), ref ret);
            return ret;
        }

        if (File.Exists(path) && TryLoadFromFileSystem(path, ref ret))
        {
            return ret;
        }
        if (TryLoadFromResources(path, ref ret))
        {
            return ret;
        }
        return ret;
    }

    public T LoadObject<T>(string path) where T : UnityEngine.Object
    {
        return LoadFromResources<T>(path);
    }

    private bool TryExistDataFromFileSystem(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }

    private bool TryLoadFromFileSystem(string path, ref byte[] ret)
    {
        string fullpath = path;
        ret = File.ReadAllBytes(fullpath);
        if (ret != null)
        {
            return true;
        }
        return false;
    }

    private static T LoadFromResources<T>(string path) where T : UnityEngine.Object
    {
        //int index = path.LastIndexOf(".");

        //if (index < 0) { return null; }
        // Unity TextAsset
        string assetpath = path; //.Substring(0, index);
        while (assetpath.StartsWith("/"))
        {
            assetpath = assetpath.Substring(1);
        }
        T ta = UnityEngine.Resources.Load<T>(assetpath);
        if (ta == null)
        {
            assetpath = Path.Combine("Prefabs", assetpath);
            ta = UnityEngine.Resources.Load<T>(assetpath);
        }
        return ta;
    }

    private bool TryLoadFromResources(string path, ref byte[] ret)
    {
        UnityEngine.TextAsset data = LoadFromResources<UnityEngine.TextAsset>(path);
        if (data != null)
        {
            ret = data.bytes;
            return true;
        }
        return false;
    }

}

public class Resource
{
    public const string PREFIX_FILE = "file://";
    public const string PREFIX_RES = "res://";

    private static IResourceLoader m_curLoader = null;

    public static void SetLoader(IResourceLoader loader)
    {
        m_curLoader = loader;
    }

    public static readonly Encoding UTF8 = new UTF8Encoding(false, false);
    public static string DecodeUTF8(byte[] data)
    {
        if (data.Length > 3)
        {
            if ((data[0] == 0xEF) && (data[1] == 0xBB) && (data[2] == 0xBF))
            {
                return UTF8.GetString(data, 3, data.Length - 3);
                //return UTF8_BOM.GetString(data);
            }
        }
        return UTF8.GetString(data);
    }

    public static bool ExistData(string path)
    {
        return m_curLoader.ExistData(path);
    }

    public static byte[] LoadData(string path)
    {
        return m_curLoader.LoadData(path);
    }

    public static UnityEngine.Object LoadObj(string path)
    {
        return m_curLoader.LoadObject<UnityEngine.Object>(path);
    }
}