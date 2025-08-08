using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    public GameObject controlPanel;
    public GameObject settingsPanel;
    public GameObject stylePanel;
    public GameObject mainButtonsPanel;
    public GameObject MainMenu;

    public Image ControlImage;
    public Image NotationsImage;

    public Sprite NotationsRu;
    public Sprite NotationsEng;

    public Sprite PCControlRu;
    public Sprite PCControlEng;

    public Sprite MobileControlRu;
    public Sprite MobileControlEng;

    public Sprite PSControlRu;
    public Sprite PSControlEng;

    public Sprite XBoxControlRu;
    public Sprite XBoxControlEng;

    public Image fadeImage; // Ссылка на Image, которое будет использоваться для затемнения
    public float fadeDuration = 1f; // Длительность затемнения/осветления


    public System.Collections.IEnumerator BrightnessScreen()
    {
        AudioListener.volume = 1f;
        float localTimer = 0;
        while (localTimer < fadeDuration)
        {
            localTimer += Time.deltaTime;
            float alpha = Mathf.SmoothStep(1, 0, localTimer / fadeDuration); // Плавно убираем затемнение (1 → 0)
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null; // Ждем следующий кадр
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    void Start()
    {
        StartCoroutine(BrightnessScreen()); // Запускаем корутину
    }

    public System.Collections.IEnumerator DarknessScreen()
    {
        float localTimer = 0;
        float initialVolume = AudioListener.volume;

        while (localTimer < fadeDuration)
        {
            localTimer += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0, 1, localTimer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);

            // Плавно уменьшаем громкость от текущего значения до 0
            AudioListener.volume = Mathf.Lerp(initialVolume, 0f, localTimer / fadeDuration);

            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
        AudioListener.volume = 0f; // Убедимся, что достигли 0
    }

    public void ChangeScenes(int numberScenes)
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        if (!PlayerPrefs.HasKey("HasLaunchedBefore"))
        {
            // Первый запуск
            PlayerPrefs.SetInt("HasLaunchedBefore", 1);
            PlayerPrefs.Save();
            numberScenes = 2;
        }

        StartCoroutine(ChangeScenesCoroutine(numberScenes));
    }

    public System.Collections.IEnumerator ChangeScenesCoroutine(int numberScenes)
    {
        yield return StartCoroutine(DarknessScreen()); // Ждем завершения затемнения
        SceneManager.LoadScene(numberScenes);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OpenSettingsMenu()
    {
        settingsPanel.SetActive(true);
        mainButtonsPanel.SetActive(false);
    }
    public void CloseSettingsMenu()
    {
        mainButtonsPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenControlMenu()
    {
        controlPanel.SetActive(true);
        mainButtonsPanel.SetActive(false);
    }

    public void CloseControlMenu()
    {
        mainButtonsPanel.SetActive(true);
        controlPanel.SetActive(false);
    }

    public void OpenStyleMenu()
    {
        stylePanel.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void CloseStyleMenu()
    {
        MainMenu.SetActive(true);
        stylePanel.SetActive(false);
    }

    private void Update()
    {
        if (ControlImage != null)
        {
            if (!Application.isMobilePlatform)
            {
                if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
                    ControlImage.sprite = PCControlRu;
                else ControlImage.sprite = PCControlEng;

                if (Gamepad.current is XInputController)
                {
                    if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
                        ControlImage.sprite = XBoxControlRu;
                    else ControlImage.sprite = XBoxControlEng;
                }

                else if (Gamepad.current is DualShockGamepad)
                {
                    if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
                        ControlImage.sprite = PSControlRu;
                    else ControlImage.sprite = PSControlEng;
                }
            }

            else
            {
                if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
                    ControlImage.sprite = MobileControlRu;
                else ControlImage.sprite = MobileControlEng;

                if (Gamepad.current is XInputController)
                {
                    if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
                        ControlImage.sprite = XBoxControlRu;
                    else ControlImage.sprite = XBoxControlEng;
                }

                else if (Gamepad.current is DualShockGamepad)
                {
                    if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
                        ControlImage.sprite = PSControlRu;
                    else ControlImage.sprite = PSControlEng;
                }
            }
        }

        if (NotationsImage != null)
        {
            if (PlayerPrefs.GetInt("GameLanguage") == (int)LanguageManager.Language.Russian)
                NotationsImage.sprite = NotationsRu;
            else NotationsImage.sprite = NotationsEng;
        }
    }
}