using Skyworth.Interaction.SafetyArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ConfirmPlayAreaState : AbstractPlayAreaState<SafetyAreaMono>
{
    public override void OnStateEnter(object data)
    {
        SafetyAreaManager safetyAreaManager = SafetyAreaManager.Instance;
        reference.confirmPlayAreaUI.gameObject.SetActive(true);
        //reference.safetyGreyCameraUI.gameObject.SetActive(true);
        reference.confirmPlayAreaUI.Init();
        reference.confirmPlayAreaUI.OnConfirmClick += ExitSafetyAreaProcess;
        reference.confirmPlayAreaUI.OnBackClick += OnConfirmPlayAreaCancel;

        reference.safetyPlaneMono.GenerateEdgeMesh((mesh, perimeter) =>
        {
            safetyAreaManager.CreatePlayArea(mesh, reference.safetyPlaneMono.GetColorArray(), perimeter, reference.safetyPlaneMono.transform.position);
        });
        reference.safetyPlaneMono.gameObject.SetActive(false);
        safetyAreaManager.playAreaMono.SetMaterial(reference.areaConfirmMat);
    }

    public override void OnStateExit(object data)
    {        
        reference.confirmPlayAreaUI.OnConfirmClick -= ExitSafetyAreaProcess;
        reference.confirmPlayAreaUI.OnBackClick -= OnConfirmPlayAreaCancel;
        reference.confirmPlayAreaUI.Release();
        reference.confirmPlayAreaUI.gameObject.SetActive(false);
        //reference.safetyGreyCameraUI.gameObject.SetActive(false);
        if (reference.safetyPlaneMono != null)
        {
            reference.safetyPlaneMono.gameObject.SetActive(true);
        }
    }

    public override void OnStateBreathe()
    { 
        
    }

    private void OnConfirmPlayAreaCancel()
    {
        SafetyAreaManager.Instance.DestroyPlayArea();
        SafetyAreaManager.Instance.ChangeStep(SafetyAreaStepEnum.PlayArea);
    }

    private void ExitSafetyAreaProcess()
    {
        SafetyAreaManager.Instance.playAreaMono.SetMaterial(reference.areaNormalMat);
        reference.DestroySafetyPlane();
        SafetyAreaManager.Instance.ExitSafeAreaStep();
    }
}
