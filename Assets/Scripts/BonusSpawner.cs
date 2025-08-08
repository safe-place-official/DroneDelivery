using UnityEngine;
using System.Collections;
using System.Linq;

public class RandomSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Array of possible spawn points")]
    public Transform[] spawnPoints;

    [Tooltip("Prefabs to spawn randomly")]
    public GameObject[] prefabsToSpawn;

    [Tooltip("Tags of objects to destroy before new spawn")]
    public string[] tagsToCleanup;

    [Tooltip("Time between spawn cycles in seconds")]
    public float spawnInterval = 30f;

    [Tooltip("Number of objects to spawn each cycle")]
    [Min(1)] // Ensures value is at least 1
    public int objectsToSpawn = 3;

    void Start()
    {
        if (spawnPoints.Length < objectsToSpawn)
        {
            Debug.LogError($"Need at least {objectsToSpawn} spawn points!");
            return;
        }

        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle()
    {
        while (true)
        {
            CleanupOldObjects();
            yield return new WaitForSeconds(0.1f);
            SpawnNewObjects();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void CleanupOldObjects()
    {
        foreach (string tag in tagsToCleanup)
        {
            if (string.IsNullOrEmpty(tag)) continue;

            var objectsToDelete = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in objectsToDelete)
            {
                Destroy(obj);
            }
        }
    }

    void SpawnNewObjects()
    {
        if (spawnPoints.Length < objectsToSpawn)
        {
            Debug.LogError($"Not enough spawn points! Need {objectsToSpawn}, have {spawnPoints.Length}");
            return;
        }

        if (prefabsToSpawn.Length == 0)
        {
            Debug.LogError("No prefabs to spawn!");
            return;
        }

        // Get random unique spawn points
        var selectedPoints = spawnPoints.OrderBy(x => Random.value).Take(objectsToSpawn).ToArray();

        foreach (var point in selectedPoints)
        {
            var randomPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];
            Instantiate(randomPrefab, point.position, Quaternion.identity);
        }
    }
}