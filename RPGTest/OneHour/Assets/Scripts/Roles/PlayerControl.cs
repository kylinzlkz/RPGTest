using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : BaseRole
{

    private int m_iCurHorizontal = 0;
    private int m_iCurVertical = 0;
    private bool m_bIsPressRight = false;
    private bool m_bIsPressLeft = false;
    private bool m_bIsPressUp = false;
    private bool m_bIsPressDown = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	protected override void Update () {
        UpdateMoveControl();
        UpdateControl();
        base.Update();
    }

    #region Mono UpdateFuncs
    private void UpdateControlByAxis()
    {
        //float h = InputHelper.GetAxis("Horizontal");
        //float v = InputHelper.GetAxis("Vertical");
        //Debug.Log("H : " + h + ";   V : " + v);
        //if (m_fLastHorizontal != h)
        //{
        //    if (h > 0)
        //    {
        //        m_bIsPressRight = true;
        //        m_bIsPressLeft = false;
        //    }
        //    else if (h < 0)
        //    {
        //        m_bIsPressLeft = true;
        //        m_bIsPressRight = false;
        //    }
        //    else
        //    {
        //        m_bIsPressRight = false;
        //        m_bIsPressLeft = false;
        //    }
        //    m_fLastHorizontal = h;
        //}
        //if (Mathf.Abs(m_fLastHorizontal) <= 0.0001f)
        //{
        //    m_bIsPressRight = false;
        //    m_bIsPressLeft = false;
        //    m_fLastHorizontal = 0.0f;
        //}        //if (m_fLastVertical != v)
        //{
        //    if (v > 0)
        //    {
        //        m_bIsPressUp = true;
        //        m_bIsPressDown = false;
        //    }
        //    else if (v < 0)
        //    {
        //        m_bIsPressDown = true;
        //        m_bIsPressUp = false;
        //    }
        //    else
        //    {
        //        m_bIsPressUp = false;
        //        m_bIsPressDown = false;
        //    }
        //    m_fLastVertical = v;
        //}
        //if (Mathf.Abs(m_fLastVertical) <= 0.0001f)
        //{
        //    m_bIsPressUp = false;
        //    m_bIsPressDown = false;
        //    m_fLastVertical = 0.0f;
        //}
    }

    private void UpdateMoveControl()
    {
        {
            if (InputHelper.GetButtonDown(KeyCode.A))
            {
                m_bIsPressLeft = true;
                m_bIsPressRight = false;
                m_iCurHorizontal = 1;
            }
            if (InputHelper.GetButtonUp(KeyCode.A))
            {
                m_bIsPressLeft = false;
            }
            if (InputHelper.GetButtonDown(KeyCode.D))
            {
                m_bIsPressRight = true;
                m_bIsPressLeft = false;
                m_iCurHorizontal = -1;
            }
            if (InputHelper.GetButtonUp(KeyCode.D))
            {
                m_bIsPressRight = false;
            }
            if (m_bIsPressLeft == false && m_bIsPressRight == false)
            {
                m_iCurHorizontal = 0;
            }
            // 设置Velocity
            if (m_iCurHorizontal == 1)
            {
                RunLeft();
            }
            else if(m_iCurHorizontal == -1)
            {
                RunRight();
            }
            else
            {
                StopHorizontal();
            }
        }

        {
            if (InputHelper.GetButtonDown(KeyCode.W))
            {
                m_bIsPressUp = true;
                m_bIsPressDown = false;
                m_iCurVertical = 1;
            }
            if (InputHelper.GetButtonUp(KeyCode.W))
            {
                m_bIsPressUp = false;
            }
            if (InputHelper.GetButtonDown(KeyCode.S))
            {
                m_bIsPressDown = true;
                m_bIsPressUp = false;
                m_iCurVertical = -1;
            }
            if (InputHelper.GetButtonUp(KeyCode.S))
            {
                m_bIsPressDown = false;
            }
            if (m_bIsPressUp == false && m_bIsPressDown == false)
            {
                m_iCurVertical = 0;
            }
            // 设置Velocity
            if (m_iCurVertical == 1)
            {
                RunUp();
            }
            else if (m_iCurVertical == -1)
            {
                RunDown();
            }
            else
            {
                StopVertical();
            }
        }

        if (m_iCurHorizontal == 0 && m_iCurVertical == 0)
        {
            SetStopAction();
        }

    }

    private void UpdateControl()
    {
        // 普攻
        if (true == InputHelper.GetButtonUp("Fire2"))
        {
            SetAtk1Action();
        }
    }

    protected override void UpdateAction()
    {
        if (TryAction(ref m_ulAction))
        {
            DoAction(m_ulAction);
        }
        base.UpdateAction();
    }

    #endregion Mono UpdateFuncs

    #region Physic UpdateFuncs
    public override void UpdatePhysic(float deltaTime)
    {
        base.UpdatePhysic(deltaTime);
    }
    #endregion Physic UpdateFuncs
}
