using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Aibruh : MonoBehaviour
{
    public GameObject[] checkpoints;
    [SerializeField] int CurTarget;
    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);

        if (other.gameObject == checkpoints[CurTarget])
        {
            CurTarget++;
            if (CurTarget >= checkpoints.Length)
            {
                CurTarget = 0;
            }
        }
    }
}