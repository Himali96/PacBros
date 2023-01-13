using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Menu : MonoBehaviourPunCallbacks
{

    public GameObject creditPanel;

    public void Start()
    {
        creditPanel.SetActive(false);
    }

    public void OnCreditClick()
    {
        creditPanel.SetActive(true);
    }

    public void OnCreditExitClick()
    {
        creditPanel.SetActive(false);
    }

    public void playGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }
}
