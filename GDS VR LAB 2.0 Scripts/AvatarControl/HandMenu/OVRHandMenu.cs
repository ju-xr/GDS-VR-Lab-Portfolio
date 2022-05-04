using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using TMPro;
using Normal.Realtime;

//Parts of this script modified from tutorial https://www.youtube.com/watch?v=AB49Cgr2u6E&t=17s&ab_channel=DilmerValecillos
public class OVRHandMenu : MonoBehaviour
{
    [SerializeField]
    private OVRCameraRig cameraRig;

    [SerializeField]
    private GameObject targetHand;

    [SerializeField]
    private GameObject changeAvatarMenu;

    [SerializeField]
    private Vector3 offsetFromHand = new Vector3(0.5f, 0.5f, 0.5f);

    [SerializeField]
    private RealtimeAvatarManager _manager;
    private OVRPlayerControllerWithHandPoses playerControllerWithHandPoses;
    private AvatarSelectionRolling avatarSelectionRolling;
    public VRAvatarCalibrator avatarCalibrator;
    private bool locomotionOn = false;
    [SerializeField]
    private TextMeshPro word;

    bool avatarCreate;

    void Awake()
    {
        playerControllerWithHandPoses = GetComponent<OVRPlayerControllerWithHandPoses>();
        playerControllerWithHandPoses.EnableLinearMovement = locomotionOn;
        _manager.avatarCreated += AvatarCreated;
        _manager.avatarDestroyed += AvatarDestroyed;
    }
    private void AvatarCreated(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        // Find Local Avatar Gameobject
        avatarSelectionRolling = _manager.localAvatar.gameObject.GetComponent<AvatarSelectionRolling>();
        avatarCreate = true;
    }

    private void AvatarDestroyed(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        // Avatar destroyed!
        avatarCreate = false;
    }
    bool menuOn;
    public void HandMenuVisibility(bool state)
    {
        changeAvatarMenu.SetActive(state);
        menuOn = state;
        //Debug.Log("chagne avatar pose");
    }

    private void Update()
    {
        if (menuOn)
        {
            changeAvatarMenu.transform.position = targetHand.transform.position + offsetFromHand;
            changeAvatarMenu.transform.rotation = Quaternion.LookRotation(changeAvatarMenu.transform.position - cameraRig.centerEyeAnchor.transform.position, Vector3.up);
        }

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    avatarCalibrator = _manager.localAvatar.gameObject.GetComponentInChildren<VRAvatarCalibrator>();
        //    Debug.Log("avatar calibrator" + avatarCalibrator.gameObject.name);
        //}

    }

    public void AvatarSelectionOnClick()
    {
        //Debug.Log("change avatar on click");
        if(avatarCreate)
        avatarSelectionRolling.SetChangableAvatar();
    }

    public void AvatarCalibratorUpOnClick()
    {
        //avatarSelectionRolling.SetChangableAvatar();
        if (avatarCreate)
        {
            avatarCalibrator = _manager.localAvatar.gameObject.GetComponentInChildren<VRAvatarCalibrator>();
            //Debug.Log("avatar calibrator" + avatarCalibrator.gameObject.name);
            avatarCalibrator.ScaleUp(true);
        }
    }

    public void AvatarCalibratorDownOnClick()
    {
        //avatarSelectionRolling.SetChangableAvatar();
        if (avatarCreate)
        {
            avatarCalibrator = _manager.localAvatar.gameObject.GetComponentInChildren<VRAvatarCalibrator>();
            //Debug.Log("avatar calibrator" + avatarCalibrator.gameObject.name);
            avatarCalibrator.ScaleDown(true);
        }
    }

    public void LocomotionOnClick()
    {
        if (avatarCreate)
        {
            locomotionOn = !locomotionOn;
            var locomotionMenuState = locomotionOn ? "ON" : "OFF";
            word.text = $"Locomotion {locomotionMenuState}";
            GetComponent<CharacterController>().enabled = locomotionOn;
            playerControllerWithHandPoses.EnableLinearMovement = locomotionOn;

        }

    }
}
