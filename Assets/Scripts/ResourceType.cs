using UnityEngine;

[CreateAssetMenu(menuName = "Game/Resource Type")]
public class ResourceType : ScriptableObject
{
    [Header("Kimlik")]
    public int id;
    
    [Header("G�rsel ve �sim")]
    public string resourceName;   // �rn: "Wood", "Stone", "Energy"
    public Sprite icon;

    [Header("K�s�tlar")]
    public bool canBeNegative = false; // �rn: Pollution negatif olabilir mi?
}
