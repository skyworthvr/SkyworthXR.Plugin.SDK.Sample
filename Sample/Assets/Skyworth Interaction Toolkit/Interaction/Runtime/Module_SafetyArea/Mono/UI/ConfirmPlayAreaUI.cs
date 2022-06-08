using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPlayAreaUI : MonoBehaviour
{
    public Action OnConfirmClick
    {
        get;
        set;
    }

    public Action OnBackClick
    {
        get;
        set;
    }

    public Button confirmClick;
    public Button backClick;

    public Text confirmSafetyAreaTitleText;
    public Text confirmSafetyAreaDescribeText;
    public Text confirmBtnText;

    public void Init()
    {
        confirmClick.onClick.AddListener(() =>
        {
            OnConfirmClick?.Invoke();
        });

        backClick.onClick.AddListener(()=>
        {
            OnBackClick?.Invoke();
        });

        SafetyAreaLanguageManager.Instance.onLanguageChange += ChangeLanguageText;
        ChangeLanguageText();
    }

    public void ChangeLanguageText()
    {
        confirmSafetyAreaTitleText.text = SafetyAreaLanguageManager.Instance.GetWord(106031);
        confirmSafetyAreaDescribeText.text = SafetyAreaLanguageManager.Instance.GetWord(106036);
        confirmBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(10092);
    }

    public void Release()
    {
        confirmClick.onClick.RemoveAllListeners();
        backClick.onClick.RemoveAllListeners();

        SafetyAreaLanguageManager.Instance.onLanguageChange -= ChangeLanguageText;
    }
}
