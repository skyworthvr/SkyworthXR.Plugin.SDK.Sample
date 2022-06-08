using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeChineseLanguageButton : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnButtonClick();
        }
    }

    public void OnButtonClick()
    {
        SafetyAreaLanguageManager.Instance.ChangeLanguage(SafetyAreaLanguageEnum.Chinese);
    }
}
