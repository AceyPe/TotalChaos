using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] BoxCollider Car;
    [SerializeField] Transform car;
    BoxCollider ground;

    // Start is called before the first frame update
    void Start()
    {
        ground = GetComponent<BoxCollider>();
        Car = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        car.position = new Vector3(-1, 21, -18);
    }
}
