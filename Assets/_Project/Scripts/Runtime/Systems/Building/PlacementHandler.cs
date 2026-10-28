using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public abstract class PlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("Base")]
        [SerializeField] protected BuildableItemData _data;
        [SerializeField] protected Vector3 _boxSize;

        protected Renderer _renderer;
        protected MaterialPropertyBlock _block;
        protected BuildingSocket _targetSocket; // violation of ISP and LSP(not every child needs this), but kept for the sake of greater DRY
        protected Collider[] _hitCache;

        protected virtual void Awake()
        {
            if (!TryGetComponent<Renderer>(out _renderer))
            {
                foreach (Transform child in transform)
                {
                    if (child.TryGetComponent<Renderer>(out _renderer)) break;
                }
            }

            _block = new MaterialPropertyBlock();
            _hitCache = new Collider[2];

            if (_renderer == null)
            {
                Debug.Log($"No Renderer found: {name}");
            }
        }

        public abstract bool CanPlace(RaycastHit hit, out GameObject targetToConsume);
        public abstract void Build(Vector3 position, Quaternion rotation);
        public abstract void UpdatePreview(GameObject preview, RaycastHit hit, bool canBuild);

        public virtual bool Raycast(Ray ray, out RaycastHit hit, float buildRange)
        {
            if (!Physics.Raycast(ray, out hit, buildRange, _data.AllowedSurfaces))
            {
                
                return false;
            }

            return true;
        }

        public virtual void UpdatePreviewColor(bool canBuild)
        {
            Color curr = canBuild ? _data.ValidColor : _data.InvalidColor;

            _renderer.GetPropertyBlock(_block);
            _block.SetColor(_data.BaseColorId, curr);
            _renderer.SetPropertyBlock(_block);
        }
    }
}
