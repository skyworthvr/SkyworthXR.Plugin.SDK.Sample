// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Gvr.Internal;
using Svr;
using UnityEngine.XR;
namespace Svr.Controller
{
    public enum ConnectStatus
    {
        Disconnected = 0,
        Scanning = 1,
        Connecting = 2,
        Connected = 3,
        NoRecenter = 4
    }
    public enum Type
    {
        I3VR = 0,
        Nolo_6dof = 1,
        Nolo_3dof = 2
    }
    public enum Handness
    {
        Right = 0,
        Left = 1,
        Head = 2
    }
    [System.Flags]
    public enum KeyCode
    { // TouchPad
        Button_Up = 0x00000001,
        Button_Down = 0x00000002,
        Button_Left = 0x00000004,
        Button_Right = 0x00000008,

        // system button
        Button_Enter = 0x00000010,
        Button_Home = 0x00000020,
        Button_Menu = 0x00000040,
        Button_Back = 0x00000080,
        Button_Volume_Up = 0x00000100,
        Button_Volume_Down = 0x00000200,

        // other button
        Button_Grip = 0x00000400,
        Button_Trigger = 0x00000800,

        Button_EnumSize = 0x7fffffff
    }
}
public class SvrAndroidServiceControllerProviderV3 : IControllerProvider
{

    //private static AndroidJavaClass JniUtilesClass;
    private String GetDeviceName(Svr.Controller.KeyCode keyCode)
    {
        switch (keyCode)
        {

            case Svr.Controller.KeyCode.Button_Enter:
                return "PrimaryButton";
            case Svr.Controller.KeyCode.Button_Home:
                return "HomeButton";
            case Svr.Controller.KeyCode.Button_Menu:
                return "MenuButton";
            case Svr.Controller.KeyCode.Button_Back:
                return "MenuButton";
            case Svr.Controller.KeyCode.Button_Volume_Up:
                return "VolumeUpButton";
            case Svr.Controller.KeyCode.Button_Volume_Down:
                return "VolumeDownButton";
            case Svr.Controller.KeyCode.Button_Grip:
                break;
            case Svr.Controller.KeyCode.Button_Trigger:
                return "TriggerButton";
            default:
                break;
        }
        return "";
    }




    private bool hasBatteryMethods = false;

    public bool SupportsBatteryStatus
    {
        get { return hasBatteryMethods; }
    }
    #region 动态库接口


    private bool SvrControllerTouchUp(ControllerState controllerState)
    {
        //if (controllerState.mpreControllerState == null) return false;
        //if (controllerState.mControllerState == null) return false;

        return controllerState.PreTouched && !controllerState.Touched;
    }

    private bool SvrControllerTouchDown(ControllerState controllerState)
    {
        //if (controllerState.mpreControllerState == null) return false;
        //if (controllerState.mControllerState == null) return false;
        return !controllerState.PreTouched && controllerState.Touched;
    }

