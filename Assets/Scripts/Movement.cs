using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Movement : NetworkBehaviour
{
    private InputManager IM;
    private Rigidbody rb;
    private GameObject centerOfMass;
    private bool isGrounded;

    enum driveType
    {
        frontWheelDrive,
        backWheelDrive,
        allWheelDrive
    }

    [SerializeField] driveType drive;

    int wheelNum;
    private WheelCollider[] wheelCollider;
    private GameObject[] wheelMesh;
    private GameObject wheelColliders, wheelMeshes;
    private WheelFrictionCurve forwardFriction, sidewaysFriction;
    Animator animator;
    [SerializeField] float torque = 200f;
    [SerializeField] float radius = 6f;
    [SerializeField] float DownForceValue = 50f;
    [SerializeField] float handBreakFrictionMultiplier = 2;
    [SerializeField] float KPH;
    [SerializeField] float maximumSpeed;
    [SerializeField] float breakPower;
    [SerializeField] int frontWheels = 2;
    [SerializeField] int backWheels = 2;
    int currentWheel = 0;
    private float driftFactor;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>()? GetComponent<Animator>() : null;
        wheelColliders = transform.Find("WheelsColliders").gameObject;
        wheelMeshes = transform.Find("Wheels").gameObject;
        wheelNum = wheelColliders.transform.childCount;
        wheelCollider = new WheelCollider[wheelNum];
        wheelMesh = new GameObject[wheelNum];
        for (int i = 0; i < wheelNum; i++)
        {
            wheelMesh[i] = wheelMeshes.transform.GetChild(i).gameObject;
            wheelCollider[i] = wheelColliders.transform.GetChild(i).gameObject.GetComponent<WheelCollider>();
        }
        centerOfMass = GameObject.Find("CenterOfMass");
        IM = gameObject.GetComponent<InputManager>();   
        rb = gameObject.GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.transform.localPosition;
        StartCoroutine(timedLoop());
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        animateWheels();
        moveVehicle();
        steerVehicle();
        downForce();
        getFriction();
        speedLimit();
        adjustTraction();
    }
    void moveVehicle()
    {


        foreach (var wheel in wheelCollider)
        {
            currentWheel++;
            if (currentWheel > frontWheels + backWheels)
            {
                currentWheel = 1;
            }
            
            if (currentWheel - 2 <= backWheels && drive == driveType.backWheelDrive)
                wheel.motorTorque = (torque / 2) * IM.getVertical();
            else if (currentWheel <= frontWheels && drive == driveType.frontWheelDrive)
                wheel.motorTorque = (torque / 2) * IM.getVertical();
            else if (drive == driveType.allWheelDrive)
                wheel.motorTorque = (torque / 4) * IM.getVertical();

        }

        KPH = rb.velocity.magnitude * 3.6f;

        if (IM.isHandBrake())
        {
            wheelCollider[2].brakeTorque = wheelCollider[3].brakeTorque = breakPower;
        }
        else
        {
            wheelCollider[2].brakeTorque = wheelCollider[3].brakeTorque = 0;
        }

    }
    void steerVehicle()
    {
        //acerman steering formula
        // front 
        //steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius +(1.5f / 2))) * InputHorizontal
        if (IM.getHorizontal() > 0)
        {
            wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (0.5f / 2))) * IM.getHorizontal();
            wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (0.5f / 2))) * IM.getHorizontal();
        }
        else if (IM.getHorizontal() < 0)
        {
            wheelCollider[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (0.5f / 2))) * IM.getHorizontal();
            wheelCollider[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (0.5f / 2))) * IM.getHorizontal();
        }
        else
        {
            wheelCollider[0].steerAngle = 0;
            wheelCollider[1].steerAngle = 0;
        }
    }
    void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;
        for (int i = 0; i< wheelCollider.Length; i++)
        {
            wheelCollider[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }        
    }
    private void downForce()
    {
        rb.AddForce(-transform.up * DownForceValue * rb.velocity.magnitude);
    }
    private void getFriction()
    {
        for(int i = 0; i< wheelCollider.Length; i++)
        {
            WheelHit wheelHit;
            wheelCollider[i].GetGroundHit(out wheelHit);
        }
        
    }
    private void adjustTraction()
    {
        //tine it takes to go from normal drive to drift 
        float driftSmothFactor = .7f * Time.deltaTime;

        if (IM.isHandBrake())
        {
            sidewaysFriction = wheelCollider[0].sidewaysFriction;
            forwardFriction = wheelCollider[0].forwardFriction;

            float velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * handBreakFrictionMultiplier, ref velocity, driftSmothFactor);

            foreach( var wheel in wheelCollider)
            { 
                wheel.sidewaysFriction = sidewaysFriction;
                wheel.forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
            //extra grip for the front wheels
            for (int i = 0; i < 2; i++)
            {
                wheelCollider[i].sidewaysFriction = sidewaysFriction;
                wheelCollider[i].forwardFriction = forwardFriction;
            }
            rb.AddForce(transform.forward * (KPH / 400) * 40000);
        }
        //executed when handbrake is being held
        else
        {

            forwardFriction = wheelCollider[0].forwardFriction;
            sidewaysFriction = wheelCollider[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                ((KPH * handBreakFrictionMultiplier) / 300) + 1;

            foreach ( var wheel in wheelCollider)
            { 
                wheel.forwardFriction = forwardFriction;
                wheel.sidewaysFriction = sidewaysFriction;

            }
        }

        //checks the amount of slip to control the drift
        for (int i = 2; i < 4; i++)
        {

            WheelHit wheelHit;

            wheelCollider[i].GetGroundHit(out wheelHit);

            if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -IM.getHorizontal()) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + IM.getHorizontal()) * Mathf.Abs(wheelHit.sidewaysSlip);
        }

    }
    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + KPH / 20;

        }
    }

    public float getKPH()
    {
        return KPH;
    }

    public void speedLimit()
    {
        float MPS = maximumSpeed / 3.6f;
        if (KPH > maximumSpeed)
        {
            rb.velocity = rb.velocity.normalized * MPS;
            
        }
    }

}
