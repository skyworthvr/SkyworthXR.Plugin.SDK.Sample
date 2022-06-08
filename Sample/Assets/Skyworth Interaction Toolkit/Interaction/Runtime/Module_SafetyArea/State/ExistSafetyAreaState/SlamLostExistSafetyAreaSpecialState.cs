using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.SDK;
using UnityEngine;

public class SlamLostExistSafetyAreaSpecialState : AbstractExistSafetyAreaSpecialState<SafetyAreaBase>
{
    private float timer = 0f;

    public override void OnStateBreathe()
    {
        Debug.Log("SlamLostExistSafetyAreaSpecialState OnStateBreathe");
        reference.meshRenderer.enabled = false;
        if (SafetyAreaManager.Instance.isSettingSafetyArea || SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            reference.outOfSafetyArea.SetActive(false);
            reference.nomapUI.SetActive(false);
            reference.slamLostUI.gameObject.SetActive(false);
            return;
        }

        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            reference.slamLostUI.gameObject.SetActive(true);
        }
    }

    private void DelayShowUI()
    {

    }

    public override void OnStateEnter(object data)
    {
        Debug.Log("SlamLostExistSafetyAreaSpecialState OnStateEnter");
        SafetyAreaManager.Instance.ExitSafetyAreaInvoke();
        //API_GSXR_Slam.GSXR_Add_SlamPauseCallback(OnSlamPause);
        SkyworthPerformance.GSXR_ResaveMap("resetslam");
        if (!SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            SkyworthPerformance.GSXR_StartSeeThrough();
        }
        timer = 0f;
    }

    public override void OnStateExit(object data)
    {
        Debug.Log("SlamLostExistSafetyAreaSpecialState OnStateExit");
        //API_GSXR_Slam.GSXR_Remove_SlamPauseCallback(OnSlamPause);
        SkyworthPerformance.GSXR_StopSeeThrough();
    }

    private void OnSlamPause(bool isPause)
    {

        if (!isPause && !SafetyAreaManager.Instance.isDisableSafetyArea)
        {
            SkyworthPerformance.GSXR_StartSeeThrough();
        }

    }
}
