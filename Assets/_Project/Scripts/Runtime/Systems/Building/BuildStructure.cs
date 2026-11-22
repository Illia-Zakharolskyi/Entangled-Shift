using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildStructure : MonoBehaviour
    {
        [field: SerializeField] public BuildableItemData Data { get; private set; }
        [field: SerializeField] public StructureShape Shape { get; set; }
        public bool IsWallBuilt; // for ceiling triangle placement handler to know if you can build the ceiling because the socket is in the triangle foundation and the wall needed to build the ceiling
    }
}
