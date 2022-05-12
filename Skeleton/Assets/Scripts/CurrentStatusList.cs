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
        purchaseButton.onClick.AddListener(() => FinalizeDay());
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
            if (gm.gameData.OwnedContracts.Count() > 0)
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
    }

    // Removes food spent from gameData
    public void SubtractCurrentlyUsedFood()
    {
        foreach (var gameObj in _listedOwnedFood)
        {
            var script = gameObj.GetComponent<OwnedFoodSelection>();
            int amountToGetRidOf = (int)script.slider.value;
            for (int i = 0; i < gm.gameData.OwnedFood.Count(); i++)
            {
                if (gm.gameData.OwnedFood[i].foodItem.name == script.currentFood.foodItem.name)
                {
                    if (amountToGetRidOf >= gm.gameData.OwnedFood[i].unitsLeft)
                    {
                        amountToGetRidOf -= gm.gameData.OwnedFood[i].unitsLeft;
                        gm.gameData.OwnedFood.RemoveAt(i);
                        i--;
                    } else
                    {
                        gm.gameData.OwnedFood[i].unitsLeft -= amountToGetRidOf;
                    }
                }
            }
        }
    }

    public void FinalizeDay()
    {
        SubtractCurrentlyUsedFood();
        gm.AdvanceDay(nutrients);
        SceneManager.LoadScene(1);
    }
}
