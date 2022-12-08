using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IInRoomCallbacks
{
    public Action OnPlayerListChange;

    [SerializeField] Text foodItemTxt = null;
    [SerializeField] int foodItems;

    [SerializeField] TextMeshProUGUI finalScoreTxt = null;
    [SerializeField] GameObject gameOverPopup;
    
    public Dictionary<int, PlayerData> playersData = new Dictionary<int, PlayerData>(2);
    int nextIdPlayerToFollow = -1;

    public static GameManager _instance;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (_instance == null) 
            _instance = this;
    }

    public int Food // This is local player score
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
            /*
            if (foodItems >= 150) //only for 1 player
            {
                gameOverPopup.SetActive(true);
                finalScoreTxt.text = "Your Final Score: " + foodItems.ToString();
            }
            */
            foodItemTxt.text = "Score: " + foodItems.ToString();
        }
    }

    public void PlayeEatFood(PhotonView playerPv)
    {
        playersData[playerPv.OwnerActorNr].food++;
    }

    public void PlayerDie(PhotonView playerPv)
    {
        playersData[playerPv.OwnerActorNr].isAlive = false;
        
        // Just debug data
        if (playerPv.AmOwner)
        {
            print("Other player wins");
        }
        else
        {
            print("Local player wins");
        }
        
        // Check if all players are death
        foreach (var pData in playersData)
        {
            if (pData.Value.isAlive)
                return;
        }

        // After this point, all players are death ---

        Time.timeScale = 0f;
        gameOverPopup.SetActive(true);
    }

    public Transform GetPlayerToFollow()
    {
        if (playersData.Count == 0)
            return null;

        nextIdPlayerToFollow++;
        if (nextIdPlayerToFollow >= playersData.Count)
            nextIdPlayerToFollow = 0;

        return playersData.ElementAt(nextIdPlayerToFollow).Value.transform;
    }

    public void NewPlayerConnected(PhotonView pv)
    {
        PlayerData pData = new PlayerData
        {
            transform = pv.transform,
            food = 0,
            isAlive = true
        };
        playersData.Add(pv.OwnerActorNr, pData);
        OnPlayerListChange?.Invoke();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playersData.ContainsKey(otherPlayer.ActorNumber))
        {
            playersData.Remove(otherPlayer.ActorNumber);
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
    
    /*void SyncLocalScore(int newScore)
    {
        Hashtable score = new Hashtable();
        score["score"] = newScore;
        PhotonNetwork.LocalPlayer.SetCustomProperties(score);
    }
    
    int GetScoreOfPlayer(Player player)
    {
        if (player.CustomProperties.TryGetValue("score", out object score))
        {
            return (int)score;
        }

        return 0;
    }*/

    public class PlayerData
    {
        public Transform transform;
        public int food;
        public bool isAlive;
    }
    
}
