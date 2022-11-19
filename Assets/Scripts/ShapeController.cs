using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeController : MonoBehaviour
{
    public List<Vector2Int> points;
    public Sprite image;
    public int index;

    void Start()
    {
        index = transform.GetSiblingIndex();
    }

    public void SetIndex()
    {
        index = transform.GetSiblingIndex();
    }
}
