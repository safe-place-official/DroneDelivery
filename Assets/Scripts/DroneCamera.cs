using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    public Transform drone;                 // ќбъект дрона (должен быть выставлен в инспекторе)
    public float defaultDistance = 5f;      // ќбычное рассто€ние камеры от дрона
    public float minDistance = 1f;          // ћинимально допустимое приближение
    public float smoothSpeed = 10f;         // —корость приближени€/отдалени€ камеры

    private Vector3 initialLocalPosition;   // Ќачальное локальное положение камеры
    private Quaternion initialLocalRotation; // Ќачальный локальный поворот камеры
    private Vector3 desiredLocalOffset;     // ∆елаемое смещение камеры в локальных координатах

    void Start()
    {
        // —охран€ем изначальное положение и поворот камеры относительно дрона
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        desiredLocalOffset = initialLocalPosition.normalized * defaultDistance;
    }

    void LateUpdate()
    {
        // ѕолучаем желаемое положение камеры в мировых координатах
        Vector3 desiredCameraPos = drone.TransformPoint(desiredLocalOffset);

        // ѕровер€ем столкновени€
        RaycastHit hit;
        Vector3 directionToCamera = desiredCameraPos - drone.position;
        float targetDistance = defaultDistance;

        if (Physics.Raycast(drone.position, directionToCamera.normalized, out hit, defaultDistance))
        {
            targetDistance = Mathf.Clamp(hit.distance - 0.1f, minDistance, defaultDistance);
        }

        // ¬ычисл€ем финальное положение камеры
        Vector3 finalCameraPos = drone.position + directionToCamera.normalized * targetDistance;

        // ѕлавно перемещаем камеру
        transform.position = Vector3.Lerp(transform.position, finalCameraPos, Time.deltaTime * smoothSpeed);

        // ѕримен€ем поворот родител€, но сохран€ем начальный локальный поворот
        transform.rotation = drone.rotation * initialLocalRotation;
    }
}