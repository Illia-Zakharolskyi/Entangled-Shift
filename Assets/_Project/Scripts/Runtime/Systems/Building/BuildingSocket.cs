using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildingSocket : MonoBehaviour
    {
        public bool IsOccupied { get; private set; }

        public void Occupy()
        {
            IsOccupied = true;
        }
    }
}
