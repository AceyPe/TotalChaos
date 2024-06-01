using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnClientConnected(ulong clientId)
    {
        StartCoroutine(SetupPlayerCamera(clientId));
    }

    IEnumerator SetupPlayerCamera(ulong clientId)
    {
        yield return new WaitForSeconds(0.5f); // Wait for the player to be fully initialized

        GameObject player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).gameObject;
        if (player != null)
        {
            Debug.Log("Player found: " + player.name);
            // Assign a unique name based on the client's network ID
            player.name = "Player_" + clientId.ToString();   
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }
}