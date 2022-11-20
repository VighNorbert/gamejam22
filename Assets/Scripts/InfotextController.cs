using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfotextController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().alpha > 0)
        {
            transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().alpha -= Time.deltaTime * 0.2f;
        }
    }

    public void UpdateInfoText(string _string)
    {
        transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().alpha = 1;
        transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = _string;
    }
}
