using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class SvrInputSample : MonoBehaviour
{
    [Header("HMD")]
    public Text HMD_position;
    public Text HMD_rotation;
    public Text HMD_battery;
    public GameObject HMD_Charging;
    public GameObject HMD_BackButton;
    public GameObject HMD_ClickButton;
    public GameObject HMD_HomeButton;
    public GameObject HMD_VolumeUpButton;
    public GameObject HMD_VolumeDownButton;
    [Header("RightController")]
    public GameObject R_Connected;
    public GameObject R_Awaked;
    public Text R_position;
    public Text R_rotation;
    public Text R_battery;
    public Text R_trigger;
    public GameObject R_ClickButton;
    public GameObject R_AppButton;
    public GameObject R_HomeButton;
    public GameObject R_TriggerButton;
    public Text R_Touch;
    public GameObject R_Touched;
    public Text R_DeviceName;
    [Header("LeftController")]
    public GameObject L_Connected;
    public Text L_position;
    public Text L_rotation;
    public Text L_battery;
    public Text L_trigger;
    public GameObject L_ClickButton;
    public GameObject L_AppButton;
    public GameObject L_HomeButton;
    public GameObject L_TriggerButton;
    public Text L_Touch;
    public GameObject L_Touched;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHMDDevice();
        UpdateRightControllerDevice();
        UpdateLeftControllerDevice();
    }
    private void UpdateHMDDevice()
    {
       
        HMD_position.text = GvrControllerInput.GetPosition(SvrControllerState.Head).ToString();
        HMD_rotation.text = GvrControllerInput.GetOrientation(SvrControllerState.Head).ToString();
        HMD_battery.text = GvrControllerInput.GetBatteryLevel(SvrControllerState.Head).ToString();

        //if (HMDDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("BatteryCharge"), out bool chagevalue))
        //{
        //    HMD_Charging.SetActive(chagevalue);
        //}
        HMD_BackButton.SetActive(GvrControllerInput.AppButton);
        HMD_ClickButton.SetActive(GvrControllerInput.ClickButton);
        HMD_HomeButton.SetActive(GvrControllerInput.HomeButtonState);
        //if (HMDDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("VolumeUpButton"), out bool VolumeUpvalue))
        //{
        //    HMD_VolumeUpButton.SetActive(VolumeUpvalue);
        //}
        //if (HMDDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("VolumeDownButton"), out bool VolumeDownvalue))
        //{
        //    HMD_VolumeDownButton.SetActive(VolumeDownvalue);
        //}
    }
    private void UpdateRightControllerDevice()
    {
        R_Connected.SetActive(GvrControllerInput.GetControllerState(SvrControllerState.RightController).isValid);
        R_Awaked.SetActive(GvrControllerInput.GetControllerState(SvrControllerState.RightController).Awaked);
        R_position.text = GvrControllerInput.GetPosition(SvrControllerState.RightController).ToString();
        R_rotation.text = GvrControllerInput.GetOrientation(SvrControllerState.RightController).ToString();
        R_battery.text = GvrControllerInput.GetBatteryLevel(SvrControllerState.RightController).ToString();
        R_ClickButton.SetActive(GvrControllerInput.ClickButton);
        R_AppButton.SetActive(GvrControllerInput.AppButton);
        R_HomeButton.SetActive(GvrControllerInput.HomeButtonState);
        R_TriggerButton.SetActive(GvrControllerInput.TriggerButton);
        R_Touch.text = GvrControllerInput.TouchPos.ToString();
        R_Touched.SetActive(GvrControllerInput.IsTouching);
        R_DeviceName.text = GvrControllerInput.GetControllerState(SvrControllerState.RightController).GetDeviceName();
    }

    private void UpdateLeftControllerDevice()
    {
        L_Connected.SetActive(GvrControllerInput.GetControllerState(SvrControllerState.RightController).isValid);
        L_position.text = GvrControllerInput.GetPosition(SvrControllerState.RightController).ToString();
        L_rotation.text = GvrControllerInput.GetOrientation(SvrControllerState.RightController).ToString();
        L_battery.text = GvrControllerInput.GetBatteryLevel(SvrControllerState.RightController).ToString();
        L_ClickButton.SetActive(GvrControllerInput.ClickButton);
        L_AppButton.SetActive(GvrControllerInput.AppButton);
        L_HomeButton.SetActive(GvrControllerInput.HomeButtonState);
        L_TriggerButton.SetActive(GvrControllerInput.TriggerButton);
        L_Touch.text = GvrControllerInput.TouchPos.ToString();
        L_Touched.SetActive(GvrControllerInput.IsTouching);
    }
}
