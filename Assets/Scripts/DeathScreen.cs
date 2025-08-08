using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [Header("Панели")]
    public RectTransform topPanel;
    public RectTransform bottomPanel;

    [Header("Настройки движения")]
    public float moveDuration = 0.75f;
    public float curveStrength = 0.75f;
    public AnimationCurve movementCurve;

    [Header("Мигающий текст")]
    public Text blinkingText; // Изменили Image на Text
    public float blinkInterval = 0.75f;

    [Header("Меню смерти")]
    public GameObject deathMenu;

    [Header("Флаг смерти")]
    public static bool isDead = false;

    private bool panelsClosed = false;
    private float elapsedMoveTime = 0f;
    private Vector2 topStartPos;
    private Vector2 bottomStartPos;
    private Vector2 topTargetPos;
    private Vector2 bottomTargetPos;

    private float blinkTimer = 0f;
    private bool textVisible = true;

    void Start()
    {
        blinkingText.gameObject.SetActive(false); // Теперь работаем с текстом
        deathMenu.SetActive(false);

        topStartPos = topPanel.anchoredPosition;
        bottomStartPos = bottomPanel.anchoredPosition;

        topTargetPos = new Vector2(topStartPos.x, 0);
        bottomTargetPos = new Vector2(bottomStartPos.x, 0);
    }

    void Update()
    {
        if (isDead && !panelsClosed)
        {
            elapsedMoveTime += Time.deltaTime * curveStrength;
            float t = Mathf.Clamp01(elapsedMoveTime / moveDuration);
            float curvedT = movementCurve.Evaluate(t);

            topPanel.anchoredPosition = Vector2.Lerp(topStartPos, topTargetPos, curvedT);
            bottomPanel.anchoredPosition = Vector2.Lerp(bottomStartPos, bottomTargetPos, curvedT);

            if (t >= 1f)
            {
                panelsClosed = true;
                blinkingText.gameObject.SetActive(true); // Активируем текст
                deathMenu.SetActive(true);
            }
        }

        if (panelsClosed)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                textVisible = !textVisible;
                blinkingText.enabled = textVisible; // Переключаем видимость текста
                blinkTimer = 0f;
            }
        }
    }
}