using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSafetyAreaButton : MonoBehaviour
{
    public void EnableSafetyArea()
    {
        API_Module_SafetyArea.DisableSafetyAreaDisplay(false);
    }

    public void DisableSafetyArea()
    {
        API_Module_SafetyArea.DisableSafetyAreaDisplay(true);
    }
}
