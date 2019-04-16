using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorDataNode : MonoBehaviour
{
    [UnityEngine.Header("是否是Sensor")]
    public bool IsSensor = false;

    [UnityEngine.Header("速度参数")]
    public float SpeedRatio = 1.0f;

    [UnityEngine.Header("是否只可单向通过")]
    public bool IsOneWay = false;
    [UnityEngine.Header("单向通过方向")]
    public Vector2 OneWayDir = new Vector2();

    [UnityEngine.Header("是否固定移动方向")]
    public bool IsStuckDir = false;

    [UnityEngine.Header("外加力方向")]
    public Vector2 ForceDir = new Vector2();

}
