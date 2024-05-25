using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] wheelMesh = new GameObject[4];

    [SerializeField] float torque = 200;
    [SerializeField] float steeringMax = 25;
    [SerializeField] int frontWheels = 2;
    [SerializeField] int backWheels = 2;
    int currentWheel = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        animateWheels();
        float verticalMov = Input.GetAxis("Vertical");
        float horizontalMov = Input.GetAxis("Horizontal");
 

        foreach (var wheel in wheels)
        {
            currentWheel++;
            if(currentWheel > frontWheels + backWheels)
            {
                currentWheel = 1;
            }
            if(currentWheel -2 <= backWheels)
                wheel.motorTorque = torque * verticalMov;
            if (currentWheel <= frontWheels)
            {
                wheel.steerAngle = steeringMax * horizontalMov;
            }
        }

       
    }

    void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;
        for (int i = 0; i< 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }        
    }
}
