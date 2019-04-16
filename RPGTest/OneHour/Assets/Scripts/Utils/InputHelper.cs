using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAxis
{
    private float m_value;
    public string name { get; private set; }
    public bool matchWithInputHelper { get; private set; }

    public VirtualAxis(string name) : this(name, true)
    {

    }

    public VirtualAxis(string name, bool matchToInputHelper)
    {
        this.name = name;
        matchWithInputHelper = matchToInputHelper;
    }

    public void Update(float value)
    {
        m_value = value;
    }

    public float GetValue { get { return m_value; } }

    public float GetValueRaw { get { return m_value; } }
}

public class VirtualButton
{
    public string name { get; private set; }
    public bool matchWithInputHelper { get; private set; }

    private int m_lastPressedFrame = -5;
    private int m_releasedFrame = -1;
    private bool m_bPressed = false;

    public VirtualButton(string name):this(name, true)
    {

    }

    public VirtualButton(string name, bool matchToInputHelper)
    {
        this.name = name;
        matchWithInputHelper = matchToInputHelper;
    }

    public void Pressed()
    {
        if (m_bPressed)
            return;
        m_bPressed = true;
        m_lastPressedFrame = Time.frameCount;
    }

    public void Release()
    {
        m_bPressed = false;
        m_releasedFrame = Time.frameCount;
    }

    public bool GetButton { get { return m_bPressed; } }

    public bool GetButtonDown { get { return m_lastPressedFrame - Time.frameCount == -1; } }

    public bool GetButtonUp { get { return m_releasedFrame == Time.frameCount - 1; } }
}

public class StandardInput {

    private Dictionary<string, VirtualAxis> axisDic = new Dictionary<string, VirtualAxis>();
    private Dictionary<string, VirtualButton> buttonDic = new Dictionary<string, VirtualButton>();

    public bool AxisExist(string name)
    {
        return axisDic.ContainsKey(name);
    }

    public bool ButtonExist(string name)
    {
        return buttonDic.ContainsKey(name);
    }

    public void RegisterAxis(VirtualAxis axis)
    {
        if (axisDic.ContainsKey(axis.name))
        {
            Debug.LogError("The Axis Is Already Existed.");
        }
        else
        {
            axisDic.Add(axis.name, axis);
        }
    }

    public void RegisterButton(VirtualButton button)
    {
        if (buttonDic.ContainsKey(button.name))
        {
            Debug.LogError("The Axis Is Already Existed.");
        }
        else
        {
            buttonDic.Add(button.name, button);
        }
    }

    public void UnRegisterAixs(string name)
    {
        if (axisDic.ContainsKey(name))
        {
            axisDic.Remove(name);
        }
    }

    public void UnRegisterButton(string name)
    {
        if (buttonDic.ContainsKey(name))
        {
            buttonDic.Remove(name);
        }
    }

    public VirtualAxis AxisReference(string name)
    {
        return axisDic.ContainsKey(name) ? axisDic[name] : null;
    }

    public VirtualButton ButtonReference(string name)
    {
        return buttonDic.ContainsKey(name) ? buttonDic[name] : null;
    }

    public float GetAxis(string name, bool raw)
    {
        return raw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
    }

    public bool GetButton(string name)
    {
        return Input.GetButton(name);
    }

    public bool GetButtonDown(string name)
    {
        return Input.GetButtonDown(name);
    }

    public bool GetButtonUp(string name)
    {
        return Input.GetButtonUp(name);
    }

    public bool GetKey(KeyCode code)
    {
        return Input.GetKey(code);
    }

    public bool GetKeyDown(KeyCode code)
    {
        return Input.GetKeyDown(code);
    }

    public bool GetKeyUp(KeyCode code)
    {
        return Input.GetKeyUp(code);
    }

    public Vector3 MousePosition()
    {
        return Input.mousePosition;
    }
}

public class InputHelper
{
    private static StandardInput activeInput;

    public InputHelper()
    {
        activeInput = new StandardInput();
    }

    public static bool AxisExist(string name)
    {
        return activeInput.AxisExist(name);
    }

    public static bool ButtonExist(string name)
    {
        return activeInput.ButtonExist(name);
    }

    public static void RegisterAxis(VirtualAxis axis)
    {
        activeInput.RegisterAxis(axis);
    }

    public static void RegisterButton(VirtualButton button)
    {
        activeInput.RegisterButton(button);
    }

    public static void UnRegisterAixs(string name)
    {
        activeInput.UnRegisterAixs(name);
    }

    public static void UnRegisterButton(string name)
    {
        activeInput.UnRegisterButton(name);
    }

    public static VirtualAxis AxisReference(string name)
    {
        return activeInput.AxisReference(name);
    }

    public static VirtualButton ButtonReference(string name)
    {
        return activeInput.ButtonReference(name);
    }

    public static float GetAxis(string name)
    {
        return GetAxis(name, false);
    }

    public static float GetAxis(string name, bool raw)
    {
        return activeInput.GetAxis(name, raw);
    }

    public static bool GetButton(string name)
    {
        return activeInput.GetButton(name);
    }

    public static bool GetButtonDown(string name)
    {
        return activeInput.GetButtonDown(name);
    }

    public static bool GetButtonUp(string name)
    {
        return activeInput.GetButtonUp(name);
    }

    public static bool GetButton(KeyCode code)
    {
        return activeInput.GetKey(code);
    }

    public static bool GetButtonDown(KeyCode code)
    {
        return activeInput.GetKeyDown(code);
    }

    public static bool GetButtonUp(KeyCode code)
    {
        return activeInput.GetKeyUp(code);
    }

    public static Vector3 MousePosition()
    {
        return activeInput.MousePosition();
    }
}