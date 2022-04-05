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

public struct WeeklyFoodItem
{
    public FoodItem foodItem;
    public int minPurchase;
    public int maxPurchase;
}

[Serializable]
public struct OwnedFoodItem
{
    public FoodItem foodItem;
    public int unitsLeft;
}

[Serializable]
public struct Contract
{
    public int people;
    public int weeks;
    public int payment;
}

[Serializable]
public struct GameData
{
    public float money;
    public List<OwnedFoodItem> OwnedFood;
    public List<Contract> OwnedContracts;
    public WeeklyFoodItem[] weeklyFood;
    public Contract[] weeklyContracts;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager get()
    {
        return instance;
    }
    public TextAsset foodDataJson;
    public GameData gameData;
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
        gameData.OwnedFood = new List<OwnedFoodItem>();
        gameData.OwnedContracts = new List<Contract>();
        gameData.money = 3000;
        random = new System.Random();
        AdvanceWeek();
    }

    public void OnStoreClick(string str)
    {
        Debug.Log(str);
    }

    public void AdvanceWeek()
    {
        gameData.weeklyFood = new WeeklyFoodItem[4];
        for (int i = 0; i < 4; i++)
        {
            gameData.weeklyFood[i] = new WeeklyFoodItem();
            var randType = foodData.FoodTypes[random.Next(foodData.FoodTypes.Length)];

            gameData.weeklyFood[i].foodItem = randType.items[random.Next(randType.items.Length)];
            gameData.weeklyFood[i].minPurchase = random.Next(100, 500);
            gameData.weeklyFood[i].maxPurchase = random.Next(gameData.weeklyFood[i].minPurchase, 700);
        }

        gameData.weeklyContracts = new Contract[10];
        for (int i = 0; i < 10; i++)
        {
            gameData.weeklyContracts[i] = new Contract();
            gameData.weeklyContracts[i].people = random.Next(3, 20);
            gameData.weeklyContracts[i].weeks = random.Next(1, 10);
            gameData.weeklyContracts[i].payment = random.Next(3,6) * gameData.weeklyContracts[i].people * gameData.weeklyContracts[i].weeks * 7;
        }
    }

    public WeeklyFoodItem[] GetWeeklyFoodItems()
    {
        return gameData.weeklyFood;
    }

    public Contract[] GetWeeklyContracts()
    {
        return gameData.weeklyContracts;
    }

    public void PurchaseGoods(List<OwnedFoodItem> foodItems, float price)
    {
        //TODO Bug of not removing items from weekly
        gameData.OwnedFood.AddRange(foodItems);
        gameData.money -= price;
        Hud.get().getUpdatedGameState();
    }

    public void AquireContract(Contract contract)
    {
        //TODO Bug of not removing contract from weekly
        gameData.OwnedContracts.Add(contract);
        gameData.money += contract.payment;
        Hud.get().getUpdatedGameState();
    }
}
