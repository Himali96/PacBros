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

    public bool isChasingOn = true;

    public Material normalMat;
    public Material evadeMat;

    public Transform[] waypoints;

    [SerializeField]
    private int currentWaypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        startPosition = transform.position;
        spawnPoint = GameObject.FindGameObjectWithTag("EnemySpawnPoint");

        GameManager._instance.OnPlayerListChange += OnPlayerListChange;
        GameManager._instance.OnGameFinish += OnGameFinish;

        currentWaypointIndex = Random.Range(0, waypoints.Length);
    }

    void OnDestroy()
    {
        if(GameManager._instance)
        {
            GameManager._instance.OnPlayerListChange -= OnPlayerListChange;
            GameManager._instance.OnGameFinish -= OnGameFinish;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (playerTransform == null) return;
        if (isDeath) return;
        
        playerPowerUp = goPlayer.GetComponent<ThirdPersonMovement>().HasPowerUp; // This can be improved

        if(playerPowerUp)
        {
            if(Vector3.Distance(transform.position, playerTransform.position) <= CloseEnoughDistance)
            {
                agent.destination = spawnPoint.transform.position;
            }

            GetComponent<SkinnedMeshRenderer>().material = evadeMat;
        }
        else
        {
            if(isChasingOn)
                agent.destination = playerTransform.position;
            else
            {
                if(Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) > 1f)
                {
                    agent.destination = waypoints[currentWaypointIndex].position;
                }
                else
                {
                    currentWaypointIndex = Random.Range(0, waypoints.Length);
                }
            }

            GetComponent<SkinnedMeshRenderer>().material = normalMat;
        }

        /*if (playerPowerUp && Vector3.Distance(transform.position, playerTransform.position) <= CloseEnoughDistance)
        {
            agent.destination = spawnPoint.transform.position;
        }
        else
        {
            agent.destination = playerTransform.position;
        }*/
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

    void OnGameFinish()
    {
        agent.speed = 0f;
        agent.isStopped = true;
    }
    
}
