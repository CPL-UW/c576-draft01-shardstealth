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
    public WeeklyFoodItem currentFood;

    public void setStartingData(WeeklyFoodItem weeklyFood)
    {
        currentFood = weeklyFood;
        foodNameTxt.text += weeklyFood.foodItem.name;
        minPurchaseTxt.text = weeklyFood.minPurchase.ToString();
        maxPurchaseTxt.text += weeklyFood.maxPurchase.ToString();
        priceTxt.text += weeklyFood.foodItem.price.ToString();
        slider.minValue = weeklyFood.minPurchase;
        slider.maxValue = weeklyFood.maxPurchase;
    }

    public void setPurchaseQuantityText(float value)
    {
        curAmountTxt.text = value.ToString();
    }
}
