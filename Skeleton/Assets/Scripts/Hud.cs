using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hud : MonoBehaviour
{
    public TextMeshProUGUI moneyTxt;
    public TextMeshProUGUI contractsTxt;
    public TextMeshProUGUI dayTxt;
    public TextMeshProUGUI ratingTxt;

    public static Hud instance;
    public static Hud get()
    {
        return instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        getUpdatedGameState();
    }

    public void getUpdatedGameState()
    {
        var gameData = GameManager.get().gameData;
        moneyTxt.text = "$" + gameData.money;
        contractsTxt.text = "Contracts: " + gameData.OwnedContracts.Count;
        dayTxt.text = "Day: " + gameData.day;
        ratingTxt.text = "Rating: " + System.Math.Round(gameData.rating, 2).ToString();
    }
}
