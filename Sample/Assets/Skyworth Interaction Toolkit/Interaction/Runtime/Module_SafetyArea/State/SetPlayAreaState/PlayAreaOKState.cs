using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAreaOKState : AbstractPlayAreaState<SafetyAreaMono>
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

        ShowPlayAreaOKUI();
        reference.playAreaOKUI.OnRedrawAreaClick += ChangePrepareDrawPlayAreaState;
        reference.playAreaOKUI.OnBackClick += ChangePrepareDrawPlayAreaState;
        reference.playAreaOKUI.OnContinueClick += SwitchToConfirmPlayAreaStep;
        //FillIndices(null);

        reference.safetyPlaneMono.RegistPointerUpEvent(FillIndices);
    }

    public override void OnStateExit(object data)
    {
        reference.safetyPlaneMono.UnRegistPointerUpEvent(FillIndices);
        reference.playAreaOKUI.OnRedrawAreaClick -= ChangePrepareDrawPlayAreaState;
        reference.playAreaOKUI.OnBackClick -= ChangePrepareDrawPlayAreaState;
        reference.playAreaOKUI.OnContinueClick -= SwitchToConfirmPlayAreaStep;
        HidePlayAreaOKUI();

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

    private void FillIndices(PointerEventData pointerEventData)
    {
        reference.safetyPlaneMono.FillIndices();
    }

    private void ChangePrepareDrawPlayAreaState()
    {
        reference.safetyPlaneMono.ClearAllMeshColor();
        playAreaStep.ChangePlayAreaState(PlayAreaStateEnum.WaitingDraw);
    }

    private void SwitchToConfirmPlayAreaStep()
    {
        playAreaStep.ChangePlayAreaState(PlayAreaStateEnum.ConfirmPlayArea);
    }

    private void ShowPlayAreaOKUI()
    {
        reference.playAreaOKUI.gameObject.SetActive(true);
        reference.playAreaOKUI.Init();
    }

    private void HidePlayAreaOKUI()
    {
        reference.playAreaOKUI.Release();
        reference.playAreaOKUI.gameObject.SetActive(false);
    }
}
