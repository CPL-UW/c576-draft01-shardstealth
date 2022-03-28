using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class FoodData
{
    public FoodType[] FoodTypes;
}

[Serializable]
public class FoodType
{
    public string name;
    public RecommendedDietInfo recommended;
    public FoodItem[] items;
}

[Serializable]
public class FoodItem
{
    public string name;
    public float price;
    public string units;
    public float nutritionPerUnit;
}

[Serializable]
public class RecommendedDietInfo
{
    public string unit;
    public int[] women;
    public int[] men;
}

public class DailyFoodItem
{
    FoodItem foodItem;
    int minPurchase;
    int maxPurchase;
}

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public static GameManager get()
    {
        return instance;
    }
    public TextAsset foodDataJson;
    FoodData foodData;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        foodData = UnityEngine.JsonUtility.FromJson<FoodData>(foodDataJson.text);
        Debug.Log(foodData.FoodTypes[0].items.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStoreClick(string str)
    {
        Debug.Log(str);
    }

    //public DailyFoodItem[] GetDailyFoodItems()
    //{

    //}
}
