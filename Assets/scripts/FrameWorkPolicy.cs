using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class FrameWorkPolicy
{
    private ContextSelection context;
    private SupportedFrameWorks ARFrameWork;
    private GameObject chosenFrameWork;
    private GameObject uiField;
    // Start is called before the first frame update
    public FrameWorkPolicy(ContextSelection contextSelection, SupportedFrameWorks ARFramework, GameObject chosenFrameWork, GameObject uiField)
    {
        context = contextSelection;
        this.ARFrameWork = ARFramework;
        this.chosenFrameWork = chosenFrameWork;
        
    }

    // Update is called once per frame
    public void Configure()
    {
        if (ARFrameWork == SupportedFrameWorks.Vuforia)
        {
            //Check if Area Targeting is available
            if (false)
            {
                //Set up Area Target
                context.SetTrackingStrategy(new AreaTargetingStrategy(chosenFrameWork, uiField));
                //chosenFrameWork.transform.GetChild(1).gameObject.SetActive(false);
                //chosenFrameWork.transform.GetChild(2).gameObject.SetActive(true);
            }
            //Check if Plane Detection is available
            else if (true)
            {
                context.SetTrackingStrategy(new PlaneDetectionStrategy(ARFrameWork, chosenFrameWork, uiField));
                //chosenFrameWork.transform.GetChild(1).gameObject.SetActive(true);
                //chosenFrameWork.transform.GetChild(2).gameObject.SetActive(false);
                //Set up Plane Detection
            }
            else
            {
                context.SetTrackingStrategy(new SampleTrackingStrategy());
                //Set up alternative --> suggest to choose other framework
            }

        }
        else if (ARFrameWork == SupportedFrameWorks.ARFoundation)
        {
            AROcclusionManager occlusionManager = chosenFrameWork.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<AROcclusionManager>();
            //Check if Depth Tracking is available
            
            if (occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
            {
                //Set up Depth Tracking
                context.SetTrackingStrategy(new DepthTrackingStrategy(chosenFrameWork, uiField));
            }
            //Check if Plane Detection is available as alternative
            else if (true)
            {
                context.SetTrackingStrategy(new PlaneDetectionStrategy(ARFrameWork, chosenFrameWork, uiField));
                //Set up Plane Detection
            }
            else
            {
                context.SetTrackingStrategy(new SampleTrackingStrategy());
                //Set up alternative --> suggest to choose other framework
            }
        }
        //Decide which strategy to initialize with
        //context.SetStrategy()
    }
}
