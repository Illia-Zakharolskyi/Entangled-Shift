using UnityEngine;

[CreateAssetMenu(fileName = "BuildableItemData", menuName = "Scriptable Objects/BuildableItemData")]
public class BuildableItemData : ItemData
{
    [Header("Building Settings")]
    [SerializeField] private GameObject _buildPrefab;
    [SerializeField] private GameObject _previewPrefab;
    [SerializeField] private LayerMask _allowedSurfaces;
    [field: SerializeField] public LayerMask ObstacleMask {  get; private set; }

    [Header("Color Settings")]
    [field: SerializeField] public Color ValidColor { get; private set; } = new Color(0f, 1f, 0f, 0.5f);
    [field: SerializeField] public Color InvalidColor { get; private set; } = new Color(1f, 0f, 0f, 0.5f);

    public GameObject BuildPrefab => _buildPrefab;
    public GameObject PreviewPrefab => _previewPrefab;
    public LayerMask AllowedSurfaces => _allowedSurfaces;

    public int BaseColorId {get; private set; } = Shader.PropertyToID("_BaseColor");
}