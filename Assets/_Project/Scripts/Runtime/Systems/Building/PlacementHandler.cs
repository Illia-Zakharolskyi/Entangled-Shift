using DG.Tweening;
using UnityEngine;

namespace EntangledShift.Systems.Building
{
    public abstract class PlacementHandler : MonoBehaviour, IPlacementHandler, IAnimationable
    {
        [Header("Base")]
        [SerializeField] protected BuildableItemData _data;
        [SerializeField] protected InventoryEvents _events;
        [SerializeField] protected Vector3 _boxSize;
        public string Name { get; set; }

        [Header("Grid & Terrain Settings")]
        [SerializeField] protected float _heightSnapStep = 1.5f;
        [SerializeField] protected float _minGroundOffset = -0.5f;
        [SerializeField] protected float _maxGroundOffset = 2.0f;

        protected Renderer[] _renderers;
        protected MaterialPropertyBlock _block;
        protected BuildingSocket _targetSocket;
        protected Collider[] _hitBuffer = new Collider[5];

        protected virtual void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);

            _block = new MaterialPropertyBlock();

            if (_renderers.Length == 0)
            {
                Debug.Log($"No Renderer found: {name}");
            }
        }

        public abstract bool CanPlace(RaycastHit hit);
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

            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;

                renderer.GetPropertyBlock(_block);
                _block.SetColor(_data.BaseColorId, curr);
                renderer.SetPropertyBlock(_block);
            }
        }

        protected virtual Vector3 GetSnappedPosition(Vector3 point)
        {
            float snappedX = Mathf.Round(point.x / _boxSize.x) * _boxSize.x;
            float snappedZ = Mathf.Round(point.z / _boxSize.z) * _boxSize.z;
            float snappedY = Mathf.Round(point.y / _heightSnapStep) * _heightSnapStep;

            return new Vector3(snappedX, snappedY, snappedZ);
        }

        public virtual void Animate(GameObject buildObject, float duration, float dropHeight)
        {
            BuildStructure structure = buildObject.GetComponentInChildren<BuildStructure>();
            if (structure == null) structure = buildObject.GetComponent<BuildStructure>();
            if (structure == null) return;

            Transform structureTransform = structure.transform;
            int childCount = structureTransform.childCount;
            if (childCount == 0) return;

            float pieceDuration = duration * 0.5f;
            float delayStep = (duration - pieceDuration) / Mathf.Max(1, childCount - 1);

            Sequence buildSequence = DOTween.Sequence();

            for (int i = 0; i < childCount; i++)
            {
                Transform piece = structureTransform.GetChild(i);

                Vector3 targetPos = piece.localPosition;
                Vector3 targetScale = piece.localScale;

                piece.localScale = Vector3.zero;
                piece.localPosition = targetPos + Vector3.up * dropHeight;

                float startTime = i * delayStep;

                buildSequence.Insert(startTime, piece.DOScale(targetScale, pieceDuration).SetEase(Ease.OutBack));
                buildSequence.Insert(startTime, piece.DOLocalMove(targetPos, pieceDuration).SetEase(Ease.OutQuad));
            }
        }
    }
}