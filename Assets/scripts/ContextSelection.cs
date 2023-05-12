using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSelection : MonoBehaviour
{
    private TrackingStrategy trackingStrategy;

    public void SetTrackingStrategy(TrackingStrategy trackingStrategy)
    {
        this.trackingStrategy = trackingStrategy;
    }

    public TrackingStrategy GetTrackingStrategy()
    {
        return trackingStrategy;
    }
    
    public void SetUpTracking()
    {
        trackingStrategy.Initialize();
    }
}
