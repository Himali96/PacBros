using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class GhostNavAgent : MonoBehaviour
{
    public Transform playerTransform;
    public NavMeshAgent agent;
    GameObject goPlayer,spawnPoint;
    bool playerPowerUp;
    public float CloseEnoughDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("EnemySpawnPoint");

        GameManager._instance.OnPlayerListChange += OnPlayerListChange;
    }

    void OnDestroy()
    {
        if(GameManager._instance)
            GameManager._instance.OnPlayerListChange -= OnPlayerListChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (playerTransform == null) return;
        
        playerPowerUp = goPlayer.GetComponent<ThirdPersonMovement>().HasPowerUp; // This can be improved

        if (playerPowerUp && Vector3.Distance(transform.position, playerTransform.position) <= CloseEnoughDistance)
        {
            agent.destination = spawnPoint.transform.position;
        }
        else
        {
            agent.destination = playerTransform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if(other.gameObject.CompareTag("Player") && playerPowerUp == false)
        {
            PhotonNetwork.Destroy(other.gameObject);
            GameManager._instance.PlayerDie(other.GetComponent<PhotonView>());
        }
    }

    void OnPlayerListChange()
    {
        playerTransform = GameManager._instance.GetPlayerToFollow();
        if(playerTransform)
        {
            goPlayer = playerTransform.gameObject;
        }
    }
}
