using System;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildingSocket : MonoBehaviour
    {
        [field: SerializeField] public SocketType Type { get; private set; }
        [field: SerializeField] public StructureShape Shape { get; private set; } // for foundation top sockets on foundament
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
        WallDoorway,
        FoundationTriangle
    }
    public enum StructureShape
    {
        Cube,
        Triangle
    }
}
