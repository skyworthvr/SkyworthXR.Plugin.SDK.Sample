using UnityEngine;


public abstract class AbstractPlayAreaState<T> : IState where T : MonoBehaviour
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
