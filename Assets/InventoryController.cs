using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i=0; i<3; i++)
        {
            this.transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = player.transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().image;
        }
    }

    public void UseShape(int shape)
    {
        player.GetComponent<PlayerController>().currShape = player.transform.Find("Shapes").transform.GetChild(shape).gameObject;
    }


    public void UpdateInventory()
    {
        for (int i = 0; i < 3; i++)
        {
            this.transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = player.transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().image;
        }
    }

}
