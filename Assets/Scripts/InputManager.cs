using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    float horizontal;
    float vertical;
    bool handBrake;

    void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        handBrake = (Input.GetAxis("Jump" ) != 0) ? true : false;
    }


    public float getHorizontal()
    {
        return horizontal;
    }
    public float getVertical()
    {
        return vertical;
    }
    public bool isHandBrake()
    {
        return handBrake;
    }
}
