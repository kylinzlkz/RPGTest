using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataEditorPolygonDrawer : MonoBehaviour {

    public Transform[] vertices;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    }

    void OnDrawGizmosSelected()
    {
        int childCount = this.transform.childCount;
        vertices = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            vertices[i] = this.transform.GetChild(i);
        }
        for (int iv = 0; iv < childCount; ++iv)
        {
            Vector2 src = vertices[(iv) % childCount].position;
            Vector2 des = vertices[(iv + 1) % childCount].position;

            Gizmos.DrawLine(src, des);
        }
    }
}
