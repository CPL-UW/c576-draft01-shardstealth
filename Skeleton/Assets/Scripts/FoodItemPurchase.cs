using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodItemPurchase : MonoBehaviour
{
    public TextMeshProUGUI foodNameTxt;
    public TextMeshProUGUI maxPurchaseTxt;
    public TextMeshProUGUI minPurchaseTxt;
    public TextMeshProUGUI curAmountTxt;
    public TextMeshProUGUI priceTxt;
    public Toggle inCartToggle;
    public Slider slider;
    public DailyFoodItem currentFood;

    public void setStartingData(DailyFoodItem dailyFood)
    {
        currentFood = dailyFood;
        foodNameTxt.text += dailyFood.foodItem.name;
        minPurchaseTxt.text = dailyFood.minPurchase.ToString();
        maxPurchaseTxt.text += dailyFood.maxPurchase.ToString();
        priceTxt.text += dailyFood.foodItem.price.ToString();
        slider.minValue = dailyFood.minPurchase;
        slider.maxValue = dailyFood.maxPurchase;
    }

    public void setPurchaseQuantityText(float value)
    {
        curAmountTxt.text = value.ToString();
    }
}
