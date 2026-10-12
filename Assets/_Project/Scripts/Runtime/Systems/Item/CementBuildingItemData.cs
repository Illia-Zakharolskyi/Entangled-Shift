using UnityEngine;

[CreateAssetMenu(fileName = "CementBuildingItemData", menuName = "Scriptable Objects/Building/Cement Building Item")]
public class CementBuildingItemData : BuildableItemData
{
    [Header("Placement Mechanics")]
    [SerializeField] private bool _requiresWetCement = true;

    public bool RequiresWetCement => _requiresWetCement;
}