[System.Serializable]
public class ResourceEffect
{
    public ResourceType resource;   // Örn: Wood, Stone, Energy
    public float changeAmount;      // + veya - etki miktarı

    public ResourceEffect(float amount, ResourceType resourceType)
    {
        changeAmount = amount;
        resource = resourceType;
    }
}