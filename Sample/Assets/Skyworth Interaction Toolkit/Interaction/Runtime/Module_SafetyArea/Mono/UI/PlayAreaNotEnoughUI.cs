using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayAreaNotEnoughUI : MonoBehaviour
{
    public Action OnSwitchToStationaryAreaClick
    {
        get;
        set;
    }

    public Action OnRedrawAreaClick
    {
        get;
        set;
    }

    public Action OnBackClick
    {
        get;
        set;
    }

    public Button redrawButton;
    public Button switchToStationaryAreaButton;
    public Button backButton;

    public Text playAreaNotEnoughTitleText;
    public Text playAreaNotEnoughDescribeText;
    public Text redrawBtnText;
    public Text stationaryAreaBtnText;

    public void Init()
    {
        redrawButton.onClick.AddListener(()=>
        {
            OnRedrawAreaClick();
        });

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
        playAreaNotEnoughTitleText.text = SafetyAreaLanguageManager.Instance.GetWord(106023);
        playAreaNotEnoughDescribeText.text = SafetyAreaLanguageManager.Instance.GetWord(106024);
        redrawBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(106027);
        stationaryAreaBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(106022);
    }

    public void Release()
    {
        redrawButton.onClick.RemoveAllListeners();
        switchToStationaryAreaButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        SafetyAreaLanguageManager.Instance.onLanguageChange -= ChangeLanguageText;
    }
}
