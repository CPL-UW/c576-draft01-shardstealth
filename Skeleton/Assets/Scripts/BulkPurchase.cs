using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public void PurchaseCartGoods()
    {
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
                owned.unitsLeft = (int)script.slider.value;
                purchased.Add(owned);

                script.slider.minValue = 0;
                script.minPurchaseTxt.text = "Min Order: 0";
                script.slider.value = 0;
                script.slider.maxValue -= owned.unitsLeft;
                script.maxPurchaseTxt.text = "Max Order: " + owned.unitsLeft;

                if (script.slider.maxValue == 0)
                    gameObj.SetActive(false);
            }
        }

        gm.PurchaseGoods(purchased, tempCart);

        RecalculateCart();
    }
}
