using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCollider : MonoBehaviour {

    public string m_sHandleName = "";
    public MapCollider m_mapCollider = null;
    public MapWorld m_curMap = null;

    public GameObject m_showObj = null;

    public InfoNode m_infoNode = null; // new InfoNode();

    protected float m_fWidth;
    protected float m_fHeight;
    protected float m_fPosX = 0.0f;
    protected float m_fPosY = 0.0f;
    protected Vector3 m_pos = new Vector3();

    protected bool m_bStarted = false;


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
