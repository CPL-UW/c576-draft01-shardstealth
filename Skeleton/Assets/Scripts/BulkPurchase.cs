using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulkPurchase : MonoBehaviour
{
    public GameObject template;

    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.get();
        var weekly = gm.GetWeeklyFoodItems();
        foreach (var foodItem in weekly)
        {
            GameObject thing = Instantiate(template) as GameObject;
            thing.SetActive(true);
            thing.transform.SetParent(template.transform.parent, false);
            thing.GetComponent<FoodItemPurchase>().setStartingData(foodItem.foodItem.name, foodItem.minPurchase, foodItem.maxPurchase, foodItem.foodItem.price);
        }
    }
}
