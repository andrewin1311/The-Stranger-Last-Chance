using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{

    // Watched https://www.youtube.com/watch?v=kJZLFyu6V78 and used ChatGPT to figure out how to spawn enemies.

    public GameObject zombiePrefab;              // Assign your zombie prefab in the Inspector
    public float spawnInterval = 5f;             // Time interval between spawns
    public Transform spawnAreaTransform;         // The transform of the object (e.g., plane) that defines the spawn area
    private const float fixedYPosition = 0.029f; // Fixed Y position for spawning

    // private float timer = 0f;                 // Timer to track time passed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Update()
    //{
    // timer += Time.deltaTime;

    //if (timer >= 10f)
    // {
    // SpawnZombie();
    //timer = 0f;
    //}
    //}

    private bool coroutineStarted = false;

    void Start()
    {
        // Make sure we only start the coroutine once
        if (!coroutineStarted)
        {
            StartCoroutine(SpawnZombieAtIntervals());
            coroutineStarted = true;
        }
    }

    public IEnumerator SpawnZombieAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            // SpawnZombie(); <- Comment this out for now.
        }
    }


    public void SpawnZombie()
    {

        // Get the bounds of the spawn area from the spawn area's Transform
        Collider spawnCollider = spawnAreaTransform.GetComponent<Collider>();
        if (spawnCollider != null)
        {
            // Get the center and size of the spawn area (bounds)
            Vector3 spawnCenter = spawnCollider.bounds.center;
            Vector3 spawnSize = spawnCollider.bounds.size;
            
            // Spawn the zombies
           
            // Generate random positions within the spawn area's bounds
            float randomX = Random.Range(spawnCenter.x - spawnSize.x / 2f, spawnCenter.x + spawnSize.x / 2f);
            float randomZ = Random.Range(spawnCenter.z - spawnSize.z / 2f, spawnCenter.z + spawnSize.z / 2f);

            Vector3 randomPosition = new Vector3(randomX, fixedYPosition, randomZ); // Keep Y fixed at 0.029f

            // Apply a rotation of -180 degrees on the Y-axis
            Quaternion rotation = Quaternion.Euler(0f, -180f, 0f);

            // Instantiate the zombie at the random position
            Instantiate(zombiePrefab, randomPosition, rotation);
        }
    }
}
