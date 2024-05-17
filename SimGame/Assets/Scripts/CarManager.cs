using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
    public bool lOn = false;
    public Light flHeadlight;
    public Light frHeadlight;
    public Light flSpotlight;
    public Light frSpotlight;

    public Light rlTaillight;
    public Light rrTaillight;

    private Camera[] cameras;
    public Camera chaseCam;
    public Camera rearFacingcam;
    public Camera firstPersoncam;
    public Camera fendercam;
    public Camera PedalCam;
    public Camera RearFenderCam;
    private int currentCameraIndex = 0;

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
    public bool controllerConnected;
    public bool isKeyPressed = false;
    bool ispressed;

    float GetForward() => playerInput.actions["Throttle"].ReadValue<float>();
    float GetBrake() => playerInput.actions["Brake"].ReadValue<float>();
    float GetShiftUp() => playerInput.actions["ShiftUp"].ReadValue<float>();
    float GetShiftDown() => playerInput.actions["ShiftDown"].ReadValue<float>();
    float GetCamera() => playerInput.actions["Camera"].ReadValue<float>();
    float GetLights() => playerInput.actions["Lights"].ReadValue<float>();

    [SerializeField] private PlayerInput playerInput;
    public float turnInput;
    public float throttleInput;

    Rigidbody rb;

    private float cameraSwitchCooldown = 0.5f; // Cooldown period in seconds
    private float cameraSwitchTimestamp;

    private void Start()
    {
        cameras = new Camera[] { rearFacingcam, chaseCam, firstPersoncam, fendercam, PedalCam, RearFenderCam };
        UpdateCamera();
        AdjustFrictionProperties();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (GetCamera() > 0 && Time.time > cameraSwitchTimestamp)
        {
            cameraSwitchTimestamp = Time.time + cameraSwitchCooldown;
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;
            UpdateCamera();
        }

        rpmText.text = $"RPM  {RearLeft.rpm.ToString("F1")}";

        if (controllerConnected)
        {
            throttleInput = GetForward();
        }
        else
        {
            throttleInput = Input.GetAxis("Vertical");
        }

        ApplyThrottle(throttleInput);
        turnInput = Input.GetAxis("Horizontal");
        ApplyTurning(turnInput);
        UpdateLights();
        Gearing();
    }

    private void Gearing()
    {
        if (Input.GetKey(KeyCode.LeftShift) || GetShiftUp() > 0)
        {
            if (currentGear > 1)
            {
                currentGear = 1;
            }
            currentGear++;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || GetShiftDown() > 0)
        {
            if (currentGear < 0)
            {
                currentGear = 0;
            }
            currentGear--;
        }
    }

    void UpdateCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = (i == currentCameraIndex);
        }
    }

    private void UpdateLights()
    {
        
        if (Input.GetKeyDown(KeyCode.H) || GetLights() > 0 && ispressed)
        {
            
            Debug.Log(GetLights());
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
        float steerAngle = turnAngle * maxTurnangle;

        FrontLeft.steerAngle = steerAngle;
        FrontRight.steerAngle = steerAngle;

        RotateSteeringWheel(steerAngle);
    }

    private void RotateSteeringWheel(float steerAngle)
    {
        float steeringWheelRotationMultiplier = 8f;
        steeringWheel.localRotation = Quaternion.Euler(new Vector3(-15.957f, -180, steerAngle * steeringWheelRotationMultiplier));
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyHandbrake();
        }
        else if (Input.GetKey(KeyCode.S) || GetBrake() > 0)
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
        Brake.localRotation = Quaternion.Euler(new Vector3(-15.957f, -180, 0));
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

        forwardFriction.stiffness = 1; // Increase forward grip
        sidewaysFriction.stiffness = 1; // Increase sideways grip

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
