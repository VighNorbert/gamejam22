using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeController : MonoBehaviour
{
    public List<Vector2Int> points;
    public Sprite image;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        index = transform.GetSiblingIndex();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIndex()
    {
        index = transform.GetSiblingIndex();
    }
}
