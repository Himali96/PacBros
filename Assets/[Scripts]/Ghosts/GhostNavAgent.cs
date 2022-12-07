using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class GhostNavAgent : MonoBehaviour
{
    const float kTimeOfDeath = 5f;
    
    public Transform playerTransform;
    public NavMeshAgent agent;
    GameObject goPlayer,spawnPoint;
    bool playerPowerUp;
    public float CloseEnoughDistance = 5f;

    Vector3 startPosition;
    bool isDeath;

    PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        startPosition = transform.position;
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
        if (isDeath) return;
        
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
        if(other.gameObject.CompareTag("Player") && playerPowerUp == false)
        {
            PhotonView otherPv = other.GetComponent<PhotonView>();
            GameManager._instance.PlayerDie(otherPv);

            if (otherPv.AmOwner) // Only the controller of player can kill it
            {
                PhotonNetwork.Destroy(other.gameObject);
            }
        }
    }

    void OnPlayerListChange()
    {
        playerTransform = GameManager._instance.GetPlayerToFollow();

        if (playerTransform)
        {
            goPlayer = playerTransform.gameObject;
        }
    }

    public void Kill()
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        
        pv.RPC(nameof(KillRpc), RpcTarget.All);
    }

    [PunRPC]
    void KillRpc()
    {
        isDeath = true;
        if (PhotonNetwork.IsMasterClient)
        {
            transform.position = startPosition;
        }
        Invoke(nameof(Revive), kTimeOfDeath);
    }

    void Revive()
    {
        isDeath = false;
    }
    
    
    
}
