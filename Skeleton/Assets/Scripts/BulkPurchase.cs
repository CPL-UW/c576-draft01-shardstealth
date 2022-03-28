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
        var daily = gm.GetDailyFoodItems();
        foreach (var foodItem in daily)
        {
            GameObject thing = Instantiate(template) as GameObject;
            thing.SetActive(true);
            thing.transform.SetParent(template.transform.parent, false);
            thing.GetComponent<FoodItemPurchase>().setStartingData(foodItem.foodItem.name, foodItem.minPurchase, foodItem.maxPurchase);
        }
    }
}
