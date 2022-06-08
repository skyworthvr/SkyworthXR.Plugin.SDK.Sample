using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractExistSafetyAreaSpecialState<T> : IState where T : SafetyAreaBase
{
    protected T reference;

    public void Init(T reference)
    {
        this.reference = reference;
    }

    public abstract void OnStateEnter(object data);

    public abstract void OnStateExit(object data);

    public abstract void OnStateBreathe();
}
    
