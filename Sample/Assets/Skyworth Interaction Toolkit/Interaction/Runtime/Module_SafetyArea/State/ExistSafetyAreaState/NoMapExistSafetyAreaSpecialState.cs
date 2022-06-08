using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.SDK;
using UnityEngine;

public class NoMapExistSafetyAreaSpecialState : AbstractExistSafetyAreaSpecialState<SafetyAreaBase>
{
    public override void OnStateBreathe()
    {
        Debug.Log("NoMapExistSafetyAreaSpecialState OnStateBreathe");
        reference.meshRenderer.enabled = false;
        if (SafetyAreaManager.Instance.isSettingSafetyArea || SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            reference.outOfSafetyArea.SetActive(false);
            reference.nomapUI.SetActive(false);
            reference.slamLostUI.gameObject.SetActive(false);
            return;
        }

        reference.nomapUI.SetActive(true);

        int currentRelocState = SkyworthPerformance.GSXR_Get_OfflineMapRelocState();

        if (currentRelocState != 0)
        {
            reference.ChangeState(ExistSafetyAreaEnum.Normal);
            return;
        }
    }

    public override void OnStateEnter(object data)
    {
        Debug.Log("NoMapExistSafetyAreaSpecialState OnStateEnter");
        SafetyAreaManager.Instance.ExitSafetyAreaInvoke();
        if (!SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            SkyworthPerformance.GSXR_StartSeeThrough();
        }
        //API_GSXR_Slam.GSXR_Add_SlamPauseCallback(OnSlamPause);
    }

    public override void OnStateExit(object data)
    {
        Debug.Log("NoMapExistSafetyAreaSpecialState OnStateExit");
        //API_GSXR_Slam.GSXR_Remove_SlamPauseCallback(OnSlamPause);
        SkyworthPerformance.GSXR_StopSeeThrough();
        reference.nomapUI.SetActive(false);
    }

    private void OnSlamPause(bool isPause)
    {
        if (!isPause && !SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            SkyworthPerformance.GSXR_StartSeeThrough();
        }
    }
}
