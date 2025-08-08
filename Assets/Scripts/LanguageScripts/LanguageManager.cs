using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public enum Language
    {
        Russian,
        English
    }
    public static event Action<Language> OnLanguageChanged; // Событие при смене языка

    private Language currentLanguage;
    private List<LocalizedText> localizedTexts = new List<LocalizedText>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLanguage()
    {
        if (PlayerPrefs.HasKey("GameLanguage"))
        {
            currentLanguage = (Language)PlayerPrefs.GetInt("GameLanguage");
        }
        else
        {
            if (Application.systemLanguage == SystemLanguage.Russian ||
                Application.systemLanguage == SystemLanguage.Ukrainian ||
                Application.systemLanguage == SystemLanguage.Belarusian)
            {
                currentLanguage = Language.Russian;
            }
            else
            {
                currentLanguage = Language.English;
            }
        }
    }

    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        PlayerPrefs.SetInt("GameLanguage", (int)language);
        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke(currentLanguage); // Уведомляем подписчиков
    }

    public Language GetCurrentLanguage() => currentLanguage;

    public void RegisterText(LocalizedText text)
    {
        if (!localizedTexts.Contains(text))
        {
            localizedTexts.Add(text);
            text.UpdateTextContent();
        }
    }

    public void UnregisterText(LocalizedText text)
    {
        if (localizedTexts.Contains(text))
        {
            localizedTexts.Remove(text);
        }
    }

    private void UpdateAllTexts()
    {
        foreach (LocalizedText text in localizedTexts)
        {
            text.UpdateTextContent();
        }
    }

    public void SetRussian()
    {
        SetLanguage(Language.Russian);
    }

    public void SetEnglish()
    {
        SetLanguage(Language.English);
    }
}