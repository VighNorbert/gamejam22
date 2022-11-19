using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveScript : MonoBehaviour
{
    private List<List<EnemySpawnPoint>> Enemies = new List<List<EnemySpawnPoint>>();
    
    private int _enemyRowIndex = 0;
    
    public GameScript gs;

    [Space(10)]
    
    public int kingCount = 0;
    public int knightCount = 0;
    public int rookCount = 0;
    public int bishopCount = 0;
    public int supermanCount = 0;

    [Space(10)]
    
    public int rowsCount;
    
    public void StartWave()
    {
        // Enemies.Add();
    }

    public bool SpawnNextEnemies()
    {
        if (_enemyRowIndex < Enemies.Count)
        {
            foreach (EnemySpawnPoint esp in Enemies[_enemyRowIndex])
            {
                GameObject prefab;
                switch (esp.type)
                {
                    case EnemyScript.EnemyType.King: prefab = gs.kingPrefab; break;
                    case EnemyScript.EnemyType.Knight: prefab = gs.knightPrefab; break;
                    case EnemyScript.EnemyType.Rook: prefab = gs.rookPrefab; break;
                    case EnemyScript.EnemyType.Bishop: prefab = gs.bishopPrefab; break;
                    case EnemyScript.EnemyType.Superman: prefab = gs.supermanPrefab; break;
                    default: prefab = gs.kingPrefab; break;
                }
                GameObject enemy = Instantiate(prefab,
                    new Vector3(esp.xCoord * 2 - GameScript.width + 1, 0, GameScript.height + 1),
                    Quaternion.identity);
                EnemyScript es = enemy.GetComponent<EnemyScript>();
                es.position = new Vector2Int(esp.xCoord, GameScript.height - 1);
                gs.enemiesAlive.Add(es);
            }
            _enemyRowIndex++;
            return true;
        }

        return false;
    }
    
}
