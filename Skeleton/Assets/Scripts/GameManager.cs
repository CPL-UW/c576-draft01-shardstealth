using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct FoodData
{
    public FoodType[] FoodTypes;
}

[Serializable]
public struct FoodType
{
    public string name;
    public RecommendedDietInfo recommended;
    public FoodItem[] items;
}

[Serializable]
public struct FoodItem
{
    public string name;
    public float price;
    public string units;
    public float nutritionPerUnit;
}

[Serializable]
public struct RecommendedDietInfo
{
    public string unit;
    public int[] women;
    public int[] men;
}

public struct DailyFoodItem
{
    public FoodItem foodItem;
    public int minPurchase;
    public int maxPurchase;
}

public class GameManager : MonoBehaviour
{
    [Serializable]
    struct GameData
    {
        public float money;
        public DailyFoodItem[] dailyFood;
    }

    public static GameManager instance;
    public static GameManager get()
    {
        return instance;
    }
    public TextAsset foodDataJson;
    GameData gameData;
    FoodData foodData;
    
    System.Random random;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foodData = UnityEngine.JsonUtility.FromJson<FoodData>(foodDataJson.text);
        gameData = new GameData();
        random = new System.Random();
        AdvanceDay();
        Debug.Log(foodData.FoodTypes[0].items.Length);
    }

    public void OnStoreClick(string str)
    {
        Debug.Log(str);
    }

    public void AdvanceDay()
    {
        gameData.dailyFood = new DailyFoodItem[2];
        for (int i = 0; i < 2; i++)
        {
            gameData.dailyFood[i] = new DailyFoodItem();
            var randType = foodData.FoodTypes[random.Next(foodData.FoodTypes.Length)];

            gameData.dailyFood[i].foodItem = randType.items[random.Next(randType.items.Length)];
            gameData.dailyFood[i].minPurchase = random.Next(100, 500);
            gameData.dailyFood[i].maxPurchase = random.Next(gameData.dailyFood[i].minPurchase, 700);
        }
    }

    public DailyFoodItem[] GetDailyFoodItems()
    {
        return gameData.dailyFood;
    }
}
