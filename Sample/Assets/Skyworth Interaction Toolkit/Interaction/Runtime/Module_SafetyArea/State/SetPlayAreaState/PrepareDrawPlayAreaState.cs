using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrepareDrawPlayAreaState : AbstractPlayAreaState<SafetyAreaMono>
{
    private PlayAreaStep playAreaStep;

    public override void OnStateEnter(object data)
    {
        if (playAreaStep == null)
        {
            playAreaStep = SafetyAreaManager.Instance.GetStep<PlayAreaStep>(SafetyAreaStepEnum.PlayArea);
        }

        reference.safetyPlaneMono.RegistPointerDownFillEvent();
        reference.safetyPlaneMono.RegistPointerUpFillEvent();

        ShowPlayAreaWaitingDrawUI();
        reference.safetyPlaneMono.RegistPointerUpEvent(FillIndices);
        reference.playAreaWaitingDrawUI.OnSwitchToStationaryAreaClick += SwitchToStationaryAreaStep;
        reference.playAreaWaitingDrawUI.OnBackClick += SwitchToGroundHeightStep;
    }

    public override void OnStateExit(object data)
    {
        reference.playAreaWaitingDrawUI.OnSwitchToStationaryAreaClick -= SwitchToStationaryAreaStep;
        reference.playAreaWaitingDrawUI.OnBackClick -= SwitchToGroundHeightStep;
        reference.safetyPlaneMono.UnRegistPointerUpEvent(FillIndices);
        HidePlayAreaWaitingDrawUI();
        

        //HidePlane();
        if (reference.safetyPlaneMono != null)
        {
            reference.safetyPlaneMono.UnRegistPointerDownFillEvent();
            reference.safetyPlaneMono.UnRegistPointerUpFillEvent();
        }
    }

    public override void OnStateBreathe()
    {

    }

    private void SwitchToStationaryAreaStep()
    {
        SafetyAreaManager.Instance.ChangeStep(SafetyAreaStepEnum.StationaryArea);
    }

    private void SwitchToGroundHeightStep()
    {
        SafetyAreaManager.Instance.ChangeStep(SafetyAreaStepEnum.GroundHeight);
    }

    public void ShowPlayAreaWaitingDrawUI()
    {
        reference.playAreaWaitingDrawUI.gameObject.SetActive(true);
        reference.playAreaWaitingDrawUI.Init();       
    }

    public void HidePlayAreaWaitingDrawUI()
    {
        reference.playAreaWaitingDrawUI.Release();
        reference.playAreaWaitingDrawUI.gameObject.SetActive(false);
    }

    private void FillIndices(PointerEventData pointerEventData)
    {
        reference.safetyPlaneMono.FillIndices();
        if (reference.safetyPlaneMono.CheckIndicesEnough())
        {
            playAreaStep.ChangePlayAreaState(PlayAreaStateEnum.OK);
        }
        else
        {
            playAreaStep.ChangePlayAreaState(PlayAreaStateEnum.NotEnough);
        }       
    }
}
