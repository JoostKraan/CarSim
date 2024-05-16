using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Aibruh : CarManager
{
    public GameObject[] checkpoints;
    public float speed;
    float angle;
    public Rigidbody rb;
    [SerializeField] int CurTarget;
    public float gasinput;
    void Start()
    {
        gasinput = 1;
    }

    void Update()
    {
        //transform.position = Vector3.MoveTowards(transform.position, checkpoints[CurTarget].transform.position, speed * Time.deltaTime);
        ApplyThrottle(gasinput);
        Debug.Log(gasinput);
        /*
        throttleInput = Input.GetAxis("Vertical");
        ApplyThrottle(throttleInput);
        turnInput = Input.GetAxis("Horizontal");
        ApplyTurning(turnInput);
        */


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
}