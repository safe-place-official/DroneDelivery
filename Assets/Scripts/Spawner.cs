using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject SpawnObject;
    public Transform[] SpawnPoses; // ћассив точек спавна
    public Collider SpawnArea; //  оллайдер, внутри которого должны быть точки спавна
    public int MaxSpawnAttempts = 30; // ћакс. попыток найти точку внутри коллайдера

    public static int Counter = 0;

    void Start()
    {
        SpawnObjects();
    }

    private void Update()
    {
        if (NewMonoBehaviourScript.IsPizzaGave)
        {
            NewMonoBehaviourScript.IsPizzaGave = false;
            Counter = 2;
        }

        if (Counter != 0)
        {
            SpawnObjects();
            Counter -= 1;
        }
    }

public void SpawnObjects()
{
    Vector3 validSpawnPosition = GetRandomPositionInsideCollider();
    if (validSpawnPosition != Vector3.zero)
    {
        // »спользуем rotation из префаба SpawnObject
        Instantiate(SpawnObject, validSpawnPosition, SpawnObject.transform.rotation);
        NewMonoBehaviourScript.i = 0;
        NewMonoBehaviourScript.TemperaturePizza = 180f;
    }
    else
    {
        Debug.LogWarning("No valid spawn points inside the collider!");
    }
}

    // ¬озвращает случайную позицию внутри SpawnArea в глобальных координатах
    private Vector3 GetRandomPositionInsideCollider()
    {
        if (SpawnArea == null)
        {
            Debug.LogError("Spawn Area Collider is not assigned!");
            return Vector3.zero;
        }

        // ≈сли есть точки спавна, сначала попробуем использовать их
        if (SpawnPoses != null && SpawnPoses.Length > 0)
        {
            for (int i = 0; i < MaxSpawnAttempts; i++)
            {
                int randomIndex = Random.Range(0, SpawnPoses.Length);
                Vector3 spawnPos = SpawnPoses[randomIndex].position; // »спользуем глобальные координаты

                if (IsPointInsideCollider(spawnPos))
                {
                    return spawnPos;
                }
            }

            // ≈сли не нашли точку за MaxSpawnAttempts попыток, ищем вручную
            foreach (Transform point in SpawnPoses)
            {
                Vector3 spawnPos = point.position; // √лобальные координаты
                if (IsPointInsideCollider(spawnPos))
                {
                    return spawnPos;
                }
            }
        }

        // ≈сли нет точек спавна или ни одна не подошла, генерируем случайную точку внутри коллайдера
        Bounds bounds = SpawnArea.bounds;
        for (int i = 0; i < MaxSpawnAttempts; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (IsPointInsideCollider(randomPoint))
            {
                return randomPoint;
            }
        }

        return Vector3.zero; // ≈сли не удалось найти подход€щую точку
    }

    // ѕровер€ет, находитс€ ли точка внутри коллайдера (в глобальных координатах)
    private bool IsPointInsideCollider(Vector3 point)
    {
        Collider[] colliders = Physics.OverlapSphere(point, 0.01f); // маленький радиус
        foreach (var col in colliders)
        {
            if (col == SpawnArea)
            {
                return true;
            }
        }
        return false;
    }

}