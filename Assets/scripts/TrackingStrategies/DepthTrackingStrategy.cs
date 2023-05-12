using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class DepthTrackingStrategy : TrackingStrategy
{
    [SerializeField]
    private GameObject depthMap;
    private GameObject chosenFrameWork;
    private GameObject uiField;
    // Start is called before the first frame update
    public DepthTrackingStrategy(GameObject chosenFrameWork, GameObject uiField)
    {
        this.uiField = uiField;
        this.chosenFrameWork = chosenFrameWork;
        Initialize();
    }

    public override void Initialize()
    {
        chosenFrameWork.transform.GetChild(0).gameObject.GetComponent<ReceiveAndUseDepthValues>().enabled = true;
        chosenFrameWork.transform.GetChild(0).gameObject.GetComponent<ARPlaneManager>().enabled = false;
        chosenFrameWork.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<AROcclusionManager>().enabled = true;
        uiField.transform.GetChild(1).gameObject.SetActive(true);
        uiField.transform.GetChild(2).gameObject.SetActive(true);
    }

    // Update is called once per frame
    public override void Track()
    {

    }
}
