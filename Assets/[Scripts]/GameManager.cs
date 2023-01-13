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

public class GameManager : MonoBehaviourPunCallbacks
{
    public Action OnPlayerListChange;

    public Action OnGameFinish;

    PhotonView pv;
    
    public bool HasPowerUp = false;
    public float powerUpTimer;

    [SerializeField] Text foodItemTxt = null;
    [SerializeField] int foodItems;

    [SerializeField] TextMeshProUGUI finalScoreTxt = null;
    [SerializeField] GameObject gameOverPopup;
    
    public Dictionary<int, PlayerData> playersData = new Dictionary<int, PlayerData>(2);
    int nextIdPlayerToFollow = -1;

    public ParticleSystem foodParticle;

    public static GameManager _instance;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        Time.timeScale = 1f;
        if (_instance == null) 
            _instance = this;
    }

    void Update()
    {
        // Check timer only when Power up time
        if (HasPowerUp)
        {
            // Countdown the timer with update time
            powerUpTimer -= Time.deltaTime;
            //Debug.Log("PowerUp");

            if (powerUpTimer <= 0)
            {
                // End of power up time 
                HasPowerUp = false;
                powerUpTimer = 0;
                //Debug.Log("PowerDown");
            }
        }
    }

    public void PlayeEatFood(PhotonView playerPv)
    {
        if (PhotonNetwork.IsMasterClient == false) return;
        
        playersData[playerPv.OwnerActorNr].food++;
        SyncPlayerScore(playerPv.Owner, playersData[playerPv.OwnerActorNr].food);
    }
    
    // Add any time player picks to timer
    public void OnPickPowerUp(float buffTime)
    {
        HasPowerUp = true;
        powerUpTimer = buffTime;
    }

    public void PlayerDie(PhotonView playerPv)
    {
        playersData[playerPv.OwnerActorNr].isAlive = false;
        
        // Just debug data
        if (playerPv.AmOwner)
        {
            print("You die");
        }
        else
        {
            print("The other player dies");
        }
        
        // Check if all players are death
        foreach (var pData in playersData)
        {
            if (pData.Value.isAlive)
                return;
        }

        DisplayGameOver();
    }

    public void DisplayGameOver()
    {
        pv.RPC(nameof(DisplayGameOverRpc), RpcTarget.All);
    }

    [PunRPC]
    void DisplayGameOverRpc()
    {
        if (gameOverPopup.activeSelf) return;
        
        gameOverPopup.SetActive(true);
        OnGameFinish?.Invoke();
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

    public void PlayFoodParticle(Vector3 _pos)
    {
        foodParticle.transform.position = _pos;
        foodParticle.Emit(30);
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playersData.ContainsKey(otherPlayer.ActorNumber))
        {
            playersData.Remove(otherPlayer.ActorNumber);
            OnPlayerListChange?.Invoke();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(targetPlayer.IsLocal == false) return;
        
        foodItemTxt.text = "Score: " + changedProps["score"].ToString();
    }

    void SyncPlayerScore(Player player ,int newScore)
    {
        Hashtable score = new Hashtable();
        score["score"] = newScore;
        player.SetCustomProperties(score);
    }

    public int GetLocalScore()
    {
        return GetScoreOfPlayer(PhotonNetwork.LocalPlayer);
    }

    public int GetOtherScore()
    {
        var otherPlayers = PhotonNetwork.PlayerListOthers;
        return otherPlayers.Length == 0 ? 0 : GetScoreOfPlayer(otherPlayers[0]);
    }

    int GetScoreOfPlayer(Player player)
    {
        if (player.CustomProperties.TryGetValue("score", out object score))
        {
            return (int)score;
        }

        return 0;
    }

    public class PlayerData
    {
        public Transform transform;
        public int food;
        public bool isAlive;
    }
}
