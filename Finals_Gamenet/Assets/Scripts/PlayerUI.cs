using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TMPro.TMP_Text playerName_TXT;
    public Image hp_Bar;
    
    public GameObject[] winsIndicator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateWinsIndicator(int score)
    {
        int i = 1;
        foreach (GameObject o in winsIndicator)
        {
            if (i <= score)
            {
                o.SetActive(true);
            }

            i++;
        }
    }

    public void SetName(string playerName, string CharacterName)
    {
        playerName_TXT.text = $"{playerName}    ({CharacterName})";
    }

    public void UpdateHealthBar(float percentage)
    {
        hp_Bar.fillAmount = percentage;
    }
}
