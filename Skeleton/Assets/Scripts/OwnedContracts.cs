using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OwnedContracts : MonoBehaviour
{
    public GameObject template;

    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.get();
        var weekly = gm.gameData.OwnedContracts;
        foreach (var contract in weekly)
        {
            GameObject thing = Instantiate(template) as GameObject;
            thing.SetActive(true);
            thing.transform.SetParent(template.transform.parent, false);
            thing.GetComponent<ContractOption>().setStartingData(contract);
            thing.GetComponent<ContractOption>().aquireButton.gameObject.SetActive(false);
        }
    }
}
