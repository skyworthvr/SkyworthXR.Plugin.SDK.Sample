using Skyworth.Interaction.SafetyArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayAreaStep : AbstractSafetyAreaStep<SafetyAreaMono>
{
    private PlayAreaStateMachine playAreaStateMachine;

    public PlayAreaStep(SafetyAreaMono safetyAreaMono) : base(safetyAreaMono)
    {

        if (playAreaStateMachine == null)
        {
            playAreaStateMachine = new PlayAreaStateMachine();
            playAreaStateMachine.InitStateMachine(safetyAreaMono);
        }
    }

    public override void OnEnterStep()
    {
        //reference.safetyGreyCameraUI.gameObject.SetActive(true);
        ShowPlane();
        ClearPlaneColor();
        ChangePlayAreaState(PlayAreaStateEnum.WaitingDraw);
    }

    public override void OnExitStep()
    {
        if (playAreaStateMachine != null)
        {
            playAreaStateMachine.ExitCurrentState();
        }
        //reference.safetyGreyCameraUI.gameObject.SetActive(false);
    }

    public override SafetyAreaStepEnum GetStepEnum()
    {
        return SafetyAreaStepEnum.PlayArea;
    }


    //切换游戏区域步骤的状态
    public void ChangePlayAreaState(PlayAreaStateEnum playAreaStateEnum, object data = null)
    {
        if (playAreaStateMachine != null)
        {
            playAreaStateMachine.ChangeState(playAreaStateEnum, data);
        }
    }

    //切换一次平面位置
    public void ChangePlanePosition()
    {
        reference.safetyPlaneMono.ContinueSetPlaneHeight();
    }

    //显示平面
    private void ShowPlane()
    {
        reference.safetyPlaneMono.gameObject.SetActive(true);
    }

    //清空平面颜色
    private void ClearPlaneColor()
    {
        reference.safetyPlaneMono.ClearAllMeshColor();
    }
}