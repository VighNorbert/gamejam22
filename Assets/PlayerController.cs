using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject healthUI;
    [HideInInspector]
    public GameObject currShape;
    private GameObject[] currTiles;
    private Color color = Color.green;
    private int currShapeIndex = 0;

    private List<Vector2Int> shapeToUse = new List<Vector2Int>();

    private Vector2Int currTileCoords;
    
    public GameScript gs;

    public InventoryController ic;

    public int totalShapes;

    private int xNegativeRotation = 1;
    private int yNegativeRotation = 1;
    private bool swap = false;
    private int angle = 0;

    public static int health = 5;
    // Start is called before the first frame update
    void Start()
    {
        totalShapes = transform.Find("Shapes").transform.childCount; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (angle < 360)
            {
                angle = ((angle - 90) + 360) % 360;
            }
            else
            {

            angle = (angle - 90) % 360;
            }
            HandleAngles();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            angle = (angle + 90) % 360;
            HandleAngles();
        }


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
                            int pointsY = currShape.GetComponent<ShapeController>().points[i].y;
                            int pointsX = currShape.GetComponent<ShapeController>().points[i].x;

                            if (swap)
                            {
                                int swap;
                                swap = pointsX;
                                pointsX = pointsY;
                                pointsY = swap;
                            }

                            pointsX = xNegativeRotation * pointsX;
                            pointsY = yNegativeRotation * pointsY;


                            int y = hit.transform.GetComponent<TileScript>().coords.y + pointsY;
                            int x = hit.transform.GetComponent<TileScript>().coords.x + pointsX;

                            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
                            {
                                contine;
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


                            int pointsY = currShape.GetComponent<ShapeController>().points[i].y;
                            int pointsX = currShape.GetComponent<ShapeController>().points[i].x;

                            if (swap)
                            {
                                int swap;
                                swap = pointsX;
                                pointsX = pointsY;
                                pointsY = swap;
                            }

                            pointsX = xNegativeRotation * pointsX;
                            pointsY = yNegativeRotation * pointsY;


                            int y = hit.transform.GetComponent<TileScript>().coords.y + pointsY;
                            int x = hit.transform.GetComponent<TileScript>().coords.x + pointsX;


                            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
                            {
                                continue;
                            }
                            GameScript.tiles[y][x].transform.GetComponent<Renderer>().material.color = color;
                            
                            if (currShape.GetComponent<ShapeController>().points.Count > shapeToUse.Count)
                            {

                            shapeToUse.Add(new Vector2Int(pointsX, pointsY));
                            }

                        }
                        
                        if (Input.GetMouseButtonDown(0) && color == Color.green)
                        {
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
        Debug.Log(shapeToUse.Count);
        for (int i = 0; i < shapeToUse.Count; i++)
        {
            int y = currTileCoords.y + shapeToUse[i].y;
            int x = currTileCoords.x + shapeToUse[i].x;
            Debug.Log(shapeToUse[i].y);
            Debug.Log(shapeToUse[i].x);
            if (GameScript.width <= x || GameScript.height <= y || 0 > x || 0 > y)
            {
                continue;
            }
            GameScript.tiles[y][x].GetComponent<TileScript>().SetFog(true);
        }
        shapeToUse.Clear();
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


    public void HandleAngles()
    {
        shapeToUse.Clear();
        if (angle == 0)
        {
            xNegativeRotation = 1;
            yNegativeRotation = 1;
            swap = false;
        }
        else if (angle == 90)
        {
            xNegativeRotation = 1;
            yNegativeRotation = -1;
            swap = true;

        }
        else if (angle == 180)
        {
            xNegativeRotation = -1;
            yNegativeRotation = -1;
            swap = false;
        }
        else if (angle == 270)
        {
            xNegativeRotation = -1;
            yNegativeRotation = 1;
            swap = true;
        }
    }

    public static int GetHealth()
    {
        return health;
    }

    public static void HealthDown()
    {
        health -= 1;
        if (health <= 0)
        {
            Debug.Log("Game Over");
        }
    }
}
