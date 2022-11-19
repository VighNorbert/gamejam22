using System.Collections.Generic;
using UnityEditor;
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

    public GameScript gs;

    private bool _moving = false;
    private float _timeElapsed = 0f;

    public float movementSpeed = 2f;
    private float _movementDuration = 1f;
    
    private Vector3 _worldPosition;
    private Vector3 _nextWorldPosition;

    void Start()
    {
        randomness = Random.Range(0f, 0.3f);
        personality = Random.Range(0f, 1f);
    }

    void Update()
    {
        if (_moving)
        {
            if (_timeElapsed < _movementDuration)
            {
                transform.position = Vector3.Lerp(_worldPosition, _nextWorldPosition, _timeElapsed / _movementDuration);
                _timeElapsed += Time.deltaTime;
            }
            else
            {
                _worldPosition = _nextWorldPosition;
                transform.position = _worldPosition;
                _timeElapsed = 0f;
                _moving = false;
                gs.enemiesMoved++;
            }
        }
    }

    public void ChooseNextMove()
    {
        if (GameScript.Tiles[position.y][position.x].GetHasFog())
        {
            nextPosition = position;
            return;
        }
        
        if (Random.Range(0f, 1f) <= randomness)
        {
            // random move
            
            nextPosition = position + possibleMoves[Random.Range(0, possibleMoves.Count)];

            int tries = 5;
            while (
                  !IsInBounds(nextPosition.x, nextPosition.y)
                  || (
                        (
                           GameScript.Tiles[nextPosition.y][nextPosition.x].GetHasFog()
                           || GameScript.Tiles[nextPosition.y][nextPosition.x].hasEnemy
                        ) && tries > 0
                     )
                  )
            {
                tries--;
                nextPosition = position + possibleMoves[Random.Range(0, possibleMoves.Count)];
            }
            
            // GameScript.Tiles[nextPosition.y][nextPosition.x].hasEnemy = true;
            // GameScript.Tiles[nextPosition.y][nextPosition.x].enemy = gameObject;
            // // GameScript.Tiles[position.y][position.x].hasEnemy = false;
            // GameScript.Tiles[position.y][position.x].enemy = null;
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
        // GameScript.Tiles[nextPosition.y][nextPosition.x].hasEnemy = true;
        // GameScript.Tiles[nextPosition.y][nextPosition.x].enemy = gameObject;
        // GameScript.Tiles[position.y][position.x].hasEnemy = false;
        // GameScript.Tiles[position.y][position.x].enemy = null;
    }

    public void Move()
    {
        GameScript.Tiles[position.y][position.x].hasEnemy = false;
        GameScript.Tiles[position.y][position.x].enemy = null;
        GameScript.Tiles[nextPosition.y][nextPosition.x].enemy = gameObject;
        GameScript.Tiles[nextPosition.y][nextPosition.x].hasEnemy = true;
        _worldPosition = new Vector3(position.x * 2f - GameScript.Width + 1, 0.5f, position.y * 2f - GameScript.Height + 1);
        _nextWorldPosition = new Vector3(nextPosition.x * 2f - GameScript.Width + 1, 0.5f, nextPosition.y * 2f - GameScript.Height + 1);
        _moving = true;
        _movementDuration = Vector3.Distance(_worldPosition, _nextWorldPosition) / movementSpeed;
        position = nextPosition;
        if (nextPosition.y * 2f - GameScript.Height + 1 == -19)
        {
            PlayerController.HealthDown();
            GameScript.RemoveEnemy(this.gameObject);
        }
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
        return x >= 0 && x < GameScript.Width && y >= 0 && y < GameScript.Height;
    }
    
    private void OnMouseExit()
    {
        TileScript.ResetAllColors();
    }
}
