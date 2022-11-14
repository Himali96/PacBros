using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostNavAgent : MonoBehaviour
{
    public Transform playerTransform;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<ThirdPersonMovement>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = playerTransform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(other.gameObject);
        }
    }
}
