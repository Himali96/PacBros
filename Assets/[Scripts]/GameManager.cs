using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour, IInRoomCallbacks
{
    public Action OnPlayerListChange;

    [SerializeField] TextMeshProUGUI foodItemTxt = null;

    [SerializeField] int foodItems;

    [System.NonSerialized] public Dictionary<int, Transform> playersTransform = new Dictionary<int, Transform>();
    int nextIdPlayerToFollow = -1;

    public static GameManager _instance;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (_instance == null) 
            _instance = this;
    }

    public int Food
    {
        get
        {
            return foodItems;
        }

        set
        {
            foodItems = value;
            if (foodItems < 0)
                foodItems = 0;
            foodItemTxt.SetText("Score: " +foodItems.ToString());
        }
    }

    public void PlayerDie(PhotonView playerPv)
    {
        if (playerPv.AmOwner)
        {
            print("Other player wins");
        }
        else
        {
            print("Local player wins");
        }

        Time.timeScale = 0f;
    }

    public Transform GetPlayerToFollow()
    {
        if (playersTransform.Count == 0)
            return null;

        nextIdPlayerToFollow++;
        if (nextIdPlayerToFollow >= playersTransform.Count)
            nextIdPlayerToFollow = 0;

        return playersTransform.ElementAt(nextIdPlayerToFollow).Value;
    }

    public void NewPlayerConnected(PhotonView pv)
    {
        playersTransform.Add(pv.OwnerActorNr, pv.transform);
        OnPlayerListChange?.Invoke();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playersTransform.ContainsKey(otherPlayer.ActorNumber))
        {
            playersTransform.Remove(otherPlayer.ActorNumber);
            OnPlayerListChange?.Invoke();
        }
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
}
