using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class FrameWorkManagerScript : MonoBehaviour
{
    public SupportedFrameWorks ARFrameWorks;
    [SerializeField]
    private List<GameObject> listOfFrameWorks;
    private GameObject chosenFrameWork;
    [SerializeField]
    GameObject uiField;
    void OnEnable()
    {
        //ARFrameWorks = SupportedFrameWorks.Vuforia;
        ARFrameWorks = SupportedFrameWorks.ARFoundation;//(SupportedFrameWorks) System.Enum.Parse(typeof(SupportedFrameWorks), PlayerPrefs.GetString("ARFrameWork"));
    }
    // Start is called before the first frame update
    void Start()
    {
        ARFrameWorks = (SupportedFrameWorks)System.Enum.Parse(typeof(SupportedFrameWorks), "ARFoundation");//(SupportedFrameWorks) System.Enum.Parse(typeof(SupportedFrameWorks), PlayerPrefs.GetString("ARFrameWork"));
        //ARFrameWorks = (SupportedFrameWorks)System.Enum.Parse(typeof(SupportedFrameWorks), "Vuforia");//(SupportedFrameWorks) System.Enum.Parse(typeof(SupportedFrameWorks), PlayerPrefs.GetString("ARFrameWork"));
        SelectFrameWork();
    }

    public void SwitchFrameWork()
    {
        if (ARFrameWorks == SupportedFrameWorks.Vuforia)
        {
            ARFrameWorks = SupportedFrameWorks.ARFoundation;
        }
        else if (ARFrameWorks == SupportedFrameWorks.ARFoundation)
        {
            ARFrameWorks = SupportedFrameWorks.Vuforia;
        }
        listOfFrameWorks.ForEach(x =>
        {
            x.SetActive(false);
        });
        listOfFrameWorks.Find(x => x.GetComponent<ARFrameWorkScript>().GetName() == ARFrameWorks).SetActive(true);
    }
    void SelectFrameWork()
    {
        foreach (GameObject frameWork in listOfFrameWorks)
        {
            if (frameWork.GetComponent<ARFrameWorkScript>().GetName() == ARFrameWorks)
            {
                frameWork.SetActive(true);
                chosenFrameWork = frameWork;
            }
            else
            {
                frameWork.SetActive(false);
            }
        }
        ContextSelection context = new ContextSelection();
        FrameWorkPolicy policy = new FrameWorkPolicy(context, ARFrameWorks, chosenFrameWork, uiField);
    }

    public SupportedFrameWorks GetCurrentFrameWork()
    {
        return ARFrameWorks;
    }
}
