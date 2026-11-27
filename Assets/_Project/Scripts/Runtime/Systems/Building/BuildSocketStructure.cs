using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public class BuildSocketStructure : BuildStructure
    {
        [field: SerializeField] public BuildingSocket BuiltOnSocket { get; set; }

        public override void Destroy()
        {
            BuiltOnSocket.Deoccupy();
            base.Destroy();
        }
    }
}
