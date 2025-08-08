using UnityEngine;

public class MarkerFollowTagMinimap : MonoBehaviour
{
    public string targetTag; // Тег целевого объекта
    public RectTransform marker; // UI-элемент (флажок)
    public RectTransform minimap; // RectTransform миникарты (RawImage)
    public Camera minimapCamera; // Камера миникарты
    public float markerOffset = 10f; // Отступ маркера от краев миникарты

    private Transform target;

    void Start()
    {
        // Запускаем регулярную проверку на наличие объектов с нужным тегом
        InvokeRepeating(nameof(FindClosestTarget), 0f, 1f); // Проверка раз в секунду
    }

    void Update()
    {
        if (target != null)
        {
            // Определяем положение объекта на миникарте
            Vector3 worldPosition = target.position;
            Vector3 minimapPosition = minimapCamera.WorldToViewportPoint(worldPosition);

            // Проверяем, находится ли объект в поле зрения камеры миникарты
            if (minimapPosition.z > 0 &&
                minimapPosition.x >= 0 && minimapPosition.x <= 1 &&
                minimapPosition.y >= 0 && minimapPosition.y <= 1)
            {
                // Позиционируем маркер внутри миникарты
                Vector2 localPosition = new Vector2(
                    (minimapPosition.x - 0.5f) * minimap.rect.width,
                    (minimapPosition.y - 0.5f) * minimap.rect.height
                );

                marker.anchoredPosition = localPosition;
                marker.gameObject.SetActive(true);
            }
            else
            {
                // Если объект за пределами миникарты, размещаем маркер на границе
                Vector2 edgePosition = CalculateEdgePosition(minimapPosition);
                marker.anchoredPosition = edgePosition;
                marker.gameObject.SetActive(true);
            }
        }
        else
        {
            // Если цели нет, скрываем маркер
            marker.gameObject.SetActive(false);
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag); // Ищем все объекты с нужным тегом
        float closestDistance = Mathf.Infinity;

        target = null;

        // Ищем ближайший объект
        foreach (GameObject obj in targets)
        {
            float distance = Vector3.Distance(minimapCamera.transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = obj.transform;
            }
        }

        // Если новая цель найдена, включаем маркер
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

    private Vector2 CalculateEdgePosition(Vector3 viewportPos)
    {
        // Переводим Viewport-координаты в локальные координаты миникарты
        Vector2 center = Vector2.zero;
        Vector2 direction = new Vector2(viewportPos.x - 0.5f, viewportPos.y - 0.5f).normalized;

        // Вычисляем край миникарты, где должен находиться маркер
        float halfWidth = minimap.rect.width / 2 - markerOffset;
        float halfHeight = minimap.rect.height / 2 - markerOffset;

        float slope = Mathf.Abs(direction.y / direction.x);

        if (slope > halfHeight / halfWidth)
        {
            return center + new Vector2(direction.x * halfHeight / Mathf.Abs(direction.y), Mathf.Sign(direction.y) * halfHeight);
        }
        else
        {
            return center + new Vector2(Mathf.Sign(direction.x) * halfWidth, direction.y * halfWidth / Mathf.Abs(direction.x));
        }
    }
}
