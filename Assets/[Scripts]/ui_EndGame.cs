using Photon.Pun;
using UnityEngine;
using TMPro;

public class ui_EndGame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI localScoreTxt = null;
    [SerializeField] TextMeshProUGUI otherScoreTxt = null;

    void Start()
    {
        var playerScoresDic = GameManager._instance.playerFoodEaten;

        foreach (var playerScore in playerScoresDic)
        {
            if (playerScore.Key == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                localScoreTxt.SetText($"You score: {playerScore.Value}");
            }
            else
            {
                otherScoreTxt.SetText($"Other player score: {playerScore.Value}");
            }
        }
    }
}
