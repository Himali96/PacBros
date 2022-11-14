using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform playerTransform;

    void Start()
    {
        playerTransform = FindObjectOfType<ThirdPersonMovement>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + new Vector3(0, 10.0f, -5.0f);
        }
    }
}
