using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the memory so that only 1 wave is present in memory at a time.
public class WaveManager
{
    int currentWave;
    Wave allocatedResource;

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public WaveManager()
    {
        currentWave = 0;
        allocatedResource = null;
    }

    public Wave GetNextWave()
    {
        currentWave++;
        if (allocatedResource != null)
        {
            Resources.UnloadAsset(allocatedResource);
        }
        allocatedResource = Resources.Load<Wave>("Waves/wave" + currentWave.ToString());
        return allocatedResource;
    }
}
