using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
public class PlaneDetectionStrategy : TrackingStrategy
{
    private SupportedFrameWorks ARFrameWork;
    private GameObject chosenFrameWork;
    private GameObject uiField;
    public PlaneDetectionStrategy(SupportedFrameWorks ARFrameWork, GameObject chosenFrameWork, GameObject uiField)
    {
        this.uiField = uiField;
        this.ARFrameWork = ARFrameWork;
        this.chosenFrameWork = chosenFrameWork;
        Initialize();
    }
    // Start is called before the first frame update
    public override void Initialize()
    {
        if (ARFrameWork == SupportedFrameWorks.ARFoundation)
        {
            chosenFrameWork.transform.GetChild(0).gameObject.GetComponent<ReceiveAndUseDepthValues>().enabled = false;
            chosenFrameWork.transform.GetChild(0).gameObject.GetComponent<ARPlaneManager>().enabled = true;
            chosenFrameWork.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<AROcclusionManager>().enabled = false;
        }
        else if (ARFrameWork == SupportedFrameWorks.Vuforia)
        {
            chosenFrameWork.transform.GetChild(1).gameObject.SetActive(true);
            chosenFrameWork.transform.GetChild(2).gameObject.SetActive(false);
        }
        uiField.transform.GetChild(1).gameObject.SetActive(false);
        uiField.transform.GetChild(2).gameObject.SetActive(false);
    }

    // Update is called once per frame
    public override void Track()
    {

    }
}
