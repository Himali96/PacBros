using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject panelGo = null;

    [Header("Create room")] 
    [SerializeField] TMP_InputField roomCreateInput = null;
    [SerializeField] Button roomCreateBtn = null;

    [Header("Room list")]
    [SerializeField] GameObject roomPrefab = null;
    [SerializeField] RectTransform roomsPanel = null;

    [Header("Waiting room")] 
    [SerializeField] GameObject waitingRoomGo = null;
    [SerializeField] string gameScene = "";

    [Header("Join Fast")] 
    [SerializeField] Button jointFastBtn = null;

    int numPlayers = 0;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.GameVersion = "0.1";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
            //OnJoinedLobby();
        }
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedLobby()
    {
        print("Joined Lobby Server");
        if (!PhotonNetwork.InRoom)
        {
            panelGo.SetActive(true);
        }
    }

    public void CreateRoom()
    {
        if (roomCreateInput.text.Length == 0)
        {
            print("Room name empty");
            return;
        }

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };

        if (PhotonNetwork.CreateRoom(roomCreateInput.text, roomOptions, TypedLobby.Default))
        {
            waitingRoomGo.SetActive(true);
            roomCreateBtn.interactable = false;
            jointFastBtn.interactable = false;
        }
        else
        {
            print("Create room failed");
        }
    }

    public void JoinRoom(string _roomName)
    {
        if (PhotonNetwork.JoinRoom(_roomName))
        {
            Debug.Log("Player Joined in the Room");
            waitingRoomGo.SetActive(true);
        }
        else
        {
            Debug.Log("Failed to join in the room, please fix the error!");
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        waitingRoomGo.SetActive(false);
        roomCreateBtn.interactable = true;
        jointFastBtn.interactable = true;
    }

    public void PlayAlone()
    {
        //PhotonNetwork.OfflineMode = true;
        PhotonNetwork.LoadLevel(gameScene);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        CreateRoom();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            PhotonNetwork.NickName = $"PacBro-{Random.Range(1, 100000)}";   
        }
        roomCreateInput.text = $"Room-{PhotonNetwork.NickName}";

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        print($"New player enter in room: {newPlayer.NickName}");
        numPlayers++;
        if (numPlayers >= 2)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(gameScene);
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("Room Created Successfully");
        numPlayers = 1;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("Create Room Failed: " + returnCode + " - " + message);
        roomCreateBtn.interactable = true;
        jointFastBtn.interactable = true;
        waitingRoomGo.SetActive(false);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("Room list updated");

        // Clean
        foreach (Transform t in roomsPanel)
        {
            Destroy(t.gameObject);
        }

        // Creat news
        foreach (RoomInfo room in roomList)
        {
            RoomReceived(room);
        }
    }

    private void RoomReceived(RoomInfo room)
    {
        if (room.IsVisible && room.PlayerCount < room.MaxPlayers) // Visisble and not full
        {
            GameObject newRoomGo = Instantiate(roomPrefab, roomsPanel);

            ui_Room uiRoom = newRoomGo.GetComponent<ui_Room>();
            uiRoom.SetRoomName(room.Name);
        }
    }
}
