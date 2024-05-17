
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Aibruh : CarManager
{
    [SerializeField] Vector3 enemyDirectionLocal;

    public List<GameObject> checkpoints;
    public int CurTarget;
    public int lap;

    private float speedOffset;
    private Vector3 checkpointOffset;
    private Vector3 checkpointOffsetTarget;

    private float checkpointThrottleMult;
    private float brakeDuration;
    
    void Start()
    {
        checkpointThrottleMult = 1;
        Init();

        checkpoints = GameObject.FindGameObjectsWithTag("checkpoint").OrderBy(go => go.name).ToList();
        //checkpoints.Reverse();

        var meshes = GetComponentsInChildren<MeshRenderer>();
        var randomColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        foreach (var mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                if (mat.name.Contains("PAINT"))
                {
                    mat.color = randomColor;
                }
            }
        }

        RecalcOffsets();
    }


    void Update()
    {
        print(brakeDuration);
        enemyDirectionLocal = gameObject.transform.InverseTransformPoint(checkpoints[CurTarget].transform.position) + checkpointOffset;

        float strength = 100;
        var steer = Mathf.Clamp(Remap(-enemyDirectionLocal.x / (1 + Vector3.Distance(checkpoints[CurTarget].transform.position, gameObject.transform.position) * 0.25f), strength, -1, -strength, 1), -1, 1);

        ApplyTurning(steer);

        var cpoffset = Vector3.MoveTowards(checkpointOffset, checkpointOffsetTarget, Time.deltaTime * 0.1f);
        checkpointOffset = cpoffset;

        var throttle = (1 + speedOffset) * checkpointThrottleMult;
        if (brakeDuration > 0)
        {
            brakeDuration -= Time.deltaTime;

            ApplyBrake();
        }
        else
        {
            ApplyThrottle(throttle);
            print(throttle);

            ReleaseBrake();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == checkpoints[CurTarget])
        {
            RecalcOffsets();

            var aiCheck = other.gameObject.GetComponent<AiCheck>();
            checkpointThrottleMult = aiCheck.speedMultiplier;
            checkpointThrottleMult = Mathf.Clamp(checkpointThrottleMult, 0.1f, 1f);
            if (aiCheck.brakeTrigger)
            {
                brakeDuration = aiCheck.brakeDuration;
            }

            CurTarget++;
            if (CurTarget >= checkpoints.Count)
            {
                CurTarget = 0;
            }
        }
    }
    public float Remap( float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2; 
    }

    private void RecalcOffsets()
    {
        checkpointOffsetTarget = Random.onUnitSphere * 2;
        speedOffset = Random.Range(-0.25f, 0.5f);
    }
}

