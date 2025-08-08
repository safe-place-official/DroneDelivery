using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform quadcopter;
    public float fixedHeight = 350f;

    void LateUpdate()
    {
        // Фиксируем положение камеры над квадрокоптером
        transform.position = new Vector3(quadcopter.position.x, quadcopter.position.y + fixedHeight, quadcopter.position.z);
        // Фиксируем вращение камеры (смотрим строго вниз)
        transform.rotation = Quaternion.Euler(90, quadcopter.eulerAngles.y, 0);
    }
}
