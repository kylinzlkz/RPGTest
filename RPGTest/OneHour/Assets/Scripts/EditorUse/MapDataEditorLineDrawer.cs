using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataEditorLineDrawer : MonoBehaviour {

    public Transform start;
    public Transform end;

    void OnDrawGizmosSelected()
    {
        start = this.transform.Find("Start");
        end = this.transform.Find("End");
        if (start == null || end == null)
        {
            return;
        }

        Gizmos.DrawLine(start.position, end.position);
    }
}
