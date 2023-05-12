using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchFrameworkButton : MonoBehaviour
{
    public GameObject FrameWorkManager;
    // Start is called before the first frame update
    public void SwitchFramework()
    {
        FrameWorkManager.GetComponent<FrameWorkManagerScript>().SwitchFrameWork();
    }
}
