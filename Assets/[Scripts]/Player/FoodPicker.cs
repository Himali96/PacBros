using UnityEngine;
using Photon.Pun;

public class FoodPicker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.gameObject.CompareTag("Player"))
        {
            GameManager._instance.Food++;
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
