using UnityEngine;

public class IonAirPurifier : MonoBehaviour
{
    public Transform fanBlade; // Ссылка на объект винта
    public float rotationSpeed = 200f; // Скорость вращения винта
    public float oscillationSpeed = 0.5f; // Скорость колебаний
    public float oscillationAmount = 0.1f; // Амплитуда колебаний
    public float movementSpeed = 0.3f; // Скорость перемещения
    public float movementRange = 0.2f; // Размах перемещения

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float timeOffset;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        timeOffset = Random.Range(0f, 100f); // Для случайных колебаний между очистителями
    }

    void Update()
    {
        // Вращение винта по оси Y
        if (fanBlade != null)
        {
            fanBlade.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        // Колебания очистителя
        float oscillation = Mathf.Sin(Time.time * oscillationSpeed + timeOffset) * oscillationAmount;
        float movement = Mathf.Sin(Time.time * movementSpeed + timeOffset) * movementRange;

        transform.position = initialPosition + new Vector3(0, movement, 0);
        transform.rotation = initialRotation * Quaternion.Euler(oscillation, 0, oscillation);
    }
}
