using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public GameObject currShape;
    public GameObject game;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (currShape)
        {
        Debug.Log(currShape.name);

        }*/
        if (GameScript.phase == 2)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);
            if (hit.transform != null)
            {
                if (hit.transform.tag == "Tile")
                {
                    hit.transform.GetComponent<Renderer>().material.color = Color.red;
                    for (int i = 0; i < currShape.GetComponent<ShapeController>().points.Count; i++)
                    {
//                        GameScript.tiles[hit.transform.GetComponent<TileScript>().coords.x + currShape.GetComponent<ShapeController>().points[i].x, hit.transform.GetComponent<TileScript>().coords.y + currShape.GetComponent<ShapeController>().points[i].y].transform.GetComponent<Renderer>().material.color = Color.red; ;


                    }
                }
            }
        }
        
        
    }
}
