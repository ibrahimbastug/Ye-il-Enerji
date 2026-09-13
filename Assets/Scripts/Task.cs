using UnityEngine;

public enum TaskType
{
    Build,          // 1. İnşa Görevi (Örn: Ev inşa et)
    CollectItem,    // 2. Nesne Toplama (Örn: 5 adet Anahtar topla)
    GatherResource  // 3. Kaynak Toplama (Örn: 100 Adet Odun topla)
}

[System.Serializable]
public class Task
{
    public string taskName;
    public string description;
    public TaskType taskType;
    
    // İnşa için BuildingID, Nesne için ItemID, Kaynak için ResourceID olarak kullanılır
    public int targetID; 
    
    public int requiredCount;   // Hedef miktar
    public int currentCount = 0; // Mevcut ilerleme
    public bool isCompleted = false;

    public ResourceEffect[] rewards;

    public Task(string name, string desc, TaskType type, int targetId, int count, ResourceEffect[] reward)
    {
        taskName = name;
        description = desc;
        taskType = type;
        targetID = targetId;
        requiredCount = count;
        rewards = reward;
        currentCount = 0;
        isCompleted = false;
    }
}