using UnityEngine;

[System.Serializable]
public class BuildingType
{
    public int id;                  // Bina ID'si
    public string name;             // Örn: "Ev", "Kömür Santrali", "Ağaç"
    public GameObject prefab;       // Sahneye yerleştirilecek nesne
    public bool isCollectible;      // Toplanabilir mi?

    [Header("Bina Takip ve İstatistik")]
    public int builtCount = 0;      // Sahnede şu an aktif kaç tane inşa edildi?
    public int maxLimit = 0;        // Sınırsız ise 0 bırakın, sınır koymak isterseniz sayı yazın (Örn: 5)

    [Header("Maliyet veya Üretim (inşa anında)")]
    public ResourceEffect[] effects; // İnşa anında uygulanacak değişimler

    [Header("Toplanabilir Ayarları")]
    [Tooltip("Bu nesne toplandığında hangi kaynağı verecek?")]
    public ResourceType collectResource;

    [Tooltip("Bu nesne toplandığında verilecek miktar.")]
    public float collectAmount = 0f;

    public bool isBuilt = false; // En az 1 adet inşa edildi mi?

    // Bina inşa etme fonksiyonu
    public void BuildBuilding()
    {
        isBuilt = true;
        builtCount++; // Sayacı 1 artırıyoruz
    }

    // Bina yıkıldığında veya toplandığında çağrılacak fonksiyon
    public void RemoveBuilding()
    {
        builtCount--;
        if (builtCount <= 0)
        {
            builtCount = 0;
            isBuilt = false; // Sahnede hiç kalmadıysa isBuilt tekrar false olur
        }
    }

    // Maksimum sınıra ulaşıldı mı kontrolü
    public bool CanBuildMore()
    {
        if (maxLimit <= 0) return true; // Sınır belirlenmemişse sonsuz yapılabilir
        return builtCount < maxLimit;
    }
}