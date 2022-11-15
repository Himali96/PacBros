using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        
        playerTransform = FindObjectOfType<ThirdPersonMovement>().gameObject.transform;
        goPlayer = playerTransform.gameObject;
        playerPowerUp = goPlayer.GetComponent<ThirdPersonMovement>().HasPowerUp;


        if (playerPowerUp && Vector3.Distance(transform.position, playerTransform.position) <= CloseEnoughDistance)
        {
            agent.destination = spawnPoint.transform.position;
        }
        else
        {
            if (playerTransform != null)
            {
                agent.destination = playerTransform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && playerPowerUp == false)
        {
            Destroy(other.gameObject);
        }
    }
}
