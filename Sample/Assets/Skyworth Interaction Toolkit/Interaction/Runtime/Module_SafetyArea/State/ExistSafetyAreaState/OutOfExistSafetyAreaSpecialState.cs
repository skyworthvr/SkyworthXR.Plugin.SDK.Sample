using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.SDK;
using UnityEngine;

public class OutOfExistSafetyAreaSpecialState : AbstractExistSafetyAreaSpecialState<SafetyAreaBase>
{
    private CameraClearFlags flags;
    private Color background;
    public override void OnStateBreathe()
    {
        //Debug.Log("OutOfExistSafetyAreaSpecialState OnStateBreathe");
        if (SafetyAreaManager.Instance.isSettingSafetyArea || SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            reference.outOfSafetyArea.SetActive(false);
            reference.nomapUI.SetActive(false);
            reference.slamLostUI.gameObject.SetActive(false);
            reference.meshRenderer.enabled = !SafetyAreaManager.Instance.isDisableSafetyArea;
            return;
        }

        reference.outOfSafetyArea.SetActive(true);
        reference.meshRenderer.enabled = true;

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

        if (reference.alpha < 10)
        {
            reference.ChangeState(ExistSafetyAreaEnum.Normal);
            return;
        }
    }

    public override void OnStateEnter(object data)
    {
        Debug.Log("OutOfExistSafetyAreaSpecialState OnStateEnter");
        flags = Camera.main.clearFlags;
        background = Camera.main.backgroundColor;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.clear;
        SafetyAreaManager.Instance.ExitSafetyAreaInvoke();
        if (!SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            SkyworthPerformance.GSXR_StartSeeThrough();
        }
        //SkyworthPerformance.GSXR_Add_SlamPauseCallback(OnSlamPause);
    }

    public override void OnStateExit(object data)
    {
        Debug.Log("OutOfExistSafetyAreaSpecialState OnStateExit");
        //API_GSXR_Slam.GSXR_Remove_SlamPauseCallback(OnSlamPause);
        Camera.main.clearFlags = flags;
        Camera.main.backgroundColor = background;
        SkyworthPerformance.GSXR_StopSeeThrough();
        reference.outOfSafetyArea.SetActive(false);
    }

    private void OnSlamPause(bool isPause)
    {
        if (!isPause)
        {
            SkyworthPerformance.GSXR_StartSeeThrough();
        }
    }
}
