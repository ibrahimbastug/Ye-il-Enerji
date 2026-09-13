using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [Header("GÖREVLER")]
    public List<Task> tasks = new List<Task>();

    [Header("REFERANSLAR")]
    public GameManager gameManager;
    public TextMeshProUGUI taskText;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("TaskManager: GameManager bulunamadı!");
            return;
        }

        if (taskText == null)
            Debug.LogWarning("TaskManager: Task TextMeshProUGUI atanmamış.");

        CreateExampleTasks();

        UpdateTaskUI();
    }

    // ---------------------------------------------------------
    // ÖRNEK GÖREVLER
    // ---------------------------------------------------------

    void CreateExampleTasks()
    {
        /*
         * Buradaki görevleri daha sonra Inspector'dan veya
         * başka bir sistemden oluşturabiliriz.
         *
         * Şimdilik eski sistemindeki örnekleri koruyoruz.
         */

        if (gameManager.foodType == null)
        {
            Debug.LogWarning(
                "TaskManager: Food ResourceType atanmadığı için örnek görevler oluşturulmadı."
            );

            return;
        }

        // Daha önce otomatik olarak eklenmiş görevler varsa
        // tekrar eklenmesini önle.
        if (tasks.Count > 0)
            return;

        // 1 - Bina inşa et
        AddTask(
            "Köy Evi İnşa Et",
            "3 adet ev inşa et.",
            TaskType.Build,
            1,
            3,
            new ResourceEffect[]
            {
                new ResourceEffect(10, gameManager.foodType)
            }
        );

        // 2 - Nesne topla
        AddTask(
            "Hazine Topla",
            "2 adet sandık topla.",
            TaskType.CollectItem,
            5,
            2,
            new ResourceEffect[]
            {
                new ResourceEffect(20, gameManager.foodType)
            }
        );

        // 3 - Kaynak topla
        AddTask(
            "Odun Topla",
            "50 adet odun topla.",
            TaskType.GatherResource,
            2,
            50,
            new ResourceEffect[]
            {
                new ResourceEffect(5, gameManager.foodType)
            }
        );
    }

    // ---------------------------------------------------------
    // GÖREV EKLEME
    // ---------------------------------------------------------

    public void AddTask(
        string name,
        string description,
        TaskType type,
        int targetID,
        int requiredCount,
        ResourceEffect[] rewards)
    {
        Task newTask = new Task(
            name,
            description,
            type,
            targetID,
            requiredCount,
            rewards
        );

        tasks.Add(newTask);

        UpdateTaskUI();
    }

    // ---------------------------------------------------------
    // GÖREV İLERLETME
    // ---------------------------------------------------------

    public void ProgressTask(
        TaskType type,
        int targetID,
        int amount = 1)
    {
        if (amount <= 0)
            return;

        bool taskChanged = false;

        foreach (Task task in tasks)
        {
            if (task == null)
                continue;

            if (task.isCompleted)
                continue;

            if (task.taskType != type)
                continue;

            if (task.targetID != targetID)
                continue;

            // Sayaç artır
            task.currentCount += amount;

            // Hedefi aşmasına izin verme
            if (task.currentCount > task.requiredCount)
                task.currentCount = task.requiredCount;

            taskChanged = true;

            Debug.Log(
                $"Görev ilerledi: {task.taskName} " +
                $"({task.currentCount}/{task.requiredCount})"
            );

            // Görev tamamlandı mı?
            if (task.currentCount >= task.requiredCount)
            {
                CompleteTask(task);
            }
        }

        if (taskChanged)
            UpdateTaskUI();
    }

    // ---------------------------------------------------------
    // GÖREV TAMAMLAMA
    // ---------------------------------------------------------

    private void CompleteTask(Task task)
    {
        if (task == null)
            return;

        // Güvenlik: görev ikinci kez tamamlanmasın
        if (task.isCompleted)
            return;

        task.isCompleted = true;
        task.currentCount = task.requiredCount;

        Debug.Log("Görev tamamlandı: " + task.taskName);

        // Ödülleri ver
        if (task.rewards != null && gameManager != null)
        {
            foreach (ResourceEffect reward in task.rewards)
            {
                if (reward == null || reward.resource == null)
                    continue;

                gameManager.ApplyEffects(
                    new ResourceEffect[]
                    {
                        reward
                    }
                );
            }
        }

        // Oyuncuya mesaj göster
        if (gameManager != null)
        {
            gameManager.ShowMessage(
                $"Görev tamamlandı: {task.taskName}!"
            );
        }
    }

    // ---------------------------------------------------------
    // GÖREVLERİ TEMİZLE
    // ---------------------------------------------------------

    public void ClearTasks()
    {
        tasks.Clear();
        UpdateTaskUI();
    }

    // ---------------------------------------------------------
    // BELİRLİ GÖREVİ BUL
    // ---------------------------------------------------------

    public Task GetTask(
        TaskType type,
        int targetID)
    {
        return tasks.Find(
            task =>
                task != null &&
                task.taskType == type &&
                task.targetID == targetID &&
                !task.isCompleted
        );
    }

    // ---------------------------------------------------------
    // GÖREV UI
    // ---------------------------------------------------------

    public void UpdateTaskUI()
    {
        if (taskText == null)
            return;

        List<string> taskDescriptions = new List<string>();

        foreach (Task task in tasks)
        {
            if (task == null)
                continue;

            string status;

            if (task.isCompleted)
            {
                status = "<color=green>[TAMAMLANDI]</color>";
            }
            else
            {
                status = "<color=yellow>[DEVAM EDİYOR]</color>";
            }

            string description =
                $"{task.taskName} {status}\n" +
                $"{task.description}\n" +
                $"İlerleme: {task.currentCount}/{task.requiredCount}";

            taskDescriptions.Add(description);
        }

        taskText.text =
            string.Join("\n\n", taskDescriptions);
    }
}