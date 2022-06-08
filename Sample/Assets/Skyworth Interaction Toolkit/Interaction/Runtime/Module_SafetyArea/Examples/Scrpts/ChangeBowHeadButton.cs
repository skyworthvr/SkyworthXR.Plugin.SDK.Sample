using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBowHeadButton : MonoBehaviour
{
    public void ShowBowHead()
    {
        API_Module_SafetyArea.SetShowAreaWhenBowHead(true);
    }

    public void HideBowHead()
    {
        API_Module_SafetyArea.SetShowAreaWhenBowHead(false);
    }
}
