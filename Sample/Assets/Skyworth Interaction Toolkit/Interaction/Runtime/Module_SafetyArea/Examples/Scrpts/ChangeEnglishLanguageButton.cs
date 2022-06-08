using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeEnglishLanguageButton : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnButtonClick();
        }
    }

    public void OnButtonClick()
    {
        SafetyAreaLanguageManager.Instance.ChangeLanguage(SafetyAreaLanguageEnum.English);
    }
}
