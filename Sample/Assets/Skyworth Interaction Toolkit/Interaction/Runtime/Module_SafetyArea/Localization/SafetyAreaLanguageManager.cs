using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using SC.XR.Unity;

public class SafetyAreaLanguageManager : Singleton<SafetyAreaLanguageManager>
{
    public Action onLanguageChange;

    private const string SAFETY_AREA_LANGUAGE_SAVE_KEY = "SAFETY_AREA_LANGUAGE_SAVE_KEY";
    private const string LANGUAGE_FILE_PATH = "Localization/Language";
    private SafetyAreaLanguageList safetyAreaLanguageList;
    private SafetyAreaLanguageEnum currentLanguageEnum = SafetyAreaLanguageEnum.Chinese;

    public void Init()
    {
        TextAsset languageText = Resources.Load<TextAsset>(LANGUAGE_FILE_PATH);
        safetyAreaLanguageList = JsonUtility.FromJson<SafetyAreaLanguageList>(languageText.text);
        LoadLanguage();
    }

    public void ChangeLanguage(SafetyAreaLanguageEnum safetyAreaLanguageEnum)
    {
        currentLanguageEnum = safetyAreaLanguageEnum;
        PlayerPrefs.SetInt(SAFETY_AREA_LANGUAGE_SAVE_KEY, (int)currentLanguageEnum);
        PlayerPrefs.Save();

        onLanguageChange?.Invoke();
    }

    private void LoadLanguage()
    {
        currentLanguageEnum = (SafetyAreaLanguageEnum)PlayerPrefs.GetInt(SAFETY_AREA_LANGUAGE_SAVE_KEY, 0);
    }

    public string GetWord(int id)
    {
        List<SafetyAreaLanguageItem> languageItemList = null;
        switch (currentLanguageEnum)
        {
            case SafetyAreaLanguageEnum.Chinese:
                languageItemList = safetyAreaLanguageList.chinese;
                break;
            case SafetyAreaLanguageEnum.English:
                languageItemList = safetyAreaLanguageList.english;
                break;
        }

        if (languageItemList == null)
        {
            Debug.LogError("GetWord languageItemList == null");
            return string.Empty;
        }

        SafetyAreaLanguageItem safetyAreaLanguageItem = languageItemList.Where((item) => item.id == id).FirstOrDefault();
        if (safetyAreaLanguageItem == null)
        {
            Debug.LogError("GetWord safetyAreaLanguageItem == null id:" + id + " not exist");
            return string.Empty;
        }
        return safetyAreaLanguageItem.value;
    }
}
