using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

// A bunch of types used to interpret json or used elsewhere for data transfer purposes
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
    public int type;
}

[Serializable]
public struct RecommendedDietInfo
{
    public string unit;
    public int[] women;
    public float men;
}

public struct DailyFoodItem
{
    public FoodItem foodItem;
    public int minPurchase;
    public int maxPurchase;
}

[Serializable]
public class OwnedFoodItem
{
    public FoodItem foodItem;
    public int unitsLeft;
}

[Serializable]
public struct Contract
{
    public int people;
    public int days;
    public int payment;
    public int id;
    public bool enabled;
}

// Holds the only state of the program
[Serializable]
public struct GameData
{
    public int day;
    public float money;
    public float rating;
    public List<OwnedFoodItem> OwnedFood;
    public List<Contract> OwnedContracts;
    public DailyFoodItem[] dailyFood;
    public Contract[] dailyContracts;
    public float[] AINutrients;
    public bool purchasedForDay;
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
    public FoodData foodData;
    public float[] idealNutrients;

    System.Random random;
 
    void Awake()
    {
        // Maintain one instance of the gamemanager
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foodData = UnityEngine.JsonUtility.FromJson<FoodData>(foodDataJson.text);
        // PreProcess foodData by setting type int
        // In future should be an enum
        for (int i = 0; i < foodData.FoodTypes.Length; i++)
        {
            for (int j = 0; j < foodData.FoodTypes[i].items.Length; j++)
            {
                foodData.FoodTypes[i].items[j].type = i;
            }
        }

        idealNutrients = new float[foodData.FoodTypes.Length];
        for (int i = 0; i < foodData.FoodTypes.Length; i++)
        {
            idealNutrients[i] = foodData.FoodTypes[i].recommended.men;
        }

        gameData = new GameData();
        gameData.OwnedFood = new List<OwnedFoodItem>();
        gameData.OwnedContracts = new List<Contract>();
        gameData.money = 50;
        gameData.rating = 5;
        random = new System.Random();
        gameData.AINutrients = new float[foodData.FoodTypes.Length];
        gameData.purchasedForDay = false;
    }

    void Start()
    {
        AdvanceDay();
    }

    public void AdvanceDay(float[] curNutrients = null)
    {
        Analytics.ReportNewDay("testUser", gameData);
        gameData.day++;
        gameData.purchasedForDay = false;
        int rating = (int)gameData.rating;
        int numDailyFoodItems = (int)(gameData.rating / 2);
        gameData.dailyFood = new DailyFoodItem[numDailyFoodItems];
        for (int i = 0; i < numDailyFoodItems; i++)
        {
            gameData.dailyFood[i] = new DailyFoodItem();
            var randType = foodData.FoodTypes[random.Next(foodData.FoodTypes.Length)];

            gameData.dailyFood[i].foodItem = randType.items[random.Next(randType.items.Length)];
            gameData.dailyFood[i].minPurchase = random.Next(1, Math.Max(rating,1));
            gameData.dailyFood[i].maxPurchase = random.Next(gameData.dailyFood[i].minPurchase, (rating + 2) * 2);
        }

        gameData.dailyContracts = new Contract[10];
        int numdailyContracts = Math.Max((int)(gameData.rating / 5), 1);
        for (int i = 0; i < numdailyContracts; i++)
        {
            gameData.dailyContracts[i] = new Contract();
            gameData.dailyContracts[i].people = random.Next(3, Math.Max((int)(gameData.rating / 2), 3));
            gameData.dailyContracts[i].days = random.Next(2, Math.Max((int)(gameData.rating) * 7, 2));
            gameData.dailyContracts[i].payment = random.Next(3,6) * gameData.dailyContracts[i].people * gameData.dailyContracts[i].days;
            gameData.dailyContracts[i].id = i;
            gameData.dailyContracts[i].enabled = true;
        }

        // Advance day by updating star rating and contracts
        // Food Removal is processed by CurrentStatusList
        if (curNutrients != null && gameData.OwnedContracts.Count() > 0)
        {
            int totalMeals = gameData.OwnedContracts.Sum(x => x.people);
            float starDelta = 4;
            for (int i = 0; i < curNutrients.Length; i++)
            {
                starDelta -= System.Math.Abs(idealNutrients[i] - (curNutrients[i] / totalMeals));
            }
            gameData.rating += starDelta * (totalMeals/3);
            gameData.rating = System.Math.Max(gameData.rating, 0);
            for (int i = 0; i < gameData.OwnedContracts.Count; i++)
            {
                var tempStruct = gameData.OwnedContracts[i];
                tempStruct.days--;
                gameData.OwnedContracts[i] = tempStruct;
            }
            gameData.OwnedContracts.RemoveAll(x => x.days == 0);
        }
        Hud.get().getUpdatedGameState();
    }

    public DailyFoodItem[] GetDailyFoodItems()
    {
        return gameData.dailyFood;
    }

    public Contract[] GetDailyContracts()
    {
        return gameData.dailyContracts;
    }

    public void PurchaseGoods(List<OwnedFoodItem> foodItems, float price)
    {
        gameData.OwnedFood.AddRange(foodItems);
        gameData.money -= price;
        Hud.get().getUpdatedGameState();
    }

    public void AquireContract(Contract contract)
    {
        gameData.OwnedContracts.Add(contract);
        gameData.money += contract.payment;
        gameData.dailyContracts[contract.id].enabled = false;
        Hud.get().getUpdatedGameState();
    }

    void OnApplicationQuit()
    {
        Analytics.Close();
    }
}
