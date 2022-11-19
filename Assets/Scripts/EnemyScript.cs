using System;
using System.Collections;
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
            GameScript.tiles[nextPosition.y][nextPosition.x].hasEnemy = true;
            GameScript.tiles[position.y][position.x].hasEnemy = false;
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
        GameScript.tiles[nextPosition.y][nextPosition.x].hasEnemy = true;
        GameScript.tiles[position.y][position.x].hasEnemy = false;
    }

    public void Move()
    {
        position = nextPosition;
        transform.position = new Vector3(nextPosition.x * 2f - GameScript.width + 1, 0.5f, nextPosition.y * 2f - GameScript.height + 1);
    }

    private float EvaluateMove(Vector2Int move)
    {
        Vector2Int newPosition = position + move;
        
        if (!IsInBounds(newPosition.x, newPosition.y))
        {
            return -2f;
        }
        if (GameScript.tiles[newPosition.y][newPosition.x].GetHasFog())
        {
            return 0f;
        }
        if (GameScript.tiles[newPosition.y][newPosition.x].hasEnemy)
        {
            return -1f;
        }
        
        float fogDistance = 99999f;
        int layer = 1;
        while (layer < fogDistance)
        {
            for (int i = -layer; i < layer; i++)
            {
                if ((IsInBounds(newPosition.x + i, newPosition.y + layer) && GameScript.tiles[newPosition.y + layer][newPosition.x + i].GetHasFog())
                 || (IsInBounds(newPosition.x + i, newPosition.y - layer) && GameScript.tiles[newPosition.y - layer][newPosition.x + i].GetHasFog())
                 || (IsInBounds(newPosition.x + layer, newPosition.y + i) && GameScript.tiles[newPosition.y + i][newPosition.x + layer].GetHasFog())
                 || (IsInBounds(newPosition.x - layer, newPosition.y + i) && GameScript.tiles[newPosition.y + i][newPosition.x - layer].GetHasFog()))
                {
                    float actDist = Mathf.Sqrt(i ^ 2 + layer ^ 2);
                    if (actDist < fogDistance)
                    {
                        fogDistance = actDist;
                    }
                }
            }
            layer++;
        }

        float baseDistance = GameScript.height - newPosition.y;

        float alteredPersonality = personality + baseDistance * 0.01f;
        if (alteredPersonality > 1f)
        {
            alteredPersonality = 1f;
        }

        return baseDistance * alteredPersonality + fogDistance * (1 - alteredPersonality);

    }
    
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < GameScript.width && y >= 0 && y < GameScript.height;
    }
}
