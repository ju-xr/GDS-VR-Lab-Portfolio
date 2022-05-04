using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Normal.Realtime;

//*
public class VRAvatarCalibrator : RealtimeComponent<VRAvatarCalibratorModel>
{
    public RealtimeAvatar realtimeAvatar;
    private bool _isSelf;

    private PrimaryButtonWatcher watcherP;
    private SecondaryButtonWatcher watcherS;
    public bool IsPressed = false; // used to display button state in the Unity Inspector window

    public VRIK ik;
    private float scaleMlp_Avatar = 1f;

    float sizeF_Avatar;

    bool buttonP, buttonS;

    Vector3 scale_Avatar;

    void Start()
    {
        //watcherP = GameObject.Find("XR Origin Locomotion").GetComponent<PrimaryButtonWatcher>();
        //watcherS = GameObject.Find("XR Origin Locomotion").GetComponent<SecondaryButtonWatcher>();
        //watcherP.primaryButtonPress.AddListener(OnPrimaryButtonEvent);
        //watcherS.secondaryButtonPress.AddListener(OnSecondaryButtonEvent);


        if (realtimeAvatar.isOwnedLocallyInHierarchy)
        {
            _isSelf = true;
        }
    }
    #region sync model
    protected override void OnRealtimeModelReplaced(VRAvatarCalibratorModel previousModel, VRAvatarCalibratorModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.avatarScaleDidChange -= AvatarScaleDidChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it, populate it with 0 player
            if (currentModel.isFreshModel)
            {
                currentModel.avatarScale = ik.references.root.localScale;//
            }


            // Update the mesh render to match the new model
            UpdateAvatarScale();

            // Register for events so we'll know if player number changes later
            currentModel.avatarScaleDidChange += AvatarScaleDidChange;
        }
    }

    private void AvatarScaleDidChange(VRAvatarCalibratorModel model, Vector3 value)
    {
        // Update player number
        UpdateAvatarScale();
    }

    private void UpdateAvatarScale()
    {
        ik.references.root.localScale = model.avatarScale;

    }
    #endregion
    public void ScaleUp(bool pressedP)
    {
        if(_isSelf == true)
        {
        IsPressed = pressedP;
            //Compare the height of the head target to the height of the head bone, multiply scale by that value.
            sizeF_Avatar = (ik.solver.spine.headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);

            if (sizeF_Avatar < 1 && ik.references.root.localScale.x >= 0.8f)
            {
                scaleMlp_Avatar = 0.98f;
            }
           else
            {
                scaleMlp_Avatar = 1f;

            }
            buttonP = true;

       // print("P sizeF" + sizeF_Avatar);
        ik.references.root.localScale *= sizeF_Avatar * scaleMlp_Avatar;
            model.avatarScale = ik.references.root.localScale;
            //print("iklocalScale" + ik.references.root.localScale.x);
        }

    }

    public void ScaleDown(bool pressedS)
    {
        if (_isSelf == true)
        {
            IsPressed = pressedS;
            //Compare the height of the head target to the height of the head bone, multiply scale by that value.
            sizeF_Avatar = (ik.solver.spine.headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);

            if (sizeF_Avatar > 1 && ik.references.root.localScale.x <= 1.2f)
            {
                scaleMlp_Avatar = 1.02f;
            }
            else
            {
               scaleMlp_Avatar = 1f;

           }
            buttonS = true;

            print("S sizeF" + sizeF_Avatar);
            
            ik.references.root.localScale *= sizeF_Avatar * scaleMlp_Avatar;
            model.avatarScale = ik.references.root.localScale;
            //print("iklocalScale" + ik.references.root.localScale.x);
        }
          
    }

}
//*/