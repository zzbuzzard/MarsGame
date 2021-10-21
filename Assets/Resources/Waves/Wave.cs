using System;
using UnityEngine;

[@System.Serializable]
public class SpawnGroup
{
    public string prefabName;
    public int count;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Wave", order = 1)]
public class Wave : ScriptableObject
{
    public SpawnGroup[] objectSpawns;
    public float spawnGapTimes;

    // Time between this wave ENDING and the next one starting
    public float nextWaveGapTime;

    // Time between this wave and the next one starting 
    public float GetRealWaveGap()
    {
        return nextWaveGapTime + spawnGapTimes * (objectSpawns.Length-1);
    }
}