using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeController : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;  // Перетащите сюда AudioSource

    [Header("Save Settings")]
    public string saveKey; // Уникальный ключ для сохранения

    [Header("UI Settings")]
    public Text volumeText;    // Текст для отображения (например, "Музыка 50%")
    public string volumeLabelRussian;
    public string volumeLabelEnglish;
    public Button turnUpVolumeButton;         // Кнопка "+"
    public Button turnDownVolumeButton;        // Кнопка "-"

    private Slider slider;
    private const float Step = 0.1f;     // Шаг изменения кнопками

    public int percent;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        // Настройка слайдера (0..1)
        slider.minValue = 0f;
        slider.maxValue = 1f;

        // Загрузка сохранённой громкости (если нет — используется текущая громкость AudioSource)
        float savedVolume = PlayerPrefs.GetFloat(saveKey, audioSource.volume);
        SetVolume(savedVolume, false); // false — не сохраняем (чтобы не перезаписать)

        // Подписка на события
        slider.onValueChanged.AddListener((value) => SetVolume(value, true));

        if (turnUpVolumeButton != null)
            turnUpVolumeButton.onClick.AddListener(() => ChangeVolume(Step));

        if (turnDownVolumeButton != null)
            turnDownVolumeButton.onClick.AddListener(() => ChangeVolume(-Step));
    }

    // Установка громкости (saveToPlayerPrefs — нужно ли сохранять)
    public void SetVolume(float value, bool saveToPlayerPrefs = true)
    {
        value = Mathf.Clamp(value, 0f, 1f);
        slider.value = value;
        audioSource.volume = value;

        // Сохранение (если нужно)
        if (saveToPlayerPrefs)
        {
            PlayerPrefs.SetFloat(saveKey, value);
        }

        // Обновление текста (например, "Музыка 70%")
        if (volumeText != null)
        {
            percent = Mathf.RoundToInt(value * 100);
            UpdateText(LanguageManager.Instance.GetCurrentLanguage());
        }
    }

    public void UpdateText(LanguageManager.Language language)
    {
        string displayText = language == LanguageManager.Language.Russian
            ? volumeLabelRussian
            : volumeLabelEnglish;

        // Динамическое значение (например: "Цена: 100" / "Price: 100")
        volumeText.text = string.Format($"{displayText} {percent}%");
    }


    // Изменение громкости на шаг (+0.1 или -0.1)
    public void ChangeVolume(float step)
    {
        float newValue = audioSource.volume + step;
        SetVolume(newValue);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save(); // Сохраняем настройки
    }

    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateText;
        UpdateText(LanguageManager.Instance.GetCurrentLanguage());
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateText;
    }

}