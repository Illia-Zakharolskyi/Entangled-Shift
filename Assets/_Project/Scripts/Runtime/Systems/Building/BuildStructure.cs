using UnityEngine;

namespace EntangledShift.Systems.Building
{
    //TODO to simpify this: create children for specidic logic because now there is too much unnecessary stuff for basic class
    public class BuildStructure : MonoBehaviour, IDestroyable
    {
        [Header("References")]
        [field: SerializeField] private InventoryEvents _events;
        [field: SerializeField] public BuildableItemData Data { get; private set; }

        [Header("Settings")]
        [Tooltip("Already assigned as ShowName of data(BuildableItemData): not necessarily to assign by hand(only if you want different name than data variable ShowName)")]
        public string Name { get; private set; }
        // to know if that is a wall: used in foundation triangle
        [field: SerializeField] public StructureType Type { get; private set; }
        // for ceilng placement handlers
        [field: SerializeField] public StructureShape Shape { get; private set; }
        // for ceiling placement handlers
        public bool IsWallBuilt;

        private void Awake()
        {
            Name = Data.ShowName;
        }
        public virtual void Destroy()
        {
            _events.InvokeItemAdd(Data, 1);
            Destroy(gameObject.transform.root.gameObject);
        }
    }

    public enum StructureType
    {
        Common, // if structure doesn't need it
        Wall
    }
}
