using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Oculus.Interaction;
using Normal.Realtime;
using System.Linq;

//Please see https://www.youtube.com/watch?v=DFijT_S6FpM

public enum HandToTrack
{
    OVRLeft,
    OVRRight
}

public class Brush : MonoBehaviour {
    // Reference to Realtime to use to instantiate brush strokes
    [SerializeField] private Realtime _realtime = null;

    // Prefab to instantiate when we draw a new brush stroke
    [SerializeField] private GameObject _brushStrokePrefab = null;

    // Which hand should this brush instance track?
    private enum Hand { LeftHand, RightHand };
    [SerializeField] private Hand _hand = Hand.RightHand;

    // Used to keep track of the current brush tip position and the actively drawing brush stroke
    private Vector3     _handPosition;
    private Quaternion  _handRotation;
    private BrushStroke _activeBrushStroke;
    //private bool holdBrush;
    //[SerializeField]
    private OVRHand ovrHand;
    [SerializeField]
    private float minFingerPinchDownStrength = 0.5f;
    private bool IsPinchingReleased = false;
    private OVRSkeleton ovrSkeleton;
    private OVRBone boneToTrack;
    [SerializeField]
    private GameObject objectToTrackMovement;
    ////private GrabOwnership grabOwnership;
    //[SerializeField]
    //private HandToTrack _handToTrack = HandToTrack.OVRLeft;
    //public void BrushHold(bool hold)
    //{
    //    holdBrush = hold;
    //}
    bool triggerPressed;
    private void Start()
    {
        //grabOwnership = GetComponent<GrabOwnership>();
        ovrHand = objectToTrackMovement.GetComponent<OVRHand>();
        ovrSkeleton = objectToTrackMovement.GetComponent<OVRSkeleton>();

        boneToTrack = ovrSkeleton.Bones
        .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index1)
        .SingleOrDefault();
        //_skeleton = _fingerHand.GetComponent<OVRSkeleton>();

        //_boneToTrack = _skeleton.Bones
        //    .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index1)
        //    .SingleOrDefault();
    }
  

    private void Update() {
        if (!_realtime.connected)
            return;

        if (boneToTrack == null)
        {
            Debug.Log("boneToTrack == null");
            boneToTrack = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index3)
                .SingleOrDefault();

            if(boneToTrack != null)
            objectToTrackMovement = boneToTrack.Transform.gameObject;
        }

        CheckPinchState();

        //if(_boneToTrack == null)
        //{
        //    _boneToTrack = _skeleton.Bones
        //    .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index3)
        //    .SingleOrDefault();

        //    objectToTrackMovement = _boneToTrack.Transform.gameObject;
        //}

        // Start by figuring out which hand we're tracking
        XRNode node    = _hand == Hand.LeftHand ? XRNode.LeftHand : XRNode.RightHand;
        string trigger = _hand == Hand.LeftHand ? "Left Trigger" : "Right Trigger";

        // Get the position & rotation of the hand
        //bool handIsTracking = UpdatePose(node, ref _handPosition, ref _handRotation);
        _handPosition = objectToTrackMovement.transform.position;
        _handRotation = objectToTrackMovement.transform.rotation;
        // Figure out if the trigger is pressed or not
        //bool triggerPressed = Input.GetAxisRaw(trigger) > 0.1f;
        //bool isIndexFingerPinching = _fingerHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        //Debug.Log("handIsTracking: " + handIsTracking);
        triggerPressed = IsPinchingReleased; //local hold
        
        Debug.Log("triggerPressed: "+triggerPressed);
        // If we lose tracking, stop drawing
        //if (!handIsTracking)
         //   triggerPressed = false;

        // If the trigger is pressed and we haven't created a new brush stroke to draw, create one!
        if (triggerPressed && _activeBrushStroke == null) {
            // Instantiate a copy of the Brush Stroke prefab, set it to be owned by us.
            //GameObject brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name, ownedByClient: true, useInstance: _realtime);
            GameObject brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name,new Realtime.InstantiateOptions
            {
                ownedByClient = true,
                preventOwnershipTakeover = false,
                destroyWhenOwnerLeaves = true,
                destroyWhenLastClientLeaves = true,
                useInstance = _realtime
            }
                );
            // Grab the BrushStroke component from it
            _activeBrushStroke = brushStrokeGameObject.GetComponent<BrushStroke>();

            // Tell the BrushStroke to begin drawing at the current brush position
            _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(_handPosition, _handRotation);
        }

        // If the trigger is pressed, and we have a brush stroke, move the brush stroke to the new brush tip position
        if (triggerPressed)
            _activeBrushStroke.MoveBrushTipToPoint(_handPosition, _handRotation);

        // If the trigger is no longer pressed, and we still have an active brush stroke, mark it as finished and clear it.
        if (!triggerPressed && _activeBrushStroke != null) {
            _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(_handPosition, _handRotation);
            _activeBrushStroke = null;
        }
    }

    private void CheckPinchState()
    {
        bool isIndexFingerPinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        float indexFingerPinchStrength = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        if (ovrHand.GetFingerConfidence(OVRHand.HandFinger.Index) != OVRHand.TrackingConfidence.High)
            return;

        // finger pinch down
        if (isIndexFingerPinching && indexFingerPinchStrength >= minFingerPinchDownStrength)
        {
            IsPinchingReleased = true;
            Debug.Log("IsPinchingReleased: " + IsPinchingReleased);
            return;
        }

        // finger pinch up
        if (IsPinchingReleased)
        {
            IsPinchingReleased = false;
        }
    }

    //// Utility

    // Given an XRNode, get the current position & rotation. If it's not tracking, don't modify the position & rotation.
    private static bool UpdatePose(XRNode node, ref Vector3 position, ref Quaternion rotation) {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates) {
            if (nodeState.nodeType == node) {
                Vector3    nodePosition;
                Quaternion nodeRotation;
                bool gotPosition = nodeState.TryGetPosition(out nodePosition);
                bool gotRotation = nodeState.TryGetRotation(out nodeRotation);

                if (gotPosition)
                    position = nodePosition;
                if (gotRotation)
                    rotation = nodeRotation;

                return gotPosition;
            }
        }

        return false;
    }

    //private static bool UpdateFinger(ref Vector3 position, ref Quaternion rotation)
    //{
    //    position = objectToTrackMovement.transform.position;
    //}
}
