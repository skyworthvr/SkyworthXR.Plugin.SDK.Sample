using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISafetyAreaStep
{
    void OnEnterStep();
    void OnExitStep();

    SafetyAreaStepEnum GetStepEnum();
}
