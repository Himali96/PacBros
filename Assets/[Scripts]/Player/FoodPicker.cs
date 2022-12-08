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
        if(GameManager._instance)
            GameManager._instance.DisplayGameOver();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView playerPv = other.GetComponent<PhotonView>();

            // Increase +1 to score if is local player
            if(playerPv.AmOwner)
                GameManager._instance.Food++;

            // Send score of that player
            GameManager._instance.PlayeEatFood(playerPv);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
