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
    public Camera fendercam;
    [Header("Wheels Colliders")]
    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider RearRight;
    [SerializeField] WheelCollider RearLeft;

    [Header("Gearing")]
    [SerializeField] public float[] gearRatios = { 3.66f, 2.43f, 1.69f, 1.32f, 1.0f };
    [SerializeField] private int maxGears = 5;
    [SerializeField] private int currentGear = 1;
    [SerializeField] private int maxRpm = 7200;
    [SerializeField] private int currentRpm = 0;

    [Header("Car Stats")]
    [SerializeField] private float maxSpeed = 100f; // Adjust maximum speed
    [SerializeField] private float accelerationRate = 500f; // Adjust acceleration rate
    [SerializeField] private float decelerationRate = 1000f; // Adjust deceleration rate
    [SerializeField] private float brakeTorque = 500f; // Adjust brake torque
    [SerializeField] private float handbrakeTorque = 1000f; // Adjust handbrake torque
    [SerializeField] private float maxTurnangle = 50f;

    Rigidbody rb;
    public float throttleInput;
    private void Start()
    {
        AdjustFrictionProperties();
        rb = GetComponent<Rigidbody>();
        
    }

    private void CalcRpm()
    {
        currentGear = Mathf.Clamp(currentGear, 1, maxGears);

        float wheelRpm = (RearLeft.rpm + RearRight.rpm) / 2f;
        float finalDriveRatio = gearRatios[currentGear - 1]; // Get the gear ratio for the current gear

        // Calculate engine RPM based on wheel speed and gear ratio
        currentRpm = (int)(wheelRpm * finalDriveRatio * 60f);
        if (currentRpm > maxRpm)
        {
            currentRpm = maxRpm;
        }
        else if (currentRpm < 0)
        {
            currentRpm = 0;
        }
    }


    private void Update()
    {
        CalcRpm();
        Debug.Log(currentRpm);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            rearFacingcam.enabled = true;
            chaseCam.enabled = false;
            firstPersoncam.enabled = false;
            fendercam.enabled = false;

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            chaseCam.enabled = true;
            rearFacingcam.enabled = false;
            firstPersoncam.enabled = false;
            fendercam.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            firstPersoncam.enabled = true;
            fendercam.enabled = false;
            chaseCam.enabled = false;
            rearFacingcam.enabled = false;
           
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            fendercam.enabled = true;
            firstPersoncam.enabled = false;
            chaseCam.enabled = false;
            rearFacingcam.enabled = false;
            

        }
    }
    private void FixedUpdate()
    {
       
        speed = rb.velocity.magnitude;

        throttleInput = Input.GetAxis("Vertical");

        if (throttleInput == 0 && speed <= 0.2f)
        {
            rb.drag = 1000f;
        }
        else
        {
            rb.drag = 0.005f;
        }

        // Apply acceleration
        if (throttleInput > 0 && speed < maxSpeed)
        {
            float torque = accelerationRate * throttleInput;
            ApplyTorqueToWheels(torque);
        }
        // Apply deceleration
        else if (throttleInput <= 0 && speed > 0)
        {
            float torque = -decelerationRate * Time.deltaTime;
            ApplyTorqueToWheels(torque);
        }

        // Apply braking
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyHandbrake();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ApplyBrake();
        }
        else
        {
            ReleaseBrake();
        }

        // Apply steering
        float turnAngle = maxTurnangle * Input.GetAxis("Horizontal");
        FrontLeft.steerAngle = turnAngle;
        FrontRight.steerAngle = turnAngle;

        // Update wheel positions and rotations
        UpdateWheel(FrontLeft, wFL);
        UpdateWheel(FrontRight, wFR);
        UpdateWheel(RearLeft, wRL);
        UpdateWheel(RearRight, wRR);
    }

    private void ApplyTorqueToWheels(float torque)
    {
        FrontRight.motorTorque = torque;
        FrontLeft.motorTorque = torque;
        RearLeft.motorTorque = torque;
        RearRight.motorTorque = torque;
    }

    private void ApplyBrake()
    {
        FrontRight.brakeTorque = brakeTorque;
        FrontLeft.brakeTorque = brakeTorque;
        RearRight.brakeTorque = brakeTorque;
        RearLeft.brakeTorque = brakeTorque;
    }

    private void ApplyHandbrake()
    {
        RearRight.brakeTorque = handbrakeTorque;
        RearLeft.brakeTorque = handbrakeTorque;
    }

    private void ReleaseBrake()
    {
        FrontRight.brakeTorque = 0f;
        FrontLeft.brakeTorque = 0f;
        RearRight.brakeTorque = 0f;
        RearLeft.brakeTorque = 0f;
    }

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

    
    private void AdjustFrictionProperties()
    {
        WheelFrictionCurve forwardFriction = FrontRight.forwardFriction;
        WheelFrictionCurve sidewaysFriction = FrontRight.sidewaysFriction;

        // Increase grip by adjusting the stiffness of the friction curves
        forwardFriction.stiffness = 1; // Increase forward grip
        sidewaysFriction.stiffness = 1; // Increase sideways grip

        // Apply the adjusted friction curves to all wheels
        FrontRight.forwardFriction = forwardFriction;
        FrontLeft.forwardFriction = forwardFriction;
        RearRight.forwardFriction = forwardFriction;
        RearLeft.forwardFriction = forwardFriction;

        FrontRight.sidewaysFriction = sidewaysFriction;
        FrontLeft.sidewaysFriction = sidewaysFriction;
        RearRight.sidewaysFriction = sidewaysFriction;
        RearLeft.sidewaysFriction = sidewaysFriction;
    }



}






