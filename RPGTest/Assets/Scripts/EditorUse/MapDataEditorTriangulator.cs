using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataEditorTriangulator {

    private List<Vector2> m_points = new List<Vector2>();

    public MapDataEditorTriangulator(Vector2[] points)
    {
        m_points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();
        int n = m_points.Count;
        if (n < 3)
        {
            return indices.ToArray();
        }

        int[] values = new int[n];
        if (Area() > 0)
        {
            for (int i = 0; i < n; i++)
            {
                values[i] = i;
            }
        }
        else
        {
            for (int i = 0; i < n; i++)
            {
                values[i] = (n - 1) - i;
            }
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
            {
                return indices.ToArray();
            }
            int u = v;
            if (nv <= u)
            {
                u = 0;
            }
            v = u + 1;
            if (nv <= v)
            {
                v = 0;
            }
            int w = v + 1;
            if (nv <= w)
            {
                w = 0;
            }

            if (Snip(u, v, w, nv, values))
            {
                int a, b, c, s, t;
                a = values[u];
                b = values[v];
                c = values[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                {
                    values[s] = values[t];
                }
                nv--;
                count = 2 * nv;
            }
        }
        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float area = 0.0f;
        for(int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = m_points[p];
            Vector2 qval = m_points[q];
            area += pval.x * qval.y - qval.x * pval.y;
        }
        return (area * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] value)
    {
        int p;
        Vector2 a = m_points[value[u]];
        Vector2 b = m_points[value[v]];
        Vector2 c = m_points[value[w]];
        if (Mathf.Epsilon > (((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x)))){
            return false;
        }
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
            {
                continue;
            }
            Vector2 point = m_points[value[p]];
            if (InsideTriangle(a, b , c, point))
            {
                return false;
            }
        }
        return true;
    }

    private bool InsideTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCrossAP, bCrossCP, aCrossBP;

        ax = c.x - b.x; ay = c.y - b.y;
        bx = a.x - c.x; by = a.y - c.y;
        cx = b.x - a.x; cy = b.y - a.y;
        apx = p.x - a.x; apy = p.y - a.y;
        bpx = p.x - b.x; bpy = p.y - b.y;
        cpx = p.x - c.x; cpy = p.y - c.y;

        aCrossBP = ax * bpy - ay * bpx;
        cCrossAP = cx * apy - cy * apx;
        bCrossCP = bx * cpy - by * cpx;

        return ((aCrossBP >= 0.0f) && (bCrossCP >= 0.0f) && (cCrossAP >= 0.0f));
    }
}
