using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARFrameWorkScript : MonoBehaviour
{
    [SerializeField]
    private SupportedFrameWorks name;

    public SupportedFrameWorks GetName()
    {
        return name;
    }
}
