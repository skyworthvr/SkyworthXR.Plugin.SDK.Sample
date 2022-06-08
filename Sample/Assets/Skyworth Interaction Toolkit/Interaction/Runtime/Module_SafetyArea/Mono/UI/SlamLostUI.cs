using Skyworth.Interaction.SafetyArea;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlamLostUI : MonoBehaviour
{
    public Button safetyAreaButton;

    public Text outOfSafetyAreaText;
    public Text createSafetyAreaText;

    // Start is called before the first frame update
    private void Start()
    {
        safetyAreaButton.onClick.AddListener(OnSafetyAreaButtonClick);
    }

    private void OnDestroy()
    {
        safetyAreaButton.onClick.RemoveListener(OnSafetyAreaButtonClick);
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


    public void ChangeLanguageText()
    {
        outOfSafetyAreaText.text = SafetyAreaLanguageManager.Instance.GetWord(106039);
        createSafetyAreaText.text = SafetyAreaLanguageManager.Instance.GetWord(106038);
    }

    private void OnSafetyAreaButtonClick()
    {
        SafetyAreaManager.Instance.StartSetSafetyArea();
    }
}
