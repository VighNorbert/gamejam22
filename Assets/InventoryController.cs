using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject player;

    void Awake()
    {
        for (int i=0; i<3; i++)
        {
            transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = player.transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().image;
        }
    }

    public void UseShape(int shape)
    {
        var pc = player.GetComponent<PlayerController>();
        pc.currShape = player.transform.Find("Shapes").transform.GetChild(shape).gameObject;
        pc.SetHasSpecialAbility(false);
        pc.clearShapeToUse();
        
    }


    public void UpdateInventory()
    {
        for (int i = 0; i < 3; i++)
        {
            transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = player.transform.Find("Shapes").transform.GetChild(i).GetComponent<ShapeController>().image;
        }
    }

}
