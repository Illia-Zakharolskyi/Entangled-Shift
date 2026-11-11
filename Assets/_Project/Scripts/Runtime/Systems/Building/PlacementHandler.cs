using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public abstract class PlacementHandler : MonoBehaviour, IPlacementHandler
    {
        [Header("Base")]
        [SerializeField] protected BuildableItemData _data;
        [SerializeField] protected Vector3 _boxSize;

        [Header("Grid & Terrain Settings")]
        [SerializeField] protected float _heightSnapStep = 1.5f;
        [SerializeField] protected float _minGroundOffset = -0.5f;
        [SerializeField] protected float _maxGroundOffset = 2.0f;

        protected Renderer _renderer;
        protected MaterialPropertyBlock _block;
        protected BuildingSocket _targetSocket;
        protected Collider[] _hitBuffer = new Collider[3];

        protected virtual void Awake()
        {
            if (!TryGetComponent<Renderer>(out _renderer))
            {
                foreach (Transform child in transform)
                {
                    if (child.TryGetComponent<Renderer>(out _renderer))
                    {
                        break;
                    }
                }
            }

            _block = new MaterialPropertyBlock();

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

        protected virtual Vector3 GetSnappedPosition(Vector3 point)
        {
            float snappedX = Mathf.Round(point.x / _boxSize.x) * _boxSize.x;
            float snappedZ = Mathf.Round(point.z / _boxSize.z) * _boxSize.z;
            float snappedY = Mathf.Round(point.y / _heightSnapStep) * _heightSnapStep;

            return new Vector3(snappedX, snappedY, snappedZ);
        }
    }
}