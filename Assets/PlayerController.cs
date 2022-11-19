using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public GameObject currShape;
    private GameObject[] currTiles;
    private Color color = Color.green;
    private int currShapeIndex = 0;

    private ShapeController scToUse;
    private Vector2Int currTileCoords;
    
    public GameScript gs;

    public InventoryController ic;

    public int totalShapes;
    // Start is called before the first frame update
    void Start()
    {
        totalShapes = transform.Find("Shapes").transform.childCount; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(currShapeIndex);

            currShape = transform.Find("Shapes").transform.GetChild(currShapeIndex).gameObject;
            if (currShapeIndex < totalShapes - 1)
            {
                currShapeIndex += 1;
            }
            else
            {
                currShapeIndex = 0;
            }
        }
        /*if (currShape)
        {
            Debug.Log(currShape.name);

        }*/
        if (GameScript.phase == 2)
        {
            if (currShape)
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
                            int y = hit.transform.GetComponent<TileScript>().coords.y + currShape.GetComponent<ShapeController>().points[i].y;
                            int x = hit.transform.GetComponent<TileScript>().coords.x + currShape.GetComponent<ShapeController>().points[i].x;
                            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
                            {
                                break;
                            }
                            if ((inBounds(y + 1, x) && GameScript.tiles[y + 1][x].GetHasFog()) ||
                                (inBounds(y - 1, x) && GameScript.tiles[y - 1][x].GetHasFog()) ||
                                (inBounds(y, x + 1) && GameScript.tiles[y][x + 1].GetHasFog()) ||
                                (inBounds(y, x - 1) && GameScript.tiles[y][x - 1].GetHasFog()))
                            {
                                color = Color.green;
                                break;
                            }
                            else
                            {
                                color = Color.red;
                            }
                            


                        }


                        for (int i = 0; i < currShape.GetComponent<ShapeController>().points.Count; i++)
                        {
                            int y = hit.transform.GetComponent<TileScript>().coords.y + currShape.GetComponent<ShapeController>().points[i].y;
                            int x = hit.transform.GetComponent<TileScript>().coords.x + currShape.GetComponent<ShapeController>().points[i].x;
                            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
                            {
                                break;
                            }
                            GameScript.tiles[y][x].transform.GetComponent<Renderer>().material.color = color;
                            

                        }
                        if (Input.GetMouseButtonDown(0) && color == Color.green)
                        {
                            scToUse = currShape.GetComponent<ShapeController>();
                            currTileCoords = new Vector2Int(hit.transform.GetComponent<TileScript>().coords.x, hit.transform.GetComponent<TileScript>().coords.y);


                            SwapShape();
                            currShape = null;
                            GameScript.phase = 3;
                            gs.MoveEnemies();
                        }

                    }
                }
            }
           
        }   
    }

    public void MarkFog()
    {
        for (int i = 0; i < scToUse.points.Count; i++)
        {
            int y = currTileCoords.y + scToUse.points[i].y;
            int x = currTileCoords.x + scToUse.points[i].x;
            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
            {
                break;
            }
            GameScript.tiles[y][x].GetComponent<TileScript>().SetFog(true);
        }
    }

    public bool inBounds(int x, int y)
    {
        /*return !(GameScript.width < x && GameScript.height < y && 0 >= x && 0 >= y);*/
        return (x >= 0 && y >= 0 && x < GameScript.width && y < GameScript.height);
    }

    public void SwapShape()
    {
        int randInt = Random.Range(3, totalShapes);
        int sibIndex = transform.Find("Shapes").transform.GetChild(randInt).gameObject.transform.GetSiblingIndex();
        transform.Find("Shapes").transform.GetChild(randInt).gameObject.transform.SetSiblingIndex(currShape.GetComponent<ShapeController>().index);
        currShape.transform.SetSiblingIndex(sibIndex);

        for (int i=0; i<totalShapes; i++)
        {
            transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().index = transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().transform.GetSiblingIndex();
        }

        ic.UpdateInventory();
    }
}
