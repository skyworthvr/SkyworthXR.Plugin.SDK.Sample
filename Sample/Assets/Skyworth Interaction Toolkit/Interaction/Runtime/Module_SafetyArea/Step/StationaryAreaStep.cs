using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyworth.Interaction.SafetyArea
{
    public class StationaryAreaStep : AbstractSafetyAreaStep<SafetyAreaMono>
    {
        public StationaryAreaStep(SafetyAreaMono safetyAreaMono) : base(safetyAreaMono)
        {

        }

        public override void OnEnterStep()
        {
            //reference.safetyGreyCameraUI.gameObject.SetActive(true);
            reference.stationaryAreaUI.gameObject.SetActive(true);
            reference.stationaryAreaUI.Init();
            reference.stationaryAreaUI.OnSwitchToPlayAreaClick += OnSwitchPlayAreaClick;
            reference.stationaryAreaUI.OnCancelClick += OnStationaryAreaCancelClick;
            reference.stationaryAreaUI.OnConfirmClick += OnStationaryAreaConfirmClick;
            SafetyAreaManager.Instance.CreateStationarySafetyArea();
            UnFreezeStationarySafetyArea();
            HidePlane();
            SafetyAreaManager.Instance.stationaryAreaMono.SetMaterial(reference.areaConfirmMat);
        }

        public override void OnExitStep()
        {
            reference.stationaryAreaUI.OnSwitchToPlayAreaClick -= OnSwitchPlayAreaClick;
            reference.stationaryAreaUI.OnCancelClick -= OnStationaryAreaCancelClick;
            reference.stationaryAreaUI.OnConfirmClick -= OnStationaryAreaConfirmClick;
            reference.stationaryAreaUI.Release();
            reference.stationaryAreaUI.gameObject.SetActive(false);
            //reference.safetyGreyCameraUI.gameObject.SetActive(false);
        }

        public override SafetyAreaStepEnum GetStepEnum()
        {
            return SafetyAreaStepEnum.StationaryArea;
        }

        //暂时隐藏平面
        public void HidePlane()
        {
            reference.safetyPlaneMono.gameObject.SetActive(false);
        }

        public void OnSwitchPlayAreaClick()
        {
            SafetyAreaManager.Instance.DestroyStationaryArea();
            SafetyAreaManager.Instance.ChangeStep(SafetyAreaStepEnum.PlayArea);
            SafetyAreaManager.Instance.GetStep<PlayAreaStep>(SafetyAreaStepEnum.PlayArea).ChangePlanePosition();
        }

        private void OnStationaryAreaCancelClick()
        {
            SafetyAreaManager.Instance.DestroyStationaryArea();
            reference.DestroySafetyPlane();
            SafetyAreaManager.Instance.ExitSafeAreaStep();
        }

        private void OnStationaryAreaConfirmClick()
        {
            SafetyAreaManager.Instance.stationaryAreaMono.SetMaterial(reference.areaNormalMat);
            FreezeStationarySafetyArea();
            reference.DestroySafetyPlane();
            SafetyAreaManager.Instance.ExitSafeAreaStep();
        }

        public void FreezeStationarySafetyArea()
        {
            if (SafetyAreaManager.Instance.stationaryAreaMono == null)
            {
                Debug.LogError("stationaryAreaMono is Null FreezeStationarySafetyArea");
            }
            SafetyAreaManager.Instance.stationaryAreaMono.FreezeStationaryAreaPosition();
        }

        public void UnFreezeStationarySafetyArea()
        {
            if (SafetyAreaManager.Instance.stationaryAreaMono == null)
            {
                Debug.LogError("stationaryAreaMono is Null UnFreezeStationarySafetyArea");
            }
            SafetyAreaManager.Instance.stationaryAreaMono.UnFreezeStationaryAreaPosition();
        }

        private Vector2 circleCenter = new Vector2(0, 0);

        public void SetCircleCenter(Vector3 headPosition)
        {
            circleCenter = new Vector2(headPosition.x, headPosition.z);
        }

        public Vector2 GetCircleCenter()
        {
            return circleCenter;
        }
    }
}