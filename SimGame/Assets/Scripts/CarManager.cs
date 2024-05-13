using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarManager : MonoBehaviour
{

    [Header("States")]
    [SerializeField] public bool canDrive = false;
    [SerializeField] private bool engineRunning = false;
    [SerializeField] public float speed;

    [Header("Transforms")]
    public Transform wFR;
    public Transform wFL;
    public Transform wRR;
    public Transform wRL;
    public Transform steeringWheel;

    public Camera chaseCam;
    public Camera rearFacingcam;
    public Camera firstPersoncam;


    [Header("Wheels Colliders")]
    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider RearRight;
    [SerializeField] WheelCollider RearLeft;
    [Header("Car Stats")]
    [SerializeField] private float acceleration = 500f;
    [SerializeField] private float brakeForce = 300f;
    [SerializeField] private float maxTurnangle = 35f;
    [SerializeField] private float currentAcceleration = 0f;
    [SerializeField] private float currentBrakeForce = 0f;
    [SerializeField] private float currentTurnangle = 0f;

    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            chaseCam.enabled = false;
            rearFacingcam.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            chaseCam.enabled = true;
            rearFacingcam.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            firstPersoncam.enabled = true;
            chaseCam.enabled = false;
            rearFacingcam.enabled = false;
           
        }
    }
    private void FixedUpdate()
    {
        speed = rb.velocity.magnitude;

        currentAcceleration = acceleration * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakeForce = brakeForce;

        }
        else
        {
            currentBrakeForce = 0f;
        }
        FrontRight.motorTorque = currentAcceleration;
        FrontLeft.motorTorque = currentAcceleration;
        RearLeft.motorTorque = currentAcceleration;
        RearRight.motorTorque = currentAcceleration;

        FrontRight.brakeTorque = currentBrakeForce;
        FrontLeft.brakeTorque = currentBrakeForce;
        RearRight.brakeTorque = currentBrakeForce;
        RearLeft.brakeTorque = currentBrakeForce;

        currentTurnangle = maxTurnangle * Input.GetAxis("Horizontal");
        FrontLeft.steerAngle = currentTurnangle;
        FrontRight.steerAngle = currentTurnangle;
        UpdateWheel(FrontLeft, wFL);
        UpdateWheel(FrontRight, wFR);
        UpdateWheel(RearLeft, wRL);
        UpdateWheel(RearRight, wRR);
    }
    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }



}






