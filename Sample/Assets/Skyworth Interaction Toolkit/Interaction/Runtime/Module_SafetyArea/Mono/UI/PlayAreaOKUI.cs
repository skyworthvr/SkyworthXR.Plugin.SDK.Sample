using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayAreaOKUI : MonoBehaviour
{
    public Action OnRedrawAreaClick
    {
        get;
        set;
    }

    public Action OnContinueClick
    {
        get;
        set;
    }

    public Action OnBackClick
    {
        get;
        set;
    }

    public Button redrawAreaButton;
    public Button continueButton;
    public Button backButton;

    public Text playAreaOKText;
    public Text playAreaOKDescribeText;
    public Text continueButtonText;
    public Text redrawButtonText;

    public void Init()
    {
        redrawAreaButton.onClick.AddListener(() =>
        {
            OnRedrawAreaClick?.Invoke();
        });

        continueButton.onClick.AddListener(() =>
        {
            OnContinueClick?.Invoke();
        });

        backButton.onClick.AddListener(() =>
        {
            OnBackClick?.Invoke();
        });

        SafetyAreaLanguageManager.Instance.onLanguageChange += ChangeLanguageText;
        ChangeLanguageText();
    }


    public void ChangeLanguageText()
    {
        playAreaOKText.text = SafetyAreaLanguageManager.Instance.GetWord(106033);
        playAreaOKDescribeText.text = SafetyAreaLanguageManager.Instance.GetWord(106034);
        continueButtonText.text = SafetyAreaLanguageManager.Instance.GetWord(10092);
        redrawButtonText.text = SafetyAreaLanguageManager.Instance.GetWord(106027);
    }

    public void Release()
    {
        redrawAreaButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        SafetyAreaLanguageManager.Instance.onLanguageChange -= ChangeLanguageText;
    }
}
