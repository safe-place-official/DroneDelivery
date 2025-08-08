using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;

    public float xAx = 0f;
    public float yAx = 0f;

    private Vector2 inputVector = Vector2.zero;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        // ѕреобразуем координаты экрана в локальные координаты фона с учетом камеры
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out localPoint);

        // Ќормализуем координаты по размеру фона
        Vector2 normalizedPoint = new Vector2(
            localPoint.x / (background.sizeDelta.x / 2),
            localPoint.y / (background.sizeDelta.y / 2)
        );

        inputVector = (normalizedPoint.magnitude > 1f) ? normalizedPoint.normalized : normalizedPoint;
        xAx = inputVector.x;
        yAx = inputVector.y;
        handle.anchoredPosition = new Vector2(xAx * (background.sizeDelta.x / 2), yAx * (background.sizeDelta.y / 2));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        xAx = 0f;
        yAx = 0f;
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
