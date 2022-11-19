using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    public List<WaveScript> waves;

    private int _currentWave = 0;
    
    private void StartFirstWave()
    {
        waves[_currentWave].StartWave();
    }
    
    public bool WaveFinished()
    {
        _currentWave++;
        if (_currentWave < waves.Count)
        {
            waves[_currentWave].StartWave();
            return false;
        }

        return true;
    }
    
    public WaveScript GetCurrentWave()
    {
        return waves[_currentWave];
    }
    
}
