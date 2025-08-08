using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComicCutscene : MonoBehaviour
{
    [Header("Настройки комикса")]
    [SerializeField] private GameObject[] comicPanels; // Массив родительских панелей
    [SerializeField] private float fadeDuration = 1f; // Длительность fade-эффекта

    private int currentPanelIndex = -1; // Индекс текущей панели (-1 значит ещё не начали)

    public MainMenuControl mainMenuControl;

    void Start()
    {
        // Проверяем массив панелей
        if (comicPanels == null || comicPanels.Length == 0)
        {
            Debug.LogError("Comic panels array is not set!");
            return;
        }

        // Изначально делаем все панели и их детей полностью прозрачными
        foreach (var panel in comicPanels)
        {
            SetAlphaForPanel(panel, 0f);
            panel.SetActive(false);
        }

        // Показываем первую панель
        ShowNextPanel();
    }

    public void ShowNextPanel()
    {
        // Если все панели показаны - переходим на следующую сцену
        if (currentPanelIndex >= comicPanels.Length - 1)
        {
            mainMenuControl.StartCoroutine(mainMenuControl.ChangeScenesCoroutine(1));
            return;
        }

        // Увеличиваем индекс и показываем следующую панель
        currentPanelIndex++;
        GameObject panel = comicPanels[currentPanelIndex];
        panel.SetActive(true);
        StartCoroutine(FadeInPanel(panel));
    }

    private IEnumerator FadeInPanel(GameObject panel)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            SetAlphaForPanel(panel, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убедимся, что в конце alpha = 1
        SetAlphaForPanel(panel, 1f);
    }

    private void SetAlphaForPanel(GameObject panel, float alpha)
    {
        // Получаем все компоненты Image и Text в панели и её детях
        Image[] images = panel.GetComponentsInChildren<Image>(true);
        Text[] texts = panel.GetComponentsInChildren<Text>(true);

        // Устанавливаем alpha для всех изображений
        foreach (Image img in images)
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }

        // Устанавливаем alpha для всех текстов
        foreach (Text txt in texts)
        {
            Color color = txt.color;
            color.a = alpha;
            txt.color = color;
        }
    }
}