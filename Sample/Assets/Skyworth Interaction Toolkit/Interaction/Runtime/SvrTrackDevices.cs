using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrTrackDevices : MonoBehaviour
{
    public DeviceManufacturer deviceManufacturer;
    public SvrControllerState controllerState;
    private GvrBasePointer mPointer;
    public static List<SvrTrackDevices> trackDevices = new List<SvrTrackDevices>();
    private void Awake()
    {
        trackDevices.Add(this);
        GvrControllerInput.OnConterollerChanged += GvrControllerInput_OnConterollerChanged;
        GvrControllerInput_OnConterollerChanged(GvrControllerInput.SvrState, GvrControllerInput.SvrState);
        GvrControllerInput.OnGvrPointerEnable += GvrControllerInput_OnGvrPointerEnable;
    }

    private void GvrControllerInput_OnGvrPointerEnable(bool obj)
    {
        if ((GvrControllerInput.SvrState & controllerState) != 0 && (GvrControllerInput.GetControllerState(controllerState).deviceManufacturer & deviceManufacturer) != 0)
        {
            gameObject.SetActive(obj);
        }
    }

    private void GvrControllerInput_OnConterollerChanged(SvrControllerState state, SvrControllerState oldState)
    {
        if (mPointer == null) mPointer = GetComponentInChildren<GvrBasePointer>();
        if ((state & controllerState) != 0 && (GvrControllerInput.GetControllerState(controllerState).deviceManufacturer & deviceManufacturer) != 0)
        {
            gameObject.SetActive(true);
            switch (Svr.SvrSetting.NHandedness)
            {
                case Svr.Svr6DOFHandedness.Left:
                    if (controllerState == SvrControllerState.LeftController)
                    {
                        mPointer.gameObject.SetActive(true);
                        GvrPointerInputModule.Pointer = mPointer;
                    }
                    else
                    {
                        mPointer.gameObject.SetActive(false);
                        if (GvrPointerInputModule.Pointer == mPointer)
                        {
                            GvrPointerInputModule.Pointer = null;
                        }
                    }
                    break;
                case Svr.Svr6DOFHandedness.Right:
                    if (controllerState == SvrControllerState.RightController)
                    {
                        mPointer.gameObject.SetActive(true);
                        GvrPointerInputModule.Pointer = mPointer;
                    }
                    else
                    {
                        mPointer.gameObject.SetActive(false);
                        if (GvrPointerInputModule.Pointer == mPointer)
                        {
                            GvrPointerInputModule.Pointer = null;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {

    }
    void Update()
    {

    }

    private void OnDestroy()
    {
        GvrControllerInput.OnGvrPointerEnable -= GvrControllerInput_OnGvrPointerEnable;
        GvrControllerInput.OnConterollerChanged -= GvrControllerInput_OnConterollerChanged;
    }

}
