using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public Transform Accelerator;
    public Transform Brake;

    [Header("Lights'")]
    public bool lOn = true;
    public Light flHeadlight;
    public Light frHeadlight;
    public Light flSpotlight;
    public Light frSpotlight;

    public Light rlTaillight;
    public Light rrTaillight;


    public Camera chaseCam;
    public Camera rearFacingcam;
    public Camera firstPersoncam;
    public Camera fendercam;
    public Camera PedalCam;
    public Camera RearFenderCam;
    [Header("Wheels Colliders")]
    [SerializeField] WheelCollider FrontRight;
    [SerializeField] WheelCollider FrontLeft;
    [SerializeField] WheelCollider RearRight;
    [SerializeField] WheelCollider RearLeft;

    [Header("Gearing")]
    public TextMeshProUGUI rpmText;
    public float[] gearRatios = { 3.66f, 2.43f, 1.69f, 1.32f, 1.0f };
    [SerializeField] private int maxGears = 1;
    [SerializeField] private int currentGear = 1;
    private int maxRpm = 7200;
    private float currentRpm = 0;

    [Header("Car Stats")]
    [SerializeField] private float maxSpeed = 100f; // Adjust maximum speed
    [SerializeField] private float accelerationRate = 500f; // Adjust acceleration rate
    [SerializeField] private float decelerationRate = 1000f; // Adjust deceleration rate
    [SerializeField] private float brakeTorque = 500f; // Adjust brake torque
    [SerializeField] private float handbrakeTorque = 1000f; // Adjust handbrake torque
    [SerializeField] private float maxTurnangle = 50f;
    [SerializeField] private float currentTurnangle = 0f;

    public float turnInput;
    public float throttleInput;


    Rigidbody rb;
    private void Start()
    {

        AdjustFrictionProperties();
        rb = GetComponent<Rigidbody>();

    }


    private void Update()
    {
        rpmText.text = $"RPM  {RearLeft.rpm.ToString("F1")}";
        throttleInput = Input.GetAxis("Vertical");
        ApplyThrottle(throttleInput);
        turnInput = Input.GetAxis("Horizontal");
        ApplyTurning(turnInput);
        ChangeCameraAngle();
        UpdateLights();
        Gearing();
    }

    private void Gearing()
    {
        //currentRpm = (RearLeft.rpm * gearRatios[currentGear - 1] * 60) / (2 * Mathf.PI);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (currentGear > 1)
            {
                currentGear = 1;
            }
            currentGear++;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (currentGear < 0)
            {
                currentGear = 0;
            }
            currentGear--;
        }

    }


    private void ChangeCameraAngle()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            rearFacingcam.enabled = true;
            RearFenderCam.enabled = false;
            chaseCam.enabled = false;
            firstPersoncam.enabled = false;
            fendercam.enabled = false;
            PedalCam.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            chaseCam.enabled = true;
            RearFenderCam.enabled = false;
            rearFacingcam.enabled = false;
            firstPersoncam.enabled = false;
            fendercam.enabled = false;
            PedalCam.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            firstPersoncam.enabled = true;
            RearFenderCam.enabled = false;
            fendercam.enabled = false;
            chaseCam.enabled = false;
            rearFacingcam.enabled = false;
            PedalCam.enabled = false;

        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            fendercam.enabled = true;
            RearFenderCam.enabled = false;
            firstPersoncam.enabled = false;
            chaseCam.enabled = false;
            rearFacingcam.enabled = false;
            PedalCam.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PedalCam.enabled = true;
            RearFenderCam.enabled = false;
            fendercam.enabled = false;
            firstPersoncam.enabled = false;
            chaseCam.enabled = false;
            rearFacingcam.enabled = false;
        }
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            RearFenderCam.enabled = true;
            PedalCam.enabled = false;
            fendercam.enabled = false;
            firstPersoncam.enabled = false;
            chaseCam.enabled = false;
            rearFacingcam.enabled = false;
        }
    }
    private void UpdateLights()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            lOn = !lOn;
            if (!lOn)
            {
                flHeadlight.intensity = 10.3f;
                frHeadlight.intensity = 10.3f;
                flSpotlight.intensity = 1;
                frSpotlight.intensity = 1;
                rlTaillight.intensity = 0.71f;
                rrTaillight.intensity = 0.71f;
            }
            else
            {
                flHeadlight.intensity = 0;
                frHeadlight.intensity = 0;
                flSpotlight.intensity = 0;
                frSpotlight.intensity = 0;
                rlTaillight.intensity = 0;
                rrTaillight.intensity = 0;
            }
        }

    }

    private void ApplyThrottle(float throttleInput)
    {
        speed = rb.velocity.magnitude;

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
            if (currentGear >= 1)
            {
                ApplyForwardsTorque(torque);
            }
            if (currentGear == 0)
            {
                ApplyBackwardsTorque(torque);
            }
            
            PedalPressVisual(throttleInput);


        }
        // Apply deceleration
        else if (throttleInput <= 0 && speed > 0)
        {
            float torque = -decelerationRate * Time.deltaTime;

            ApplyForwardsTorque(torque);

        }
    }
    private void PedalPressVisual(float throttleInput)
    {
        Quaternion acceleratorPedalStartRotation = Quaternion.Euler(new Vector3(0, 180f, 0f));
        Quaternion acceleratorPedalEndRotation = Quaternion.Euler(new Vector3(-25, 180f, 0f));
        Accelerator.localRotation = Quaternion.Lerp(acceleratorPedalStartRotation, acceleratorPedalEndRotation, throttleInput);
    }
    private void BrakePedalVisual()
    {
        float brakeInput = -Input.GetAxis("Vertical");
        Quaternion BrakePedalStartRotation = Quaternion.Euler(new Vector3(0, 180f, 0f));
        Quaternion BrakePedalEndRotation = Quaternion.Euler(new Vector3(-25, 180f, 0f));
        
        Brake.localRotation = Quaternion.Lerp(BrakePedalStartRotation, BrakePedalEndRotation, brakeInput);

    }
    private void ApplyTurning(float turnAngle)
    {
        // Calculate the steer angle for the wheels
        float steerAngle = turnAngle * maxTurnangle;

        // Apply the steer angle to the front wheels
        FrontLeft.steerAngle = steerAngle;
        FrontRight.steerAngle = steerAngle;

        // Rotate the steering wheel based on the steer angle
        RotateSteeringWheel(steerAngle);
    }
    private void RotateSteeringWheel(float steerAngle)
    {
        float steeringWheelRotationMultiplier = 8f;

 
        steeringWheel.localRotation = Quaternion.Euler(new Vector3(-15.957f, -180, steerAngle * steeringWheelRotationMultiplier));
    }





    private void FixedUpdate()
    {

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

        UpdateWheel(FrontLeft, wFL);
        UpdateWheel(FrontRight, wFR);
        UpdateWheel(RearLeft, wRL);
        UpdateWheel(RearRight, wRR);
    }

    private void ApplyForwardsTorque(float torque)
    {
        FrontRight.motorTorque = torque;
        FrontLeft.motorTorque = torque;
        RearLeft.motorTorque = torque;
        RearRight.motorTorque = torque;
    }
    private void ApplyBackwardsTorque(float torque)
    {
        FrontRight.motorTorque = -torque;
        FrontLeft.motorTorque = -torque;
        RearLeft.motorTorque = -torque;
        RearRight.motorTorque = -torque;
    }

    private void ApplyBrake()
    {
        Debug.Log("tesdt");
        FrontLeft.wheelDampingRate = 10;
        FrontRight.wheelDampingRate = 10;
        RearRight.wheelDampingRate = 10;
        RearLeft.wheelDampingRate = 10;
        FrontRight.brakeTorque = brakeTorque;
        FrontLeft.brakeTorque = brakeTorque;
        RearRight.brakeTorque = brakeTorque;
        RearLeft.brakeTorque = brakeTorque;
        BrakePedalVisual();
    }

    private void ApplyHandbrake()
    {
        RearRight.brakeTorque = handbrakeTorque;
        RearLeft.brakeTorque = handbrakeTorque;
    }

    private void ReleaseBrake()
    {
        Brake.localRotation = Quaternion.Euler(new Vector3(-15.957f, -180,0));
        Debug.Log("tesdt");
        FrontLeft.wheelDampingRate = 0.25F;
        FrontRight.wheelDampingRate = 0.25F;
        RearRight.wheelDampingRate = 0.25F;
        RearLeft.wheelDampingRate = 0.25F;
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






