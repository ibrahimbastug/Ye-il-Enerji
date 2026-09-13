using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameManager gameManager;
    public float buildDistance = 3f;

    [Header("Toplanabilir Nesne Yönetimi")]
    public CollectibleManager collectibleManager;

    [Header("Görev Yönetimi")]
    public TaskManager taskManager;

    void Awake()
    {
        if (collectibleManager == null)
            collectibleManager = FindObjectOfType<CollectibleManager>();

        if (taskManager == null)
            taskManager = FindObjectOfType<TaskManager>();
    }

    public void Build(BuildingType building)
    {
        if (building == null)
        {
            Debug.LogWarning("BuildManager: BuildingType null!");
            return;
        }

        // -------------------------------------------------
        // BİNA SINIRI
        // -------------------------------------------------

        if (!building.CanBuildMore())
        {
            Debug.LogWarning(
                $"{building.name} için maksimum sınıra ulaşıldı!"
            );

            gameManager.ShowMessage(
                $"Maksimum {building.name} sınırına ulaşıldı!"
            );

            return;
        }

        // -------------------------------------------------
        // KAYNAK KONTROLÜ
        // -------------------------------------------------

        if (!gameManager.HasEnoughResources(building.effects))
        {
            Debug.Log("Kaynak yetersiz!");
            return;
        }

        // -------------------------------------------------
        // BİNA POZİSYONU
        // -------------------------------------------------

        Vector3 buildPos =
            Camera.main.transform.position +
            Camera.main.transform.forward * buildDistance;

        if (Physics.Raycast(
            buildPos + Vector3.up * 10f,
            Vector3.down,
            out RaycastHit hit,
            50f))
        {
            buildPos.y = hit.point.y;
        }

        // -------------------------------------------------
        // BİNAYI OLUŞTUR
        // -------------------------------------------------

        GameObject obj = Instantiate(
            building.prefab,
            buildPos,
            Quaternion.identity
        );

        // -------------------------------------------------
        // MALİYET / ETKİLER
        // -------------------------------------------------

        gameManager.ApplyEffects(building.effects);

        // -------------------------------------------------
        // TOPLANABİLİR BİNA/NESNE İSE
        // -------------------------------------------------

        if (building.isCollectible)
        {
            var item = obj.GetComponent<CollectibleItem>();

            if (item == null)
                item = obj.AddComponent<CollectibleItem>();

            if (collectibleManager == null)
                collectibleManager =
                    FindObjectOfType<CollectibleManager>();

            item.manager = collectibleManager;
            item.resourceName = building.collectResource;
            item.amount = building.collectAmount;
        }

        // -------------------------------------------------
        // BİNA SAYISINI ARTIR
        // -------------------------------------------------

        building.BuildBuilding();

        Debug.Log(
            $"{building.name} inşa edildi! " +
            $"Toplam: {building.builtCount}"
        );

        // -------------------------------------------------
        // GÖREVİ İLERLET
        // -------------------------------------------------

        if (taskManager == null)
            taskManager = FindObjectOfType<TaskManager>();

        if (taskManager != null)
        {
            taskManager.ProgressTask(
                TaskType.Build,
                building.id,
                1
            );
        }
        else
        {
            Debug.LogWarning(
                "BuildManager: TaskManager bulunamadı!"
            );
        }
    }
}