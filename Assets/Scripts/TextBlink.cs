using UnityEngine;
using UnityEngine.UI;  // Для работы с компонентом Text

public class TextBlink : MonoBehaviour
{
    public Text textComponent; // Ссылка на компонент Text
    public float blinkInterval = 0.5f; // Интервал мигания
    private float timeSinceLastBlink = 0f; // Время с последнего мигания


    void Update()
    {
        if (textComponent != null)
        {
            // Прибавляем время с последнего обновления
            timeSinceLastBlink += Time.deltaTime;

            // Если прошло время, равное интервалу мигания
            if (timeSinceLastBlink >= blinkInterval)
            {
                // Сброс времени и мигание текста
                timeSinceLastBlink = 0f;
                ToggleTextVisibility();
            }
        }
    }

    void ToggleTextVisibility()
    {
        // Получаем текущий цвет текста
        Color currentColor = textComponent.color;

        // Меняем альфа-канал с 1 (непрозрачный) до 0 (прозрачный)
        currentColor.a = (currentColor.a == 1f) ? 0f : 1f;

        // Применяем новый цвет с изменённым альфа-каналом
        textComponent.color = currentColor;
    }
}
