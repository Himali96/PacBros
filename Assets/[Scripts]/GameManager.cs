using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI foodItemTxt = null;

    [SerializeField] int foodItems;

    public static GameManager _instance;

    private void Awake()
    {
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
}
