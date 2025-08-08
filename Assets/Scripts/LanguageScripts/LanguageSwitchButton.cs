using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitchButton : MonoBehaviour
{
    [SerializeField] private LanguageManager.Language language;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SwitchLanguage);
    }

    private void SwitchLanguage()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.SetLanguage(language);
        }
        else
        {
            Debug.LogError("LanguageManager instance not found!");
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(SwitchLanguage);
    }
}