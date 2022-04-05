using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContractOption : MonoBehaviour
{
    public TextMeshProUGUI company;
    public TextMeshProUGUI people;
    public TextMeshProUGUI weeks;
    public TextMeshProUGUI pay;
    public Button aquireButton;
    private Contract curContract;
    // Start is called before the first frame update
    void Start()
    {
        aquireButton.onClick.AddListener(() => { 
            GameManager.get().AquireContract(curContract);
            gameObject.SetActive(false);
        });
    }

    public void setStartingData(Contract contract)
    {
        curContract = contract;
        people.text += contract.people.ToString();
        weeks.text += contract.weeks.ToString();
        pay.text += contract.payment.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
