using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractAquisition : MonoBehaviour
{
    public GameObject template;

    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.get();
        var weekly = gm.GetWeeklyContracts();
        foreach (var contract in weekly)
        {
            GameObject thing = Instantiate(template) as GameObject;
            thing.SetActive(true);
            thing.transform.SetParent(template.transform.parent, false);
            thing.GetComponent<ContractOption>().setStartingData(contract);
        }
    }
}
