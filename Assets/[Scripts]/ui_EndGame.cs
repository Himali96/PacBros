using Photon.Pun;
using UnityEngine;
using TMPro;

public class ui_EndGame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI localScoreTxt = null;
    [SerializeField] TextMeshProUGUI otherScoreTxt = null;

    void Start()
    {
        var playersDataDic = GameManager._instance.playersData;

        foreach (var pData in playersDataDic)
        {
            if (pData.Key == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                localScoreTxt.SetText($"You score: {pData.Value.food}");
            }
            else
            {
                otherScoreTxt.SetText($"Other player score: {pData.Value.food}");
            }
        }
    }
}
