using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmoothTextChange : MonoBehaviour
{
    public Text targetText; // Ссылка на текстовый компонент
    public float duration = 1.0f; // Длительность анимации
    public bool useShortFormat = true; // Включить сокращения (1K, 1.5M)

    private int currentValue = 0;
    private int targetValue = 0;
    private Coroutine countingCoroutine;

    // Устанавливаем новое значение (плавно)
    public void SetValueSmoothly(int newValue)
    {
        targetValue = newValue;
        if (countingCoroutine != null)
        {
            StopCoroutine(countingCoroutine);
        }
        countingCoroutine = StartCoroutine(CountToTarget());
    }

    private IEnumerator CountToTarget()
    {
        int startValue = currentValue;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            currentValue = (int)Mathf.Lerp(startValue, targetValue, t);
            UpdateText();
            yield return null;
        }

        currentValue = targetValue;
        UpdateText();
    }

    // Форматирование числа (1K, 1M или обычное)
    private void UpdateText()
    {
        if (useShortFormat)
        {
            targetText.text = FormatNumber(currentValue);
        }
        else
        {
            targetText.text = currentValue.ToString("N0"); // 1,000 вместо 1000
        }
    }

    // Встроенный метод форматирования (без отдельного класса)
    private string FormatNumber(int num)
    {
        if (num >= 1000000)
            return (num / 1000000f).ToString("0.0") + "M";
        if (num >= 1000)
            return (num / 1000f).ToString("0.0") + "K";
        return num.ToString();
    }
}