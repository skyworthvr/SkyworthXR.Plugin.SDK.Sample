using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHeightStep : AbstractSafetyAreaStep<SafetyAreaMono>
{
    private string PLANE_HEIGHT_SAVE_KEY = "PLANE_HEIGHT_SAVE_KEY";

    private float planeHeight;

    public GroundHeightStep(SafetyAreaMono safetyAreaMono) : base(safetyAreaMono)
    {
        LoadPlaneHeight();
    }

    public override void OnEnterStep()
    {
        if (!SafetyAreaManager.Instance.onlySettingGroundHeight)
        {
            SafetyAreaManager.Instance.DelayShow(1f, DelayShowUI);
        }
        else
        {
            reference.groundHeightUI.gameObject.SetActive(true);
        }

        //reference.groundHeightUI.gameObject.SetActive(true);
        //reference.safetyGreyCameraUI.gameObject.SetActive(true);
        reference.groundHeightUI.Init();
        reference.groundHeightUI.OnConfirmClick += SwitchToPlayAreaStep;
        reference.groundHeightUI.OnResetClick += ResetPlaneHeight;
        reference.groundHeightUI.OnCancelClick += ExitSafetyAreaProcess;
        reference.CreateSafetyPlane();
        LoadPlaneHeight();
        UnFreezePlaneHeight();
        SetPlayAreaMat();
        reference.safetyPlaneMono.StopPlaneAnimation();
    }

    public override void OnExitStep()
    {
        SavePlaneHeight();
        FreezePlaneHeight();
        SetPlayAreaConfirmedMat();
        reference.groundHeightUI.OnConfirmClick -= SwitchToPlayAreaStep;
        reference.groundHeightUI.OnResetClick -= ResetPlaneHeight;
        reference.groundHeightUI.OnCancelClick -= ExitSafetyAreaProcess;
        reference.groundHeightUI.Release();
        reference.groundHeightUI.gameObject.SetActive(false);
        //reference.safetyGreyCameraUI.gameObject.SetActive(false);
    }

    public override SafetyAreaStepEnum GetStepEnum()
    {
        return SafetyAreaStepEnum.GroundHeight;
    }

    private void DelayShowUI()
    {
        if (SafetyAreaManager.Instance.GetCurrentStepEnum() == SafetyAreaStepEnum.GroundHeight)
        {
            reference.groundHeightUI.gameObject.SetActive(true);
        }
    }

    private void ExitSafetyAreaProcess()
    {
        reference.DestroySafetyPlane();
        SafetyAreaManager.Instance.ExitSafeAreaStep();
    }

    private void DelaySafetyAreaProcess()
    {
        SafetyAreaManager.Instance.DelayExitSafeAreaStep(0.7f, reference.DestroySafetyPlane);
    }

    //设置确定地面高度材质
    public void SetPlayAreaMat()
    {
        if (reference.safetyPlaneMono == null) return;
        reference.safetyPlaneMono.SetPlaneMat(reference.safetyPlaneMat);
        reference.safetyPlaneMono.SetHoverPlaneMat(reference.safetyPlaneMat);
    }

    //设置以确定地面高度参数
    public void SetPlayAreaConfirmedMat()
    {
        if (reference.safetyPlaneMono == null) return;
        reference.safetyPlaneMono.SetPlaneMat(reference.safetyPlaneConfirmedMat);
        reference.safetyPlaneMono.SetHoverPlaneMat(reference.safetyPlaneHoverMat);
    }

    //允许平面高度变化
    public void UnFreezePlaneHeight()
    {
        if (reference.safetyPlaneMono == null)
        {
            Debug.LogError("safetyPlaneMono is Null UnFreezePlaneHeight");
            return;
        }
        reference.safetyPlaneMono.UnFreezePlaneHeight();
    }

    //冻结平面高度
    public void FreezePlaneHeight()
    {
        if (reference.safetyPlaneMono == null)
        {
            return;
        }
        reference.safetyPlaneMono.FreezePlaneHeight();
    }

    private void SwitchToPlayAreaStep()
    {
        reference.safetyPlaneMono.StartPlaneAnimation();
        if (SafetyAreaManager.Instance.onlySettingGroundHeight)
        {
            SafetyAreaManager.Instance.ResetSafetyAreaHeight();
            //ExitSafetyAreaProcess();
            DelaySafetyAreaProcess();
            return;
        }
        SafetyAreaManager.Instance.DelayChangeStep(SafetyAreaStepEnum.PlayArea, 0.7f);
    }

    public void SetHeadPosition(Vector3 headPosition)
    {
        float largestHeight = headPosition.y - PlayAreaConstant.DEFAULT_HEIGHT_FROM_HEAD;
        if (largestHeight < planeHeight)
        {
            planeHeight = largestHeight;
        }
    }

    public void SetPlaneHeight(float interactionObjectHeight)
    {
        if (interactionObjectHeight < planeHeight)
        {
            planeHeight = interactionObjectHeight;
        }
    }

    public float GetPlaneHeight()
    {
        return planeHeight;
    }

    public void ResetPlaneHeight()
    {
        planeHeight = float.MaxValue;
    }

    public void LoadPlaneHeight()
    {
        planeHeight = PlayerPrefs.GetFloat(PLANE_HEIGHT_SAVE_KEY, float.MaxValue);
    }

    public void SavePlaneHeight()
    {
        PlayerPrefs.SetFloat(PLANE_HEIGHT_SAVE_KEY, planeHeight);
        PlayerPrefs.Save();
    }
}