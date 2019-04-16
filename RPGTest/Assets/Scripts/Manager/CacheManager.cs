using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CacheManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void Load(string fileName, Action<GameObject, object> handler, object userdata = null)
    {
        var prefabName = System.IO.Path.GetFileNameWithoutExtension(fileName);
        Load(fileName, prefabName, handler, userdata);
    }

    public void Load(string fileName, string prefabName, Action<GameObject, object> handler, object userdata = null)
    {

    }


}
