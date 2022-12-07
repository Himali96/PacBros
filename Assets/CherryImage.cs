using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryImage : MonoBehaviour
{
    public GameObject _gameObject;

    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
      if(other.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            _gameObject.SetActive(true);
        }
    }
}
