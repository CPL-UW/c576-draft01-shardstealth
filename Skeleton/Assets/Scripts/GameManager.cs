using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

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
    public int day;
    public float money;
    public float rating;
    public List<OwnedFoodItem> OwnedFood;
    public List<Contract> OwnedContracts;
    public WeeklyFoodItem[] weeklyFood;
    public Contract[] weeklyContracts;
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
        // PreProcess foodData by setting type int
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
        gameData.money = 30000;
        gameData.rating = 5;
        random = new System.Random();
        gameData.AINutrients = new float[foodData.FoodTypes.Length];
        gameData.purchasedForDay = false;
    }

    void Start()
    {
        AdvanceDay();
    }

    public void OnStoreClick(string str)
    {
        Debug.Log(str);
    }

    public void AdvanceDay(float[] curNutrients = null)
    {
        Analytics.ReportNewDay("testUser", gameData);
        gameData.day++;
        gameData.purchasedForDay = false;
        gameData.weeklyFood = new WeeklyFoodItem[8];
        for (int i = 0; i < 8; i++)
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

        // Advance day by updating star rating and contracts
        //TODO: Remove used food from inventory
        if (curNutrients != null && gameData.OwnedContracts.Count() > 0)
        {
            int totalMeals = gameData.OwnedContracts.Sum(x => x.people);
            float starDelta = 4;
            for (int i = 0; i < curNutrients.Length; i++)
            {
                starDelta -= System.Math.Abs(idealNutrients[i] - (curNutrients[i] / totalMeals));
            }
            gameData.rating += starDelta;
            gameData.rating = System.Math.Max(gameData.rating, 0);
            for (int i = 0; i < gameData.OwnedContracts.Count; i++)
            {
                var tempStruct = gameData.OwnedContracts[i];
                tempStruct.weeks--;
                gameData.OwnedContracts[i] = tempStruct;
            }
            gameData.OwnedContracts.RemoveAll(x => x.weeks == 0);
        }
        Hud.get().getUpdatedGameState();
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

    void OnApplicationQuit()
    {
        Analytics.Close();
    }
}
