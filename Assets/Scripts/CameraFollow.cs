using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    public GameObject player;
    public Movement playerMovement;
    public GameObject cameraConstraint;
    public GameObject cameraLookAt;
    public float speed = 0f; // Initialize speed to avoid unassigned variable issues.

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            player = transform.parent.gameObject; // Assuming the camera is a child of the player
            Debug.Log("OnNetworkSpawn: Player set to " + player.name);
            SetupCamera();
        }
    }

    public void SetupCamera()
    {
        if (player != null)
        {
            cameraConstraint = player.transform.Find("CameraConstraint")?.gameObject;
            cameraLookAt = player.transform.Find("CameraLookAt")?.gameObject;
            playerMovement = player.GetComponent<Movement>();

            if (cameraConstraint == null || cameraLookAt == null)
            {
                Debug.LogError("CameraConstraint or CameraLookAt not found on player: " + player.name);
            }
        }
        else
        {
            Debug.LogError("Player is null in SetupCamera.");
        }
    }

    void FixedUpdate()
    {
        if (IsOwner && cameraConstraint != null && cameraLookAt != null)
        {
            Follow();
        }
    }

    private void Follow()
    {
        if (speed < 23)
        {
            speed = Mathf.Lerp(speed, playerMovement.getKPH() / 2, Time.deltaTime);
        }
        else
        {
            speed = 23;
        }

        transform.position = Vector3.Lerp(transform.position, cameraConstraint.transform.position, Time.deltaTime * speed);
        transform.LookAt(cameraLookAt.transform.position);
    }
}