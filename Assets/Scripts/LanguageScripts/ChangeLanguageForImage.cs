using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguageForImage : MonoBehaviour
{
    public Image ControlImage;

    public Sprite RuText;
    public Sprite EngText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
            ControlImage.sprite = RuText;
        else ControlImage.sprite = EngText;
    }
}
