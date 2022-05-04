using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Normal.Realtime;

[RequireComponent(typeof(RealtimeTransform))]
public class GrabOwnership : MonoBehaviour
{
    private RealtimeTransform realtimeTransform;

    private void Start()
    {
        realtimeTransform = GetComponent<RealtimeTransform>();
    }

    public bool RequestGrabOwnership()
    {
        if(realtimeTransform.ownerIDSelf == -1)
        {
            realtimeTransform.RequestOwnership(); //hold
            return true;
        }
        else
        {
            return false; //other's hold
        }
    }

    public void ClearGrabOwnership()
    {
        realtimeTransform.ClearOwnership();
    }


}
