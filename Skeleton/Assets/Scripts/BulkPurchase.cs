using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class BulkPurchase : MonoBehaviour
{
    public GameObject template;
    public TextMeshProUGUI cartPriceTxt;
    public UnityEngine.UI.Button purchaseButton;
    private List<GameObject> _purchaseableFood;
    private float _cartPrice = 0;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.get();
        purchaseButton.onClick.AddListener(() => PurchaseCartGoods());
        purchaseButton.interactable = !gm.gameData.purchasedForDay; // Disable buy button if users already purchased for day
        _purchaseableFood = new List<GameObject>();
        var weekly = gm.GetWeeklyFoodItems();
        foreach (var foodItem in weekly)
        {
            GameObject thing = Instantiate(template) as GameObject;
            thing.SetActive(true);
            thing.transform.SetParent(template.transform.parent, false);
            thing.GetComponent<FoodItemPurchase>().setStartingData(foodItem);
            thing.GetComponent<FoodItemPurchase>().inCartToggle.onValueChanged.AddListener((_) => RecalculateCart());
            thing.GetComponent<FoodItemPurchase>().slider.onValueChanged.AddListener((_) => RecalculateCart());
            _purchaseableFood.Add(thing);
        }
        RecalculateCart();
    }

    public void RecalculateCart()
    {
        _cartPrice = 0;

        foreach (var gameObj in _purchaseableFood) {
            var script = gameObj.GetComponent<FoodItemPurchase>();
            if (script.inCartToggle.isOn)
                _cartPrice += script.slider.value * script.currentFood.foodItem.price;
        }

        cartPriceTxt.text = "Purchase Price: $" + _cartPrice;
    }

    private void ExecutePurchaseAI()
    {
        int mealsCompletedByAI = 1000;
        int mealsPossibleToComplete = gm.gameData.OwnedContracts.Sum(x => x.people);
        // For each nutrient type
        for (int i = 0; i < 4; i++)
        {
            var nutrVal = gm.gameData.AINutrients[i];
            var nutrTarget =  mealsPossibleToComplete * gm.idealNutrients[i];

            var sorted = gm.gameData.weeklyFood.Where(weekFood => weekFood.foodItem.type == i).OrderBy(weekFood => weekFood.foodItem.price).ToArray();

            for (int j = 0; j < sorted.Length; j++)
            {
                if (nutrTarget - nutrVal <= 0)
                    break;
                var maxAqui = (nutrTarget - nutrVal) / sorted[j].foodItem.nutritionPerUnit;
                var aquiAmount = Math.Max((int)Math.Ceiling(maxAqui), sorted[j].maxPurchase);
                nutrVal += aquiAmount * sorted[j].foodItem.nutritionPerUnit;
                sorted[j].maxPurchase -= aquiAmount;
            }
            gm.gameData.AINutrients[i] = nutrVal;

            mealsCompletedByAI = Math.Min(mealsCompletedByAI, (int)Math.Floor(nutrVal / gm.idealNutrients[i]));
            mealsCompletedByAI = Math.Min(mealsCompletedByAI, mealsPossibleToComplete);
        }

        for (int i = 0; i < 4; i++)
        {
            gm.gameData.AINutrients[i] -= mealsCompletedByAI * gm.idealNutrients[i];
        }
    }

    public void PurchaseCartGoods()
    {
        ExecutePurchaseAI();

        var tempCart = _cartPrice;
        if (gm.gameData.money < tempCart)
            return;

        var purchased = new List<OwnedFoodItem>();

        foreach (var gameObj in _purchaseableFood)
        {
            var script = gameObj.GetComponent<FoodItemPurchase>();
            if (script.inCartToggle.isOn)
            {
                var owned = new OwnedFoodItem();
                owned.foodItem = script.currentFood.foodItem;
                int sliderVal = (int)script.slider.value;
                owned.unitsLeft = Math.Min(sliderVal, script.currentFood.maxPurchase);

                // If the food ran out, split the remaining with the ai
                if (owned.unitsLeft < sliderVal)
                    owned.unitsLeft += (sliderVal - owned.unitsLeft) / 2;
                purchased.Add(owned);

                script.slider.minValue = 0;
                script.minPurchaseTxt.text = "Min Order: 0";
                script.slider.value = 0;
                script.slider.maxValue = Math.Max(0, script.currentFood.maxPurchase - owned.unitsLeft);
                script.maxPurchaseTxt.text = "Max Order: " + owned.unitsLeft;

                if (script.slider.maxValue == 0)
                    gameObj.SetActive(false);
            }
        }

        gm.PurchaseGoods(purchased, tempCart);

        RecalculateCart();
        gm.gameData.purchasedForDay = true;
        purchaseButton.interactable = !gm.gameData.purchasedForDay; // Disable buy button if users already purchased for day

    }
}
