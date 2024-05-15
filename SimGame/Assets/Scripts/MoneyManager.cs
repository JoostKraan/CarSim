using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static int money;
    public TextMeshProUGUI playerMoney_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMoney_text.text = "$" + money.ToString();
        if (Input.GetKeyDown(KeyCode.RightControl)) money += 1000;
    }
}
