using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class CurrentStatusList : MonoBehaviour
{
    public GameObject template;
    public UnityEngine.UI.Button purchaseButton;
    public TextMeshProUGUI nutrientTxt;
    public TextMeshProUGUI servingsTxt;
    public Toggle perMealToggle;
    private List<GameObject> _listedOwnedFood;
    private float[] nutrients;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        nutrients = new float[4];
        gm = GameManager.get();
        _listedOwnedFood = new List<GameObject>();
        purchaseButton.onClick.AddListener(() => FinalizeWeek());
        perMealToggle.onValueChanged.AddListener((_) => RecalculateNutrients());
        servingsTxt.text += gm.gameData.OwnedContracts.Sum(x => x.people).ToString();


        var allOwned = gm.gameData.OwnedFood;
        foreach (var owned in allOwned)
        {
            GameObject thing = Instantiate(template) as GameObject;
            thing.SetActive(true);
            thing.transform.SetParent(template.transform.parent, false);
            thing.GetComponent<OwnedFoodSelection>().setStartingData(owned);
            thing.GetComponent<OwnedFoodSelection>().slider.onValueChanged.AddListener((_) => RecalculateNutrients());
            _listedOwnedFood.Add(thing);
        }
        RecalculateNutrients();
    } 

    public void RecalculateNutrients()
    {
        // TODO: Show Nutrients
        nutrients = new float[4];

        foreach (var gameObj in _listedOwnedFood)
        {
            var script = gameObj.GetComponent<OwnedFoodSelection>();
            var foodItem = script.currentFood.foodItem;
            nutrients[foodItem.type] += foodItem.nutritionPerUnit * script.slider.value;
        }

        int numberMeals = 1;
        if (perMealToggle.isOn)
        {
            numberMeals = gm.gameData.OwnedContracts.Sum(x => x.people);
        }
        string nutrientsString = "Nutrients:\n";
        for (int i = 0; i < 4; i++)
        {
            nutrientsString += gm.foodData.FoodTypes[i].name;
            nutrientsString += ": ";
            nutrientsString += System.Math.Round(nutrients[i] / numberMeals, 2).ToString();
            nutrientsString += gm.foodData.FoodTypes[i].recommended.unit + "\n";
        }

        nutrientTxt.text = nutrientsString;
        //cartPriceTxt.text = "Purchase Price: $" + _cartPrice;
    }

    public void FinalizeWeek()
    {
        // TODO: Advance Week
        gm.AdvanceDay(nutrients);
        SceneManager.LoadScene(1);
        //var tempCart = _cartPrice;
        //if (gm.gameData.money < tempCart)
        //    return;

        //var purchased = new List<OwnedFoodItem>();

        //foreach (var gameObj in _purchaseableFood)
        //{
        //    var script = gameObj.GetComponent<FoodItemPurchase>();
        //    if (script.inCartToggle.isOn)
        //    {
        //        var owned = new OwnedFoodItem();
        //        owned.foodItem = script.currentFood.foodItem;
        //        owned.unitsLeft = (int)script.slider.value;
        //        purchased.Add(owned);

        //        script.slider.minValue = 0;
        //        script.minPurchaseTxt.text = "Min Order: 0";
        //        script.slider.value = 0;
        //        script.slider.maxValue -= owned.unitsLeft;
        //        script.maxPurchaseTxt.text = "Max Order: " + owned.unitsLeft;

        //        if (script.slider.maxValue == 0)
        //            gameObj.SetActive(false);
        //    }
        //}

        //gm.PurchaseGoods(purchased, tempCart);

        //RecalculateCart();
    }
}
