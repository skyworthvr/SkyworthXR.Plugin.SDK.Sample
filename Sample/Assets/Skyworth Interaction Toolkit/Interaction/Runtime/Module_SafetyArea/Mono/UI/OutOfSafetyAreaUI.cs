using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutOfSafetyAreaUI : MonoBehaviour
{
    public Button safetyAreaButton;
    public Button stationaryAreaButton;

    public Text outOfSafetyAreaTitleText;
    public Text outOfSafetyAreaDescribeText;
    public Text safetyPlayAreaBtnText;
    public Text stationaryAreaBtnText;

    // Start is called before the first frame update
    private void Start()
    {
        safetyAreaButton.onClick.AddListener(OnSafetyAreaButtonClick);
        stationaryAreaButton.onClick.AddListener(OnStationaryAreaButtonClick);
    }

    private void OnDestroy()
    {
        safetyAreaButton.onClick.RemoveListener(OnSafetyAreaButtonClick);
        stationaryAreaButton.onClick.RemoveListener(OnStationaryAreaButtonClick);
    }

    private void OnEnable()
    {
        SafetyAreaManager.Instance.ExitSafetyAreaInvoke();//OnEnterSafetyArea?.Invoke();

        SafetyAreaLanguageManager.Instance.onLanguageChange += ChangeLanguageText;
        ChangeLanguageText();
    }

    private void OnDisable()
    {
        SafetyAreaManager.Instance.EnterSafetyAreaInvoke();//OnExitSafetyArea?.Invoke();

        SafetyAreaLanguageManager.Instance.onLanguageChange -= ChangeLanguageText;
    }

    private void OnSafetyAreaButtonClick()
    {
        SafetyAreaManager.Instance.StartSetSafetyArea();
    }

    private void OnStationaryAreaButtonClick()
    {
        SafetyAreaManager.Instance.StartSetStationaryArea();
    }

    public void ChangeLanguageText()
    {
        outOfSafetyAreaTitleText.text = SafetyAreaLanguageManager.Instance.GetWord(106029);
        outOfSafetyAreaDescribeText.text = SafetyAreaLanguageManager.Instance.GetWord(106030);
        safetyPlayAreaBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(106028);
        stationaryAreaBtnText.text = SafetyAreaLanguageManager.Instance.GetWord(106022);
    }
}
