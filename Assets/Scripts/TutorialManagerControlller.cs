using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManagerControlller : MonoBehaviour
{
    public GameObject player;
    public GameObject infoText;
    public GameObject buttSpecial;
    public GameObject buttonStart;
    public GameObject buttonEnemies;
    public GameObject buttonPlay;
    public GameObject inventory;
    public GameObject special;
    public GameObject infoDiff;

    private int i = 0;
    private int on = 0;
    private int step = 0;
    private int iny_step = 0;
    private int iny_step1 = 0;
    private int iny_step2 = 0;
    private int iny_step3 = 0;
    private int iny_step4 = 0;
    // Start is called before the first frame update
    void Start()
    {
        infoText.GetComponent<TextMeshProUGUI>().text = "Story\n" +
            "You play as Elkrond, the spirit of the " +
            "forest who protects it.\n" +
            "He can control the fog\n" +
            "which he uses to trap and kill enemies.\n" +
            "He can travel only via fog.";
    }

    // Update is called once per frame
    void Update()
    {
        if (on == 1)
        {
            inventory.SetActive(true);
            buttonStart.SetActive(false);
            infoText.GetComponent<TextMeshProUGUI>().text = "Inventory\n" +
              "You can choose various shapes of fog\n" +
              "to cast from your inventory.\n" +
              "Hover and click on the tiles on the right.";
            if (player.GetComponent<PlayerController>().currShape != null)
            {
                step = 1;
                on = 0;
            }

        }

        if (step == 1)
        {
            infoText.GetComponent<TextMeshProUGUI>().text = "Press W, S, A or D to control the camera.\n" +
                "Scroll with the mouse wheel to zoom.\n" +
                "Use Q and E to rotate the shape." +
              "";
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            {
                iny_step = 1;
                step = 0;
            }
        }

        if (iny_step == 1) {
            infoText.GetComponent<TextMeshProUGUI>().text = "Click on a tile to cast fog.\n" +
                    "You can only cast fog if it is directly connected to the player's tile.";
            if (player.GetComponent<PlayerController>().currShape == null)
            {
                iny_step1 = 1;
                iny_step = 0;


            }
        }

        if (iny_step1 == 1)
        {
            infoText.GetComponent<TextMeshProUGUI>().text = "Now you can move through the fog\n" +
                "by clicking on a particular tile covered by fog." +
                "";
            buttonEnemies.SetActive(true);
            if (i == 1)
            {
                buttonEnemies.SetActive(false);
                infoText.GetComponent<TextMeshProUGUI>().text = "travel" +
            "";
                iny_step2 = 1;
                iny_step1 = 0;
            }
        }

        if (iny_step2 == 1)
        {
            if (player.transform.position != new Vector3(0, 0, 0))
            {
                infoText.GetComponent<TextMeshProUGUI>().text = "Enemies spawn from the top.\n" +
                    "Trap an enemy into the fog to stop their movement.\n" +
                    "Walk up to the enemy to kill them.\n" +
                    "You can see possible moves of an enemy by hovering over them.\n" +
                    "Now, try to kill an enemy.\n" +
        "";
                infoDiff.SetActive(true);
                iny_step3 = 1;
                iny_step2 = 0;
                
            }
        }

        if (iny_step3 == 1)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 1)
            {
                special.SetActive(true);
                buttSpecial.GetComponent<Button>().interactable = true;
                infoText.GetComponent<TextMeshProUGUI>().text = "Kill enemies to collect points\n" +
                    "for a special ability \"vidmo\"\n" +
                    "which immediately kills all enemies within the area of the spell.\n" +
                    "You need 20 points to cast this spell." +
            ""; iny_step4 = 1;
                iny_step3 = 0;
                //Debug.Log(GameScript.enemiesAlive.Count);
                
            }
        }

        if (iny_step4 == 1)
        {
            if (GameScript.enemiesAlive.Count == 0)
            {
                buttonPlay.SetActive(true);
                infoText.GetComponent<TextMeshProUGUI>().text = "You have 5 lives in total.\n" +
                    "When an enemy reaches the bottom of the grid you will lose one life.\n" +
        "";
            }
        }
    }

    public void ButtonClicked()
    {
        on = 1;
        
    }

    public void EnemyCross()
    {
        i = 1;
    }
}
