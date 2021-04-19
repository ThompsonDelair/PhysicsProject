using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2[] PointsAroundPoint(Vector2 point, int pointCount, float radius) {
        Vector2[] arr = new Vector2[pointCount];
        float step = Mathf.PI * 2.0f / pointCount;
        for(int i = 0; i < pointCount; i++) {
            arr[i] = point + new Vector2(Mathf.Cos(step * i) * radius,Mathf.Sin(step * i) * radius);
        }
        return arr;
    }

    public static void DrawDebugCircle(Vector2 point,int pointCount,float radius,Color c) {
        Vector2[] arr = PointsAroundPoint(point,pointCount,radius);
        for(int i = 0; i < arr.Length; i++) {
            Debug.DrawLine(arr[i],arr[(i + 1)%arr.Length],c);
        }
    }
}
