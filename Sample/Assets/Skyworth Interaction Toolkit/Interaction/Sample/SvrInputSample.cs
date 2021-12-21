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
    public Text R_Connected;
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
        R_Connected.text = GvrControllerInput.GetControllerState(SvrControllerState.RightController).connectionState.ToString();
        if (GvrControllerInput.GetControllerState(SvrControllerState.RightController).connectionState != GvrConnectionState.Connected)
            return;
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
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>("DevicePosition"), out Vector3 position)) 
        //{
        //    R_position.text = position.ToString();
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>("DeviceRotation"), out Quaternion rotation))
        //{
        //    R_rotation.text = rotation.ToString();
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<float>("BatteryLevel"), out float battery))
        //{
        //    R_battery.text = battery.ToString();
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<float>("Trigger"), out float triggervalue))
        //{
        //    R_trigger.text = triggervalue.ToString();
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("PrimaryButton"), out bool clickvalue))
        //{
        //    R_ClickButton.SetActive(clickvalue);
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("MenuButton"), out bool backvalue))
        //{
        //    R_AppButton.SetActive(backvalue);
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("HomeButton"), out bool homevalue))
        //{
        //    R_HomeButton.SetActive(homevalue);
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("TriggerButton"), out bool triggerbuttonvalue))
        //{
        //    R_TriggerButton.SetActive(triggerbuttonvalue);
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>("Primary2DAxis"), out Vector2 touchvalue))
        //{
        //    R_Touch.text = touchvalue.ToString();
        //}
        //if (RightControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("PrimaryTouch"), out bool touchedvalue))
        //{
        //    R_Touched.SetActive(touchedvalue);
        //}
    }

    private void UpdateLeftControllerDevice()
    {
        InputDevice LeftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        L_Connected.SetActive(LeftControllerDevice.isValid);
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>("DevicePosition"), out Vector3 position))
        {
            L_position.text = position.ToString();
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>("DeviceRotation"), out Quaternion rotation))
        {
            L_rotation.text = rotation.ToString();
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<float>("BatteryLevel"), out float battery))
        {
            L_battery.text = battery.ToString();
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<float>("Trigger"), out float triggervalue))
        {
            L_trigger.text = triggervalue.ToString();
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("PrimaryButton"), out bool clickvalue))
        {
            L_ClickButton.SetActive(clickvalue);
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("MenuButton"), out bool backvalue))
        {
            L_AppButton.SetActive(backvalue);
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("HomeButton"), out bool homevalue))
        {
            L_HomeButton.SetActive(homevalue);
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("TriggerButton"), out bool triggerbuttonvalue))
        {
            L_TriggerButton.SetActive(triggerbuttonvalue);
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>("Primary2DAxis"), out Vector2 touchvalue))
        {
            L_Touch.text = touchvalue.ToString();
        }
        if (LeftControllerDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("PrimaryTouch"), out bool touchedvalue))
        {
            L_Touched.SetActive(touchedvalue);
        }
    }
}
