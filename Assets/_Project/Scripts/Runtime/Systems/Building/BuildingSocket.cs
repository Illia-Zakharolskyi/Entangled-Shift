using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildingSocket : MonoBehaviour
    {
        [field: SerializeField] public SocketType Type {  get; private set; }
        public bool IsOccupied { get; private set; }

        public void Occupy()
        {
            IsOccupied = true;
        }
    }

    public enum SocketType
    {
        FoundationTop,
        WallTopInner,
        WallTopOutside,
        WallDoorway
    }
}
