using System;
using UnityEngine;
using Photon.Pun;

public class FoodPicker : MonoBehaviour
{
    static int foodRemaining;

    void Start()
    {
        foodRemaining++;
    }

    void OnDestroy()
    {
        foodRemaining--;
        if(foodRemaining == 0 && GameManager._instance)
            GameManager._instance.DisplayGameOver();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView playerPv = other.GetComponent<PhotonView>();
            GameManager._instance.PlayFoodParticle(transform.position);

            // Send score of that player
            GameManager._instance.PlayeEatFood(playerPv);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
