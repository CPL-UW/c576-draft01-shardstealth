using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodItemPurchase : MonoBehaviour
{
    public TextMeshProUGUI foodName;
    public TextMeshProUGUI foodQuantity;
    public TextMeshProUGUI purchaseQuantityText;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setStartingData(string name, int minValue, int maxValue)
    {
        foodName.text += name;
        foodQuantity.text += maxValue.ToString();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        purchaseQuantityText.text = minValue.ToString();
    }

    public void setPurchaseQuantityText(float value)
    {
        purchaseQuantityText.text = value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
