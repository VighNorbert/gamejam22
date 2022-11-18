using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyScript : MonoBehaviour
{
    public enum EnemyType { King, Knight, Rook, Bishop, Superman }

    public EnemyType type;
    public float randomness;
    public float personality;
    
    public List<Vector2Int> possibleMoves;

    void Start()
    {
        randomness = Random.Range(0f, 1f);
        personality = Random.Range(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