    private bool SvrControllerButtonState(ControllerState controllerState, InputDevice inputDevice, Svr.Controller.KeyCode keyCode)
    {
        if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>(GetDeviceName(keyCode)), out bool value) && value)
        {

            controllerState.KeyCode |= (uint)keyCode;
            return true;
        }
        else
        {
            if ((controllerState.KeyCode & (uint)keyCode) != 0)
                controllerState.KeyCode ^= (uint)keyCode;
            return false;
        }
    }

    private bool SvrControllerButtonDown(ControllerState controllerState, Svr.Controller.KeyCode keyCode)
    {
        if ((controllerState.PreKeyCode & (uint)keyCode) == 0 && (controllerState.KeyCode & (uint)keyCode) != 0)
            return true;
        return false;
    }

    private bool SvrControllerButtonUp(ControllerState controllerState, Svr.Controller.KeyCode keyCode)
    {
        //if (controllerState.mpreControllerState == null) return false;
        //if (controllerState.mControllerState == null) return false;

        if ((controllerState.PreKeyCode & (uint)keyCode) != 0 && (controllerState.KeyCode & (uint)keyCode) == 0)
            return true;
        return false;
    }



    #endregion

    internal SvrAndroidServiceControllerProviderV3()
    {
        SvrLog.Log("SvrAndroidServiceControllerProviderV3");
        //JniUtilesClass = new AndroidJavaClass("com.ssnwt.sdk.JniUtiles");
    }

    ~SvrAndroidServiceControllerProviderV3()
    {

    }

    public void ReadState(ControllerState outState, InputDevice inputDevice)
    {

        if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>("DevicePosition"), out Vector3 positionValue))
        {
            outState.position = positionValue;
        }
        if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>("DeviceRotation"), out Quaternion rotationValue))
        {
            outState.orientation = rotationValue;
        }
        if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector3>("CenterEyePosition"), out Vector3 HeadpositionValue))
        {
            outState.position = HeadpositionValue;
        }
        if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Quaternion>("CenterEyeRotation"), out Quaternion HeadrotationValue))
        {
            outState.orientation = HeadrotationValue;
        }
        if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("ControllerAwaked"), out bool awaked))
        {
            outState.Awaked = awaked;
        }
        else
        {
            outState.Awaked = true;
        }

        SvrControllerUpdateState(outState, inputDevice);
    }

    private void SvrControllerUpdateState(ControllerState outState, InputDevice inputDevice)
    {
        if (outState.Awaked)
        {
            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<Vector2>("Primary2DAxis"), out Vector2 touch))
            {
                outState.touchPos = touch;
            }

            outState.apiStatus = GvrControllerApiStatus.Ok;

            outState.appButtonState = SvrControllerButtonState(outState, inputDevice, Svr.Controller.KeyCode.Button_Menu);
            outState.appButtonDown = SvrControllerButtonDown(outState, Svr.Controller.KeyCode.Button_Menu);
            outState.appButtonUp = SvrControllerButtonUp(outState, Svr.Controller.KeyCode.Button_Menu);

            outState.clickButtonState = SvrControllerButtonState(outState, inputDevice, Svr.Controller.KeyCode.Button_Enter);
            outState.clickButtonDown = SvrControllerButtonDown(outState, Svr.Controller.KeyCode.Button_Enter);
            outState.clickButtonUp = SvrControllerButtonUp(outState, Svr.Controller.KeyCode.Button_Enter);
            
            outState.triggerButtonState = SvrControllerButtonState(outState, inputDevice, Svr.Controller.KeyCode.Button_Trigger);
            outState.triggerButtonDown = SvrControllerButtonDown(outState, Svr.Controller.KeyCode.Button_Trigger);
            outState.triggerButtonUp = SvrControllerButtonUp(outState, Svr.Controller.KeyCode.Button_Trigger);

            outState.gripButtonState = SvrControllerButtonState(outState, inputDevice, Svr.Controller.KeyCode.Button_Grip);
            outState.gripButtonDown = SvrControllerButtonDown(outState, Svr.Controller.KeyCode.Button_Grip);
            outState.gripButtonUp = SvrControllerButtonUp(outState, Svr.Controller.KeyCode.Button_Grip);

            //outState.TouchPadUpButtonState = SvrControllerButtonState(outState, Svr.Controller.KeyCode.Button_Up);
            //outState.TouchPadLeftButtonState = SvrControllerButtonState(outState, Svr.Controller.KeyCode.Button_Left);
            //outState.TouchPadDownButtonState = SvrControllerButtonState(outState, Svr.Controller.KeyCode.Button_Down);
            //outState.TouchPadRightButtonState = SvrControllerButtonState(outState, Svr.Controller.KeyCode.Button_Right);

            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("PrimaryTouch"), out bool touched))
            {
                outState.isTouching = touched;
                outState.Touched = touched;
            }


            outState.touchDown = SvrControllerTouchDown(outState);
            outState.touchUp = SvrControllerTouchUp(outState);
            //byte[] devices = { };
            //if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<byte[]>("DeviceName"), devices))
            //{
            //    string deviceName = System.Text.Encoding.UTF8.GetString(devices);
            //    //string deviceName = JniUtilesClass.CallStatic<string>("float2string", new float[] { devices.x, devices.y, devices.z, devices.w }, 0, 4);
            //    //Debug.LogFormat("xmh deviceName({0},{1},{2},{3})--{4}", devices.x, devices.y, devices.z, devices.w, deviceName);
            //    Debug.LogFormat("devices:(%d)",devices.Length);
            //    outState.deviceName = deviceName;
            //}
            outState.deviceName = inputDevice.name;
            if (inputDevice.TryGetFeatureValue(new InputFeatureUsage<float>("BatteryLevel"), out float battery))
            {
                //int battery = (int)outState.mControllerState.battery;
                outState.batteryValue = (int)battery;
                if (battery >= 20 * 4)
                {
                    outState.batteryLevel = GvrControllerBatteryLevel.Full;
                }
                else if (battery > 20 * 3)
                {
                    outState.batteryLevel = GvrControllerBatteryLevel.AlmostFull;
                }
                else if (battery >= 20 * 2)
                {
                    outState.batteryLevel = GvrControllerBatteryLevel.Medium;
                }
                else if (battery >= 20)
                {
                    outState.batteryLevel = GvrControllerBatteryLevel.Low;
                }
                else
                {
                    outState.batteryLevel = GvrControllerBatteryLevel.CriticalLow;
                }
            }
            outState.errorDetails = "";

            if (inputDevice.manufacturer == "SkywortXr")
            {
                outState.deviceManufacturer = DeviceManufacturer.Svr_6dof;
            }
            else 
            {
                outState.deviceManufacturer = DeviceManufacturer.Svr_3dof;
            }
        }

        outState.homeButtonDown = SvrControllerButtonDown(outState, Svr.Controller.KeyCode.Button_Home);
        outState.homeButtonState = SvrControllerButtonState(outState, inputDevice, Svr.Controller.KeyCode.Button_Home);

        outState.homeButtonUp = SvrControllerButtonUp(outState, Svr.Controller.KeyCode.Button_Home);

    }

    private GvrControllerApiStatus ConvertControllerApiStatus(int gvrControllerApiStatus)
    {
        return GvrControllerApiStatus.Ok;
    }

    public enum SvrControllerHandedness
    {
        Error = -1,
        Right = 0,
        Left = 1,
    }


    public void Release()
    {

    }

    public void OnQuit()
    {
    }

    public void ReadState(ControllerState controllerStateLeft, ControllerState controllerStateRight, ControllerState controllerStateHead)
    {
       
        controllerStateRight.ClearTransientState();
        controllerStateLeft.ClearTransientState();
        controllerStateHead.ClearTransientState();
        InputDevice right_hand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        InputDevice left_hand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice Head = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        controllerStateHead.isValid = Head.isValid;
        controllerStateRight.isValid = right_hand.isValid;
        controllerStateLeft.isValid = left_hand.isValid;
        if (Head.isValid)
        {
            //controllerStateHead.connectionState = GvrConnectionState.Connected;
            ReadState(controllerStateHead, Head);
        }
        if (right_hand.isValid)
        {
            //controllerStateRight.connectionState = GvrConnectionState.Connected;
            ReadState(controllerStateRight, right_hand);
        }
        if (left_hand.isValid)
        {
            //controllerStateRight.connectionState = GvrConnectionState.Connected;
            ReadState(controllerStateLeft, left_hand);
        }

    }

    public void ReadState(ControllerState outState)
    {
    }

    public void OnPause()
    {
    }

    public void OnResume()
    {
    }
}
