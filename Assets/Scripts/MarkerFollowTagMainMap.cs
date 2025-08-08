using UnityEngine;

public class MarkerFollowTagMainMap : MonoBehaviour
{
    public string targetTag; // Тег целевого объекта, задается в инспекторе
    public RectTransform marker; // UI-элемент (флажок)
    public float screenOffset = 20f; // Отступ от краев экрана
    public float offScreenDistance = 30f; // Сколько пикселей маркер будет за пределами экрана

    private Camera mainCamera;
    private Transform target;

    void Start()
    {
        mainCamera = Camera.main; // Получаем главную камеру
        InvokeRepeating(nameof(FindClosestTarget), 0f, 1f); // Проверка наличия объекта раз в секунду
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(target.position);

            // Если объект перед камерой
            if (viewportPos.z > 0)
            {
                // Проверяем, находится ли объект в пределах экрана
                if (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
                {
                    // Объект в поле зрения камеры, отображаем маркер
                    Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
                    marker.position = screenPos;
                    marker.gameObject.SetActive(true); // Активируем маркер
                }
                else
                {
                    // Объект за пределами экрана, но перед камерой
                    Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

                    // Ограничиваем маркер краями экрана с небольшим выходом за экран
                    screenPos.x = Mathf.Clamp(screenPos.x, -offScreenDistance, Screen.width + offScreenDistance);
                    screenPos.y = Mathf.Clamp(screenPos.y, -offScreenDistance, Screen.height + offScreenDistance);

                    marker.position = screenPos;
                    marker.gameObject.SetActive(true); // Активируем маркер
                }
            }
            else
            {
                // Если объект за камерой, скрываем маркер
                marker.gameObject.SetActive(false);
            }
        }
        else
        {
            // Если цель отсутствует, скрываем маркер
            marker.gameObject.SetActive(false);
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag); // Ищем все объекты с нужным тегом
        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        // Ищем ближайший объект
        foreach (GameObject obj in targets)
        {
            float distance = Vector3.Distance(mainCamera.transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = obj.transform;
            }
        }

        // Обновляем цель
        target = closestTarget;

        // Если цель найдена, активируем маркер
        if (target != null)
        {
            marker.gameObject.SetActive(true);
        }
        else
        {
            // Если цели нет, отключаем маркер
            marker.gameObject.SetActive(false);
        }
    }
}