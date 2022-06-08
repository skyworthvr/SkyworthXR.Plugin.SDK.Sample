using System;
using UnityEngine;
using UnityEngine.UI;

public class StationaryAreaUI : MonoBehaviour
{
    public Action OnSwitchToPlayAreaClick
    {
        get;
        set;
    }

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

    public Button switchToPlayAreaButton;
    public Button confirmButton;
    public Button cancelButton;

    public Text confirmStationaryAreaTitleText;
    public Text confirmStationaryAreaDescribeText;
    public Text confirmBtnText;
    public Text redrawPlayAreaBtnText;

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

        switchToPlayAreaButton.onClick.AddListener(() =>
        {
            OnSwitchToPlayAreaClick?.Invoke();
        });

        SafetyAreaLanguageManager.Instance.onLanguageChange += ChangeLanguageText;
        ChangeLanguageText();
    }

    public void ChangeLanguageText()
    {
        confirmStationaryAreaTitleText.text = SafetyAreaLanguageManager.Instance.GetWord(106025);
        confirmStationaryAreaDescribeText.text = SafetyAreaLanguageManager.Instance.GetWord(106026);
        confirmBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(10092);
        redrawPlayAreaBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(106035);
    }

    public void Release()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        switchToPlayAreaButton.onClick.RemoveAllListeners();
        SafetyAreaLanguageManager.Instance.onLanguageChange -= ChangeLanguageText;
    }
}
