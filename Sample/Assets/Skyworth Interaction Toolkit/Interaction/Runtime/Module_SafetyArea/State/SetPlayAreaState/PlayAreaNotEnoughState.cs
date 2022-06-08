using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAreaNotEnoughState : AbstractPlayAreaState<SafetyAreaMono>
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

        ShowPlayAreaNotEnoughUI();
        reference.playAreaNotEnoughUI.OnRedrawAreaClick += ChangePrepareDrawPlayAreaState;
        reference.playAreaNotEnoughUI.OnBackClick += ChangePrepareDrawPlayAreaState;
        reference.playAreaNotEnoughUI.OnSwitchToStationaryAreaClick += SwitchToStationaryAreaStep;
        //FillIndices(null);

        reference.safetyPlaneMono.RegistPointerUpEvent(FillIndices);
    }

    public override void OnStateExit(object data)
    {
        reference.safetyPlaneMono.UnRegistPointerUpEvent(FillIndices);
        reference.playAreaNotEnoughUI.OnRedrawAreaClick -= ChangePrepareDrawPlayAreaState;
        reference.playAreaNotEnoughUI.OnBackClick -= ChangePrepareDrawPlayAreaState;
        reference.playAreaNotEnoughUI.OnSwitchToStationaryAreaClick -= SwitchToStationaryAreaStep;
        HidePlayAreaNotEnougUI();

        HidePlayAreaNotEnougUI();

        if (reference.safetyPlaneMono != null)
        {
            reference.safetyPlaneMono.UnRegistPointerDownFillEvent();
            reference.safetyPlaneMono.UnRegistPointerUpFillEvent();
        }  
    }

    public override void OnStateBreathe()
    {

    }

    private void FillIndices(PointerEventData pointerEventData)
    {
        reference.safetyPlaneMono.FillIndices();
        if (reference.safetyPlaneMono.CheckIndicesEnough())
        {
            playAreaStep.ChangePlayAreaState(PlayAreaStateEnum.OK);
        }
    }

    private void SwitchToStationaryAreaStep()
    {
        SafetyAreaManager.Instance.ChangeStep(SafetyAreaStepEnum.StationaryArea);
    }

    private void ChangePrepareDrawPlayAreaState()
    {
        reference.safetyPlaneMono.ClearAllMeshColor();
        playAreaStep.ChangePlayAreaState(PlayAreaStateEnum.WaitingDraw);
    }


    private void ShowPlayAreaNotEnoughUI()
    {
        reference.playAreaNotEnoughUI.gameObject.SetActive(true);
        reference.playAreaNotEnoughUI.Init();
    }

    private void HidePlayAreaNotEnougUI()
    {
        reference.playAreaNotEnoughUI.Release();
        reference.playAreaNotEnoughUI.gameObject.SetActive(false);
    }
}
