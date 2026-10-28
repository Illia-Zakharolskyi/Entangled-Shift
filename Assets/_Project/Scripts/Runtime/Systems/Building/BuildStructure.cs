using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildStructure : MonoBehaviour
    {
        [field: SerializeField] public BuildableItemData Data { get; private set; }
        [field: SerializeField] public StructureShape Shape { get; set; }
    }
}
