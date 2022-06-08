using System;
using UnityEngine;
using UnityEngine.UI;

public class GroundHeightUI : MonoBehaviour
{
    public Action OnConfirmClick
    {
        get;
        set;
    }

    public Action OnCancelClick
    {
        get;
        set;
    }

    public Action OnResetClick
    {
        get;
        set;
    }

    public Button confirmButton;
    public Button cancelButton;
    public Button resetButton;

    public Text confirmGroundHeightTitleText;
    public Text confirmGroundHeightDescribeText;
    public Text resetBtnText;
    public Text confirmBtnText;

    public void Init()
    {
        confirmButton.onClick.AddListener(() =>
        {
            OnConfirmClick?.Invoke();
        });

        cancelButton.onClick.AddListener(() =>
        {
            OnCancelClick?.Invoke();
        });

        resetButton.onClick.AddListener(() =>
        {
            OnResetClick?.Invoke();
        });

        SafetyAreaLanguageManager.Instance.onLanguageChange += ChangeLanguageText;
        ChangeLanguageText();
    }

    public void ChangeLanguageText()
    {
        confirmGroundHeightTitleText.text = SafetyAreaLanguageManager.Instance.GetWord(106018);
        confirmGroundHeightDescribeText.text = SafetyAreaLanguageManager.Instance.GetWord(106020);
        resetBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(10081);
        confirmBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(106018);
    }


    public void Release()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();
        SafetyAreaLanguageManager.Instance.onLanguageChange -= ChangeLanguageText;
    }
}
