using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPicker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager._instance.Food++;
            Destroy(gameObject);
        }
    }
}
