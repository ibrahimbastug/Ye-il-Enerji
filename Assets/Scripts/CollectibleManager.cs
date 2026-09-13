using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectibleType
{
    [Header("Kimlik")]
    public int id;
    public string name;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Kaynak")]
    public ResourceType resource;
    public float amount = 5f;

    [Header("Spawn")]
    public int spawnCount = 10;
}

public class CollectibleManager : MonoBehaviour
{
    public List<CollectibleType> collectibleTypes = new List<CollectibleType>();

    [Header("Yerleşim")]
    public float offsetY = 0.5f;

    [Header("Referanslar")]
    public GameManager gameManager;
    public TaskManager taskManager;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (taskManager == null)
            taskManager = FindObjectOfType<TaskManager>();

        Terrain terrain = FindObjectOfType<Terrain>();

        if (terrain == null)
        {
            Debug.LogWarning(
                "CollectibleManager: Sahne üzerinde Terrain bulunamadı."
            );

            return;
        }

        foreach (var type in collectibleTypes)
        {
            if (type.prefab == null)
            {
                Debug.LogWarning(
                    $"CollectibleManager: '{type.name}' prefabı atanmamış!"
                );

                continue;
            }

            for (int i = 0; i < type.spawnCount; i++)
            {
                float randX = Random.Range(
                    0f,
                    terrain.terrainData.size.x
                );

                float randZ = Random.Range(
                    0f,
                    terrain.terrainData.size.z
                );

                float worldX =
                    terrain.GetPosition().x + randX;

                float worldZ =
                    terrain.GetPosition().z + randZ;

                float terrainY =
                    terrain.SampleHeight(
                        new Vector3(worldX, 0f, worldZ)
                    )
                    + terrain.GetPosition().y;

                GameObject obj = Instantiate(
                    type.prefab,
                    new Vector3(
                        worldX,
                        terrainY + offsetY,
                        worldZ
                    ),
                    Quaternion.identity
                );

                // -------------------------------------------------
                // COLLECTIBLE ITEM
                // -------------------------------------------------

                var item = obj.GetComponent<CollectibleItem>();

                if (item == null)
                    item = obj.AddComponent<CollectibleItem>();

                item.manager = this;
                item.resourceName = type.resource;
                item.amount = type.amount;

                // -------------------------------------------------
                // COLLECTIBLE IDENTITY
                // -------------------------------------------------

                var identity =
                    obj.GetComponent<CollectibleIdentity>();

                if (identity == null)
                    identity = obj.AddComponent<CollectibleIdentity>();

                identity.collectibleId = type.id;
                identity.displayName = type.name;

                Debug.Log(
                    $"Collectible oluşturuldu: {type.name} " +
                    $"(ID: {type.id})"
                );
            }
        }
    }

    // ---------------------------------------------------------
    // NESNE TOPLANDIĞINDA
    // ---------------------------------------------------------

    public void OnCollected(
        ResourceType resource,
        float amount,
        int collectibleID)
    {
        // Kaynağı oyuncuya ver
        if (gameManager != null)
        {
            gameManager.Collect(resource, amount);
        }
        else
        {
            Debug.LogWarning(
                "CollectibleManager: GameManager atanmamış!"
            );
        }

        // TaskManager yoksa bulmayı dene
        if (taskManager == null)
            taskManager = FindObjectOfType<TaskManager>();

        if (taskManager != null)
        {
            // Örneğin:
            // 2 numaralı sandık toplandı → Sandık görevi +1
            taskManager.ProgressTask(
                TaskType.CollectItem,
                collectibleID,
                1
            );

            // Kaynak toplama görevi
            // Örneğin 5 odun toplandı → Odun görevi +5
            if (resource != null)
            {
                int resourceID = GetResourceID(resource);

                if (resourceID >= 0)
                {
                    taskManager.ProgressTask(
                        TaskType.GatherResource,
                        resourceID,
                        Mathf.RoundToInt(amount)
                    );
                }
            }
        }
    }

    // ---------------------------------------------------------
    // RESOURCE ID BUL
    // ---------------------------------------------------------

    private int GetResourceID(ResourceType resource)
    {
        if (resource == null)
            return -1;

        // ResourceType'ın ID alanı eklenmiş olmalı.
        return resource.id;
    }
}