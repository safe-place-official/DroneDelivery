using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    [TextArea] public string russianText;
    [TextArea] public string englishText;

    public Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<Text>();

        // Подписываемся на событие изменения языка
        LanguageManager.OnLanguageChanged += HandleLanguageChanged;

        // Первоначальная установка текста
        UpdateTextContent();
    }

    private void OnDestroy()
    {
        // Отписываемся при уничтожении объекта
        LanguageManager.OnLanguageChanged -= HandleLanguageChanged;
    }

    private void HandleLanguageChanged(LanguageManager.Language newLanguage)
    {
        UpdateTextContent();
    }

    public void UpdateTextContent()
    {
        if (textComponent == null || LanguageManager.Instance == null) return;

        switch (LanguageManager.Instance.GetCurrentLanguage())
        {
            case LanguageManager.Language.Russian:
                textComponent.text = russianText;
                break;
            case LanguageManager.Language.English:
                textComponent.text = englishText;
                break;
        }
    }
}