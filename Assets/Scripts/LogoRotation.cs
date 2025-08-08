using UnityEngine;

public class LogoRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // Скорость вращения (градусов в секунду)

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
