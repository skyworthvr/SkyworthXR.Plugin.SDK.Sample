using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExistSafetyAreaEnum
{ 
    Normal,
    OutOfArea,
    NoMap,
    SlamLost
}

public class ExistSafetyAreaStateMachine
{
    public Dictionary<ExistSafetyAreaEnum, AbstractExistSafetyAreaSpecialState<SafetyAreaBase>> playAreaStateDic;

    private IState currentState;

    public void InitStateMachine(SafetyAreaBase safetyAreaBase)
    {
        if (playAreaStateDic == null)
        {
            playAreaStateDic = new Dictionary<ExistSafetyAreaEnum, AbstractExistSafetyAreaSpecialState<SafetyAreaBase>>();
            playAreaStateDic.Add(ExistSafetyAreaEnum.Normal, new NormalExistSafetyAreaSpecialState());
            playAreaStateDic.Add(ExistSafetyAreaEnum.OutOfArea, new OutOfExistSafetyAreaSpecialState());
            playAreaStateDic.Add(ExistSafetyAreaEnum.NoMap, new NoMapExistSafetyAreaSpecialState());
            playAreaStateDic.Add(ExistSafetyAreaEnum.SlamLost, new SlamLostExistSafetyAreaSpecialState());

            foreach (AbstractExistSafetyAreaSpecialState<SafetyAreaBase> valueItem in playAreaStateDic.Values)
            {
                valueItem.Init(safetyAreaBase);
            }
        }
    }

    public void ChangeState(ExistSafetyAreaEnum existSafetyAreaEnum, object data = null)
    {
        if (currentState != null)
        {
            currentState.OnStateExit(data);
        }

        IState newState = playAreaStateDic[existSafetyAreaEnum];
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

    public void Breathe()
    {
        if (currentState != null)
        {
            currentState.OnStateBreathe();
        }
    }
}
