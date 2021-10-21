using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector2 RandomPointOnCircle()
    {
        float p = Random.Range(0, 2 * Mathf.PI);
        return new Vector2(Mathf.Cos(p), Mathf.Sin(p));
    }

    // TODO: This isn't actually uniform
    public static Vector2 RandomPointInCircle()
    {
        float p = Random.Range(0, 2 * Mathf.PI);
        float r = Random.Range(0.0f, 1.0f);
        return new Vector2(r*Mathf.Cos(p), r*Mathf.Sin(p));
    }
}
