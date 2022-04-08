using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OwnedFoodSelection : MonoBehaviour
{
    public TextMeshProUGUI foodNameTxt;
    public TextMeshProUGUI quantityOwned;
    public TextMeshProUGUI curAmountTxt;
    public TextMeshProUGUI priceTxt;
    public TextMeshProUGUI nutrPerUnitTxt;
    public Slider slider;
    public OwnedFoodItem currentFood;

    public void setStartingData(OwnedFoodItem ownedFoodItem)
    {
        currentFood = ownedFoodItem;
        foodNameTxt.text += ownedFoodItem.foodItem.name;
        quantityOwned.text += ownedFoodItem.unitsLeft.ToString();
        priceTxt.text += ownedFoodItem.foodItem.price.ToString();
        nutrPerUnitTxt.text += ownedFoodItem.foodItem.nutritionPerUnit.ToString();
        nutrPerUnitTxt.text += GameManager.get().foodData.FoodTypes[ownedFoodItem.foodItem.type].recommended.unit;
        slider.minValue = 0;
        slider.maxValue = ownedFoodItem.unitsLeft;
    }

    public void setPurchaseQuantityText(float value)
    {
        curAmountTxt.text = value.ToString();
    }
}
