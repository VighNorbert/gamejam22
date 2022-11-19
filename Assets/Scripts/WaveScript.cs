using System.Collections.Generic;
using UnityEngine;

public class WaveScript : MonoBehaviour
{
    private readonly List<List<EnemySpawnPoint>> _enemies = new();
    
    private int _enemyRowIndex;
    
    public GameScript gs;

    [Space(10)]
    
    public int kingCount;
    public int knightCount;
    public int rookCount;
    public int bishopCount;
    public int supermanCount;

    [Space(10)]
    
    public int rowsCount;
    
    public void StartWave()
    {
        for (int i = 0; i < rowsCount; i++)
        {
            _enemies.Add(new List<EnemySpawnPoint>());
        }

        InitiateEnemyClass(kingCount, EnemyScript.EnemyType.King);
        InitiateEnemyClass(knightCount, EnemyScript.EnemyType.Knight);
        InitiateEnemyClass(rookCount, EnemyScript.EnemyType.Rook);
        InitiateEnemyClass(bishopCount, EnemyScript.EnemyType.Bishop);
        InitiateEnemyClass(supermanCount, EnemyScript.EnemyType.Superman);
    }
    
    private void InitiateEnemyClass(int count, EnemyScript.EnemyType type)
    {
        while (count > 0)
        {
            int row = Random.Range(0, rowsCount);
            int col = Random.Range(0, GameScript.Width);
            bool found = false;
            foreach (var esp in _enemies[row])
            {
                if (esp.XCoord == col)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                EnemySpawnPoint esp = new EnemySpawnPoint
                {
                    Type = type,
                    XCoord = col
                };
                _enemies[row].Add(esp);
                count--;
            }
        }
    }
    

    public bool SpawnNextEnemies()
    {
        if (_enemyRowIndex < _enemies.Count)
        {
            foreach (EnemySpawnPoint esp in _enemies[_enemyRowIndex])
            {
                GameObject prefab;
                switch (esp.Type)
                {
                    case EnemyScript.EnemyType.King: prefab = gs.kingPrefab; break;
                    case EnemyScript.EnemyType.Knight: prefab = gs.knightPrefab; break;
                    case EnemyScript.EnemyType.Rook: prefab = gs.rookPrefab; break;
                    case EnemyScript.EnemyType.Bishop: prefab = gs.bishopPrefab; break;
                    case EnemyScript.EnemyType.Superman: prefab = gs.supermanPrefab; break;
                    default: prefab = gs.kingPrefab; break;
                }
                GameObject enemy = Instantiate(prefab,
                    new Vector3(esp.XCoord * 2 - GameScript.Width + 1, 0f, GameScript.Height - 1),
                    Quaternion.AngleAxis(180f, Vector3.up));
                EnemyScript es = enemy.GetComponent<EnemyScript>();
                es.gs = gs;
                es.position = new Vector2Int(esp.XCoord, GameScript.Height - 1);
                GameScript.enemiesAlive.Add(es);
            }
            _enemyRowIndex++;
            return true;
        }

        return false;
    }
    
}
