using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyScript : MonoBehaviour
{
    public enum EnemyType
    {
        King,
        Knight,
        Rook,
        Bishop,
        Superman
    }

    public EnemyType type;
    public float randomness;
    public float personality;

    public Vector2Int position;

    public Vector2Int nextPosition;

    public List<Vector2Int> possibleMoves;

    void Start()
    {
        randomness = Random.Range(0f, 1f);
        personality = Random.Range(0f, 1f);
    }

    public void ChooseNextMove()
    {
        if (Random.Range(0f, 1f) <= randomness)
        {
            // random move
            
            nextPosition = position + possibleMoves[Random.Range(0, possibleMoves.Count)];

            int tries = 5;
            while (!(IsInBounds(nextPosition.x, nextPosition.y) 
                   || GameScript.Tiles[nextPosition.y][nextPosition.x].GetHasFog()
                   || GameScript.Tiles[nextPosition.y][nextPosition.x].hasEnemy)
                   && tries > 0
                   )
            {
                tries--;
                nextPosition = position + possibleMoves[Random.Range(0, possibleMoves.Count)];
            }
            GameScript.Tiles[nextPosition.y][nextPosition.x].hasEnemy = true;
            GameScript.Tiles[position.y][position.x].hasEnemy = false;
            return;
        }
        
        // best move
        
        float bestScore = 0f;
        int bestMoveIndex = -1;
        
        foreach (var move in possibleMoves)
        {
            float score = EvaluateMove(move);
            if (score > bestScore)
            {
                bestScore = score;
                bestMoveIndex = possibleMoves.IndexOf(move);
            }
        }
        
        nextPosition = position + possibleMoves[bestMoveIndex];
        GameScript.Tiles[nextPosition.y][nextPosition.x].hasEnemy = true;
        GameScript.Tiles[position.y][position.x].hasEnemy = false;
    }

    public void Move()
    {
        position = nextPosition;
        transform.position = new Vector3(nextPosition.x * 2f - GameScript.Width + 1, 0.5f, nextPosition.y * 2f - GameScript.Height + 1);
    }

    private float EvaluateMove(Vector2Int move)
    {
        Vector2Int newPosition = position + move;
        
        if (!IsInBounds(newPosition.x, newPosition.y))
        {
            return -2f;
        }
        if (GameScript.Tiles[newPosition.y][newPosition.x].GetHasFog())
        {
            return 0f;
        }
        if (GameScript.Tiles[newPosition.y][newPosition.x].hasEnemy)
        {
            return -1f;
        }
        
        float fogDistance = 99999f;
        int layer = 1;
        while (layer < fogDistance)
        {
            for (int i = -layer; i < layer; i++)
            {
                if ((IsInBounds(newPosition.x + i, newPosition.y + layer) && GameScript.Tiles[newPosition.y + layer][newPosition.x + i].GetHasFog())
                 || (IsInBounds(newPosition.x + i, newPosition.y - layer) && GameScript.Tiles[newPosition.y - layer][newPosition.x + i].GetHasFog())
                 || (IsInBounds(newPosition.x + layer, newPosition.y + i) && GameScript.Tiles[newPosition.y + i][newPosition.x + layer].GetHasFog())
                 || (IsInBounds(newPosition.x - layer, newPosition.y + i) && GameScript.Tiles[newPosition.y + i][newPosition.x - layer].GetHasFog()))
                {
                    float actDist = Mathf.Sqrt((i ^ 2) + (layer ^ 2));
                    if (actDist < fogDistance)
                    {
                        fogDistance = actDist;
                    }
                }
            }
            layer++;
        }

        float baseDistance = GameScript.Height - newPosition.y;

        float alteredPersonality = personality + baseDistance * 0.01f;
        if (alteredPersonality > 1f)
        {
            alteredPersonality = 1f;
        }

        return baseDistance * alteredPersonality + fogDistance * (1 - alteredPersonality);

    }
    
    private static bool IsInBounds(int x, int y)
    {
        return x is >= 0 and < GameScript.Width && y is >= 0 and < GameScript.Height;
    }
}
