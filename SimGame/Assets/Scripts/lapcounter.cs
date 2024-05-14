using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class lapcounter : MonoBehaviour
{
    public GameObject Car;
    public GameObject trigger;
    public GameObject LapsON;
    public int laps;
    public float Counter;
    public bool TimerStart;

    TMP_Text Ftext;
    // Start is called before the first frame update
    void Start()
    {
        Counter = 0;
        laps = 3;
    }
    public void Update()
    {
        if (TimerStart)
        {
            Counter = Counter + Time.deltaTime;
        }
        Ftext.text = Counter.ToString();
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
        }

        if(laps == 0)
        {
            //Finish\\
        }
    }
}
