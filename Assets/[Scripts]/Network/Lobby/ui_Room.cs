using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ui_Room : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText = null;

    public bool Updated { get; set; }

    public void SetRoomName(string _name)
    {
        roomNameText.text = _name;
    }

    public void BtnJoin()
    {
        GameObject.FindObjectOfType<LobbyManager>().JoinRoom(roomNameText.text);
    }
}
