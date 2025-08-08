using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathScreenController : MonoBehaviour
{
    public bool IsDead = false; // Статическая переменная для управления из других скриптов

    [Header("Панели")]
    [SerializeField] private RectTransform topPanel;    // Верхняя черная панель
    [SerializeField] private RectTransform bottomPanel; // Нижняя черная панель

    [Header("Надписи и меню")]
    public Text signalLostText;       // Текст "Сигнал потерян"
    [SerializeField] private GameObject deathMenu;

    [Header("Настройки анимации")]
    [SerializeField] private float panelSpeed = 200f;   // Скорость движения панелей
    [SerializeField] private float blinkInterval = 0.5f; // Интервал мерцания текста

    private float _panelHeight; // Высота панели (вычисляется автоматически)
    private bool _isAnimating;  // Флаг анимации

    private void Start()
    {
        deathMenu.SetActive(false);

        // Запоминаем высоту панели (предполагаем, что она на всю ширину экрана)
        _panelHeight = topPanel.rect.height;

        // Устанавливаем панели за пределами экрана
        topPanel.anchoredPosition = new Vector2(0, _panelHeight);
        bottomPanel.anchoredPosition = new Vector2(0, -_panelHeight);
    }

    private void Update()
    {
        // Если игрок "умер" и анимация еще не началась
        if (IsDead && !_isAnimating)
        {
            StartCoroutine(DeathAnimation());
            _isAnimating = true;
        }
    }

    // Корутина анимации смерти
    private IEnumerator DeathAnimation()
    {
        // Двигаем панели к центру
        while (topPanel.anchoredPosition.y > 0 || bottomPanel.anchoredPosition.y < 0)
        {
            // Двигаем верхнюю панель вниз
            if (topPanel.anchoredPosition.y > 0)
            {
                float newY = Mathf.MoveTowards(topPanel.anchoredPosition.y, 0, panelSpeed * Time.deltaTime);
                topPanel.anchoredPosition = new Vector2(0, newY);
            }

            // Двигаем нижнюю панель вверх
            if (bottomPanel.anchoredPosition.y < 0)
            {
                float newY = Mathf.MoveTowards(bottomPanel.anchoredPosition.y, 0, panelSpeed * Time.deltaTime);
                bottomPanel.anchoredPosition = new Vector2(0, newY);
            }

            yield return null;
        }

        // Включаем текст "Сигнал потерян" и запускаем мерцание
        signalLostText.gameObject.SetActive(true);
        StartCoroutine(BlinkText());

        // Ждем 1 секунду перед показом кнопок
        yield return new WaitForSeconds(1f);

        // Показываем кнопки
        deathMenu.SetActive(true);
    }

    // Корутина мерцания текста
    private IEnumerator BlinkText()
    {
        while (true)
        {
            signalLostText.gameObject.SetActive(!signalLostText); // Включаем/выключаем текст
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}