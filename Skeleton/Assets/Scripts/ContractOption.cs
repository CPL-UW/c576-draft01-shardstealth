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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setStartingData(int peopleCount, int weeksCount, int payCount)
    {
        people.text += peopleCount.ToString();
        weeks.text += weeksCount.ToString();
        pay.text += payCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
