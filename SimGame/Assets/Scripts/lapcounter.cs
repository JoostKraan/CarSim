using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

public class lapcounter : MonoBehaviour
{
    public GameObject Car;
    public GameObject trigger;
    public GameObject LapsON;
    public int laps;
    public int lapsfinished;
    public float Counter;
    public bool TimerStart;
    TMP_Text Ftext;
    TMP_Text lc;

    // Start is called before the first frame update
    void Start()
    {
        Ftext = GameObject.Find("Canvas/Time").GetComponent<TextMeshProUGUI>();
        lc = GameObject.Find("Canvas/laps").GetComponent<TextMeshProUGUI>();

        Counter = 0;
        laps = 3;
        lapsfinished = 0;
    }
    public void Update()
    {
        if (TimerStart)
        {
            Counter = Counter + Time.deltaTime;
        }
        
        Ftext.text = Math.Round(Counter, 2).ToString();
        lc.text = lapsfinished.ToString();
    }
    public void StartTimer()
    {
        TimerStart = true;
    }
    public void StopTimer()
    {
        TimerStart = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (trigger)
        {
            LapsON.SetActive(true);
            trigger.SetActive(false);
            TimerStart = true;
        }

        if(Car == other.gameObject)
        {
            laps -= 1;
            lapsfinished += 1;
            
        }

        if(laps == 0)
        {
            //Finish\\
        }
    }
}
