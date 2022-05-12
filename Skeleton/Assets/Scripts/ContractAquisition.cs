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
        var daily = gm.GetDailyContracts();
        foreach (var contract in daily)
        {
            if (contract.enabled)
            {
                GameObject thing = Instantiate(template) as GameObject;
                thing.SetActive(true);
                thing.transform.SetParent(template.transform.parent, false);
                thing.GetComponent<ContractOption>().setStartingData(contract);
            }
        }
    }
}
