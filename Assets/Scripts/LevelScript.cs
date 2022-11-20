using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    public List<WaveScript> waves;

    private int _currentWave = 0;
    
    public void StartFirstWave()
    {
        waves[_currentWave].StartWave();
    }
    
    public bool StartNextWave()
    {
        _currentWave++;
        if (_currentWave < waves.Count)
        {
            Debug.Log("Starting wave " + (_currentWave + 1));
            waves[_currentWave].StartWave();
            return true;
        }

        return false;
    }
    
    public WaveScript GetCurrentWave()
    {
        return waves[_currentWave];
    }
    public int GetCurrentWaveIndex()
    {
        return _currentWave;
    }
    
}
