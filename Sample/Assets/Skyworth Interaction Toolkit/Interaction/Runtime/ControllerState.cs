// Copyright 2016 Google Inc. All rights reserved.
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

/// @cond
namespace Gvr.Internal
{
    /// Internal representation of the controller's current state.
    /// This representation is used by controller providers to represent the controller's state.
    ///
    /// The fields in this class have identical meanings to their correspondents in the GVR C API,
    /// so they are not redundantly documented here.
    public class ControllerState
    {
        internal SvrControllerIndex svrControllerIndex = SvrControllerIndex.SVR_CONTROLLER_INDEX_RIGHT;
        public bool Awaked = false;
        public bool isValid = false;
        internal GvrControllerApiStatus apiStatus = GvrControllerApiStatus.Unavailable;
        private Quaternion mOrientation = Quaternion.identity;
        internal Quaternion orientation
        {
            get
            {
                return RecentQuaternion * mOrientation;
            }
            set
            {
                mOrientation = value;
            }
        }
        internal Vector3 position = Vector3.zero;
        internal Vector3 gyro = Vector3.zero;
        internal Vector3 accel = Vector3.zero;
        internal bool isTouching = false;
        internal Vector2 touchPos = Vector2.zero;
        internal float triggervalue = 0;
        internal float gripvalue = 0;
        internal bool touchDown = false;
        internal bool touchUp = false;
        internal bool recentered = false;

        internal bool clickButtonState = false;
        internal bool clickButtonDown = false;
        internal bool clickButtonUp = false;

        internal bool triggerButtonState = false;
        internal bool triggerButtonDown = false;
        internal bool triggerButtonUp = false;

        internal bool appButtonState = false;
        internal bool appButtonDown = false;
        internal bool appButtonUp = false;

        public bool homeButtonDown = false;
        public bool homeButtonUp = false;
        public bool homeButtonState = false;

        internal bool gripButtonDown = false;
        internal bool gripButtonUp = false;
        internal bool gripButtonState = false;

        internal bool TouchPadUpButtonState = false;
        internal bool TouchPadUpButtonDown = false;
        internal bool TouchPadUpButtonUp = false;
        internal bool TouchPadDownButtonState = false;
        internal bool TouchPadDownButtonDown = false;
        internal bool TouchPadDownButtonUp = false;
        internal bool TouchPadLeftButtonState = false;
        internal bool TouchPadLeftButtonDown = false;
        internal bool TouchPadLeftButtonUp = false;
        internal bool TouchPadRightButtonState = false;
        internal bool TouchPadRightButtonDown = false;
        internal bool TouchPadRightButtonUp = false;

        internal string errorDetails = "";
        internal string deviceName;
        internal IntPtr gvrPtr = IntPtr.Zero;

        internal bool isCharging = false;
        internal GvrControllerBatteryLevel batteryLevel = GvrControllerBatteryLevel.Unknown;
        public GvrControllerBatteryLevel BatteryLevel { get { return batteryLevel; } }
        internal int batteryValue;
        public int BatteryValue { get { return batteryValue; } }
        public uint KeyCode;
        public uint PreKeyCode;
        public bool Touched;
        public bool PreTouched;
        public DeviceManufacturer deviceManufacturer;
        private Quaternion RecentQuaternion = Quaternion.identity;
        public ControllerState(SvrControllerIndex index)
        {
            svrControllerIndex = index;
        }
        //public GvrConnectionState GetConnectionState() { return connectionState; }
        public string GetDeviceName() { return deviceName; }
        public void CopyFrom(ControllerState other)
        {
            //connectionState = other.connectionState;
            apiStatus = other.apiStatus;
            orientation = other.orientation;
            gyro = other.gyro;
            accel = other.accel;
            isTouching = other.isTouching;
            touchPos = other.touchPos;
            touchDown = other.touchDown;
            touchUp = other.touchUp;
            recentered = other.recentered;
            clickButtonState = other.clickButtonState;
            clickButtonDown = other.clickButtonDown;
            clickButtonUp = other.clickButtonUp;
            triggerButtonState = other.triggerButtonState;
            triggerButtonDown = other.triggerButtonDown;
            triggerButtonUp = other.triggerButtonUp;
            appButtonState = other.appButtonState;
            appButtonDown = other.appButtonDown;
            appButtonUp = other.appButtonUp;
            homeButtonDown = other.homeButtonDown;
            homeButtonUp = other.homeButtonUp;
            homeButtonState = other.homeButtonState;
            gripButtonDown = other.gripButtonDown;
            gripButtonUp = other.gripButtonUp;
            gripButtonState = other.gripButtonState;

            TouchPadUpButtonState = other.TouchPadUpButtonState;
            TouchPadUpButtonDown = other.TouchPadUpButtonDown;
            TouchPadUpButtonUp = other.TouchPadUpButtonUp;
            TouchPadDownButtonState = other.TouchPadDownButtonState;
            TouchPadDownButtonDown = other.TouchPadDownButtonDown;
            TouchPadDownButtonUp = other.TouchPadDownButtonUp;
            TouchPadLeftButtonState = other.TouchPadLeftButtonState;
            TouchPadLeftButtonDown = other.TouchPadLeftButtonDown;
            TouchPadLeftButtonUp = other.TouchPadLeftButtonUp;
            TouchPadRightButtonState = other.TouchPadRightButtonState;
            TouchPadRightButtonDown = other.TouchPadRightButtonDown;
            TouchPadRightButtonUp = other.TouchPadRightButtonUp;


            errorDetails = other.errorDetails;
            gvrPtr = other.gvrPtr;
            isCharging = other.isCharging;
            batteryLevel = other.batteryLevel;

            triggervalue = other.triggervalue;
            gripvalue = other.gripvalue;
        }

        /// Resets the transient state (the state variables that represent events, and which are true
        /// for only one frame).
        public void ClearTransientState()
        {
            touchDown = false;
            touchUp = false;
            recentered = false;
            clickButtonDown = false;
            clickButtonUp = false;
            appButtonDown = false;
            appButtonUp = false;
            homeButtonDown = false;
            homeButtonUp = false;
            triggerButtonDown = false;
            triggerButtonUp = false;
            PreTouched = Touched;
            PreKeyCode = KeyCode;
            KeyCode = 0;
            //connectionState = GvrConnectionState.Disconnected;
            Awaked = false;
            deviceManufacturer = 0;
            position = Vector3.zero;
        }

        public void Recent()
        {
            //if (GvrViewer.Instance == null) return;
            //if (Vector3.Dot(Vector3.up, Quaternion.Euler(GvrViewer.Instance.HeadPose.Orientation.eulerAngles.x, 0, 0) * Vector3.forward) > 0
            //&& Vector3.Angle(Quaternion.Euler(GvrViewer.Instance.HeadPose.Orientation.eulerAngles.x, 0, 0) * Vector3.forward, Vector3.forward) > 45)
            //{
            //    RecentQuaternion = Quaternion.Inverse(mOrientation);
            //}
            //else
            //{
            //    RecentQuaternion = Quaternion.Inverse(Quaternion.Euler(0, mOrientation.eulerAngles.y, 0));
            //}
        }
    }
}
/// @endcond

