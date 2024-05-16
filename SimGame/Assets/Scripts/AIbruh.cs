using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Aibruh : CarManager
{
    public GameObject[] checkpoints;
    [SerializeField] int CurTarget;
    [SerializeField] Vector3 enemyDirectionLocal;
    
    void Start()
    {
        Init();
    }

    void Update()
    {
        enemyDirectionLocal = gameObject.transform.InverseTransformPoint(checkpoints[CurTarget].transform.position);

        var steer = Mathf.Clamp(Remap(-enemyDirectionLocal.x / (Vector3.Distance(checkpoints[CurTarget].transform.position, gameObject.transform.position) * 0.5f), 40, -1, -40, 1), -1, 1);

        ApplyTurning(steer);
        print(steer);

        ApplyThrottle(1);


        CalcRpm();
        ChangeCameraAngle();
        UpdateLights();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == checkpoints[CurTarget])
        {
            CurTarget++;
            if (CurTarget >= checkpoints.Length)
            {
                CurTarget = 0;
            }
        }
    }
    public float Remap( float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2; 
    }
}

