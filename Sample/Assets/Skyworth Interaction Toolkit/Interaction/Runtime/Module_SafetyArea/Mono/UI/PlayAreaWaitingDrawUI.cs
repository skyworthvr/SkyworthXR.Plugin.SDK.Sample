using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayAreaWaitingDrawUI : MonoBehaviour
{
    public Action OnSwitchToStationaryAreaClick
    {
        get;
        set;
    }

    public Action OnBackClick
    {
        get;
        set;
    }

    public Button switchToStationaryAreaButton;
    public Button backButton;

    public Text waitingDrawTitleText;
    public Text waitingDrawDescribeText;
    public Text sationaryAreaBtnText;

    public void Init()
    {
        switchToStationaryAreaButton.onClick.AddListener(()=>
        {
            OnSwitchToStationaryAreaClick?.Invoke();
        });

        backButton.onClick.AddListener(()=>
        {
            OnBackClick?.Invoke();
        });

        SafetyAreaLanguageManager.Instance.onLanguageChange += ChangeLanguageText;
        ChangeLanguageText();
    }

    public void ChangeLanguageText()
    {
        waitingDrawTitleText.text = SafetyAreaLanguageManager.Instance.GetWord(106019);
        waitingDrawDescribeText.text = SafetyAreaLanguageManager.Instance.GetWord(106021);
        sationaryAreaBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(106022);
    }

    public void Release()
    {
        switchToStationaryAreaButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        SafetyAreaLanguageManager.Instance.onLanguageChange -= ChangeLanguageText;
    }
}
