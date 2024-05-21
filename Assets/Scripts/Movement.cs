using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    Rigidbody rigidbody;
    Transform transform;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            rigidbody.AddForce(new Vector3(20,0,0),ForceMode.Impulse);
        }
        if(Input.GetKey(KeyCode.S))
        {
            rigidbody.AddForce(new Vector3(-20,0,0),ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.AddTorque(new Vector3(0, 20, 0) * 0.2f, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.AddTorque(new Vector3(0, -20, 0) * 0.2f, ForceMode.Impulse);
        }
    }
}
