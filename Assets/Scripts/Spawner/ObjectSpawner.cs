using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Assign your prefabs here")]
    public GameObject[] prefabs;

    [Header("Assign your spawn points here")]
    public Transform[] spawnPoints;

    private void Start()
    {
        int spawnCount = spawnPoints.Length;
        Debug.Log($"[ObjectSpawner] Starting spawn. Prefabs: {prefabs.Length}, SpawnPoints: {spawnPoints.Length}, SpawnCount: {spawnCount}");

        // Create a list of indices and shuffle them
        int[] indices = new int[spawnPoints.Length];
        for (int i = 0; i < indices.Length; i++)
            indices[i] = i;

        // Fisher-Yates shuffle
        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }
        Debug.Log("[ObjectSpawner] Shuffled spawn point indices: " + string.Join(", ", indices));

        // Spawn at unique spots
        for (int i = 0; i < spawnCount; i++)
        {
            int objectIndex = Random.Range(0, prefabs.Length);
            int spawnPointIndex = indices[i];

            Debug.Log($"[ObjectSpawner] Spawning prefab '{prefabs[objectIndex].name}' at spawn point '{spawnPoints[spawnPointIndex].name}' (Position: {spawnPoints[spawnPointIndex].position})");

            Instantiate(
                prefabs[objectIndex],
                spawnPoints[spawnPointIndex].position,
                Quaternion.identity
            );
        }
    }

    // Call this to spawn a random object at a random spawn point
    public void SpawnRandomObject()
    {
        if (prefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[ObjectSpawner] Cannot spawn: No prefabs or spawn points assigned.");
            return;
        }

        int objectIndex = Random.Range(0, prefabs.Length);
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        Debug.Log($"[ObjectSpawner] Spawning prefab '{prefabs[objectIndex].name}' at spawn point '{spawnPoints[spawnPointIndex].name}' (Position: {spawnPoints[spawnPointIndex].position})");

        Instantiate(
            prefabs[objectIndex],
            spawnPoints[spawnPointIndex].position,
            Quaternion.identity
        );
    }
}
