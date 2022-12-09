using UnityEngine;
using TMPro;

public class ui_EndGame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI localScoreTxt = null;
    [SerializeField] TextMeshProUGUI otherScoreTxt = null;

    void Start()
    {
        localScoreTxt.SetText($"You score: {GameManager._instance.GetLocalScore()}");
        otherScoreTxt.SetText($"Other player score: {GameManager._instance.GetOtherScore()}");
    }
}
