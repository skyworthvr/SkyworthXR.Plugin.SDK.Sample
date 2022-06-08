using Skyworth.Interaction.SafetyArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum PlayAreaStateEnum
{ 
    WaitingDraw,
    NotEnough,
    OK,
    ConfirmPlayArea,
}

public class PlayAreaStateMachine
{
    public Dictionary<PlayAreaStateEnum, AbstractPlayAreaState<SafetyAreaMono>> playAreaStateDic;

    private IState currentState;

    public void InitStateMachine(SafetyAreaMono safetyAreaMono)
    {
        if (playAreaStateDic == null)
        {
            playAreaStateDic = new Dictionary<PlayAreaStateEnum, AbstractPlayAreaState<SafetyAreaMono>>();
            playAreaStateDic.Add(PlayAreaStateEnum.WaitingDraw, new PrepareDrawPlayAreaState());
            playAreaStateDic.Add(PlayAreaStateEnum.OK, new PlayAreaOKState());
            playAreaStateDic.Add(PlayAreaStateEnum.NotEnough, new PlayAreaNotEnoughState());
            playAreaStateDic.Add(PlayAreaStateEnum.ConfirmPlayArea, new ConfirmPlayAreaState());

            foreach (AbstractPlayAreaState<SafetyAreaMono> valueItem in playAreaStateDic.Values)
            {
                valueItem.Init(safetyAreaMono);
            }
        }
    }

    public void ChangeState(PlayAreaStateEnum playAreaStateEnum, object data = null)
    {
        if (currentState != null)
        {
            currentState.OnStateExit(data);
        }

        IState newState = playAreaStateDic[playAreaStateEnum];
        newState.OnStateEnter(data);
        currentState = newState;
    }

    public void ExitCurrentState(object data = null)
    {
        if (currentState != null)
        {
            currentState.OnStateExit(data);
        }
        currentState = null;
    }
}
