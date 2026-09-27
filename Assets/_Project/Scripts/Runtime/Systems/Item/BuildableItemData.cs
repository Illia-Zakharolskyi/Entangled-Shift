using UnityEngine;

[CreateAssetMenu(fileName = "BuildableItemData", menuName = "Scriptable Objects/BuildableItemData")]
public class BuildableItemData : ItemData
{
    [Header("Building Settings")]
    [SerializeField] private GameObject _buildPrefab;
    [SerializeField] private GameObject _previewPrefab;

    public GameObject BuildPrefab => _buildPrefab;
    public GameObject PreviewPrefab => _previewPrefab;
}