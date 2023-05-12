using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTargetingStrategy : TrackingStrategy
{
    private GameObject chosenFrameWork;
    private GameObject uiField;

    public AreaTargetingStrategy(GameObject chosenFrameWork, GameObject uiField)
    {
        this.uiField = uiField;
        this.chosenFrameWork = chosenFrameWork;
        Initialize();
    }
    // Start is called before the first frame update
    public override void Initialize()
    {
        chosenFrameWork.transform.GetChild(1).gameObject.SetActive(false);
        chosenFrameWork.transform.GetChild(2).gameObject.SetActive(true);
        uiField.transform.GetChild(1).gameObject.SetActive(false);
        uiField.transform.GetChild(2).gameObject.SetActive(false);
    }

    // Update is called once per frame
    public override void Track()
    {

    }
}
