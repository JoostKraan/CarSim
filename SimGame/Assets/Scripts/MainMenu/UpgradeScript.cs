using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeScript : MonoBehaviour
{

    public TextMeshProUGUI headText, upgradeButton_Text, moneyAmountText, upgradeDetailText;
    public CarManager carManager;
    public Button upgradeButton;

    public GameObject upgradePanel;

    private bool upgraded_Engine = false, upgraded_Turbo = false;

    private int enginePartIndex;
    public int engineUpgradeAmount = 450, turboUpgradeAmount = 300;

    //public float engineUpgradeValue = 50;

    public AudioSource idleCarSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpgradeButtonBehaviour();
        //if (Input.GetKeyDown(KeyCode.Space)) print(CarManager.maxSpeed);
    }

    //Hoe de upgrade button moet reageren
    private void UpgradeButtonBehaviour()
	{
        if (enginePartIndex == 0)
        {
            if (!upgraded_Engine)
            {
                if (MoneyManager.money < engineUpgradeAmount) upgradeButton.interactable = false;
                else upgradeButton.interactable = true;
                upgradeButton_Text.text = "Upgrade";
            }
            else
            {
                upgradeButton.interactable = false;
                upgradeButton_Text.text = "MAX";
            }
        }
        else if (enginePartIndex == 1)
        {
            if (!upgraded_Turbo)
            {
                if (MoneyManager.money < turboUpgradeAmount) upgradeButton.interactable = false;
                else upgradeButton.interactable = true;
                upgradeButton_Text.text = "Upgrade";
            }
            else
            {
                upgradeButton.interactable = false;
                upgradeButton_Text.text = "MAX";
            }
        }
    }

    //Algemene button functie voor upgraden
    public void UpgradeButtonLogic()
	{
        if (enginePartIndex == 0)
        {
            if(MoneyManager.money >= engineUpgradeAmount)
			{
                //CarManager.maxSpeed += 30;
                upgraded_Engine = true;
                MoneyManager.money -= engineUpgradeAmount;
                idleCarSound.pitch += 0.05f;
			}
        }
        else if (enginePartIndex == 1)
		{
            if(MoneyManager.money >= turboUpgradeAmount)
			{
                //CarManager
                upgraded_Turbo = true;
                MoneyManager.money -= turboUpgradeAmount;
            }
		}
	}

    //Button functie voor engine upgrade
    public void OnEngineClick()
	{
        enginePartIndex = 0;
        upgradePanel.SetActive(true);
        headText.text = "Engine";
        moneyAmountText.text = "$" + engineUpgradeAmount.ToString();
        //upgradeDetailText.text = 
    }

    //Button functie voor turbo upgrade
    public void OnTurboClick()
	{
        enginePartIndex = 1;
        upgradePanel.SetActive(true);
        headText.text = "Turbo";
        moneyAmountText.text = "$" + turboUpgradeAmount.ToString();
        //upgradeDetailText.text = 
    }

    //Upgrade panel handmatig sluiten
    public void BackOffUpgrade() => upgradePanel.SetActive(false);
}
