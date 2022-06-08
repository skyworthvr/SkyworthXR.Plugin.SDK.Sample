using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SafetyAreaInfo
{
    public SafetyAreaInfo()
    {
        safetyAreaName = string.Empty;
        showAreaWhenBowHead = false;
        originAlphaParam = 0f;
        originSafetyAreaColorIndex = 0;
        originHeight = 0f;
    }

    [Serializable]
    public class SafetyTransform 
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public long lastchangetime;
    }
    
    public string safetyAreaName;
    public SafetyTransform transform;
    public float originHeight;
    public float perimeter;
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uv;
    //playarea
    public List<Color> colors;
    //stationaryArea
    public float radius;

    public bool showAreaWhenBowHead;
    public float originAlphaParam;
    public int originSafetyAreaColorIndex;
    public long lastchangetime;
    public SafetyTransform observer;
}
