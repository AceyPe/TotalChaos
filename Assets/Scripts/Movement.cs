using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody = new Rigidbody();
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            rigidbody.AddForce(new Vector3(20, 20, 20), ForceMode.Impulse);
        }
    }
}
