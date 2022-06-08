using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.SDK;
using UnityEngine;

/// <summary>
/// 普通状态
/// </summary>
public class NormalExistSafetyAreaSpecialState : AbstractExistSafetyAreaSpecialState<SafetyAreaBase>
{

    private bool isOpenSeeThrough = false;
    private const float TAP_TIMER = 0.5f;
    private float tapTimer = 0f;
    public override void OnStateBreathe()
    {
        //Debug.Log("NormalExistSafetyAreaSpecialState OnStateBreathe");
        if (SafetyAreaManager.Instance.isSettingSafetyArea || SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            reference.outOfSafetyArea.SetActive(false);
            reference.nomapUI.SetActive(false);
            reference.slamLostUI.gameObject.SetActive(false);
            reference.meshRenderer.enabled = !SafetyAreaManager.Instance.isDisableSafetyArea;
            return;
        }

        int currentRelocState = SkyworthPerformance.GSXR_Get_OfflineMapRelocState();
        if (currentRelocState == 0)
        {
            reference.ChangeState(ExistSafetyAreaEnum.NoMap);
            return;
        }

        if (reference.distance > 5f)
        {
            reference.ChangeState(ExistSafetyAreaEnum.SlamLost);
            return;
        }

        if (reference.alpha >= 10)
        {
            reference.ChangeState(ExistSafetyAreaEnum.OutOfArea);
            return;
        }
    }

    public override void OnStateEnter(object data)
    {
        Debug.Log("NormalExistSafetyAreaSpecialState OnStateEnter");
        //API_GSXR_Slam.GSXR_Add_SlamPauseCallback(OnSlamPause);
        if (!SafetyAreaManager.Instance.isSettingSafetyArea)
        {
            SafetyAreaManager.Instance.EnterSafetyAreaInvoke();
        }
#if !UNITY_EDITOR
        //TpActionManager.Instance.ActiveTpListener(OnTpDown);
#endif
    }

    public override void OnStateExit(object data)
    {
        Debug.Log("NormalExistSafetyAreaSpecialState OnStateExit");
#if !UNITY_EDITOR
        //TpActionManager.Instance.CancelTpListener();
#endif
        //API_GSXR_Slam.GSXR_Remove_SlamPauseCallback(OnSlamPause);
    }

    private void OnSlamPause(bool isPause)
    {
        if (!isPause && !SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            if (isOpenSeeThrough)
            {
                SkyworthPerformance.GSXR_StartSeeThrough();
            }
        }
    }

    private void OnDisableChange(bool isDisable)
    {
        if (!isDisable && isOpenSeeThrough)
        {
            SkyworthPerformance.GSXR_StartSeeThrough();
        }
        else
        {
            SkyworthPerformance.GSXR_StopSeeThrough();
        }
    }

    private void OnTpDown()
    {
        if (SafetyAreaManager.Instance.isSettingSafetyArea || SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            return;
        }

        if (tapTimer != 0f)
        {
            if (isOpenSeeThrough)
            {
                //Debug.LogError("LQR GSXR_StopSeeThrough");
                isOpenSeeThrough = false;
                SkyworthPerformance.GSXR_StopSeeThrough();
            }
            else
            {
                //Debug.LogError("LQR GSXR_StartSeeThrough");
                isOpenSeeThrough = true;
                SkyworthPerformance.GSXR_StartSeeThrough();
            }
            tapTimer = 0f;
        }
        else
        {
            tapTimer = TAP_TIMER;
        }
    }
}
