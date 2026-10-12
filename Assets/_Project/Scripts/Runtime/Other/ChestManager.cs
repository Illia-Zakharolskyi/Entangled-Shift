using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EntangledShift.Runtime.Systems;

// написав ШІ
public class ChestManager : MonoBehaviour
{
    [Header("Event Channel")]
    [SerializeField] private ChestEvents _chestEvents;

    [Header("Animation Settings")]
    [SerializeField] private Vector3 _openRotation = new Vector3(-80f, 0f, 0f);
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private Ease _openEase = Ease.OutBack;
    [SerializeField] private Ease _closeEase = Ease.InQuad;

    [Header("Audio SFX")]
    [SerializeField] private AudioClip _openAudio;
    [SerializeField] private AudioClip _closeAudio;

    private readonly Dictionary<Transform, bool> _chestStates = new();
    private readonly Dictionary<Transform, Tween> _activeTweens = new();
    private readonly Dictionary<Transform, Vector3> _initialRotations = new();

    private void OnEnable()
    {
        if (_chestEvents != null)
        {
            _chestEvents.OnChestInteract += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (_chestEvents != null)
        {
            _chestEvents.OnChestInteract -= OnInteract;
        }
    }

    private void OnInteract(Chest chest)
    {
        if (chest == null || chest._top == null) return;

        Transform topTransform = chest._top;

        if (!_initialRotations.ContainsKey(topTransform))
        {
            _initialRotations[topTransform] = topTransform.localEulerAngles;
        }

        bool isOpen = !_chestStates.GetValueOrDefault(topTransform, false);
        _chestStates[topTransform] = isOpen;

        if (isOpen)
        {
            Open(topTransform);
        }
        else
        {
            Close(topTransform);
        }
    }

    private void Open(Transform top)
    {
        if (_openAudio != null)
        {
            AudioController.Instance.PlayOneShotSFXSound(_openAudio);
        }

        if (_activeTweens.TryGetValue(top, out Tween activeTween) && activeTween.IsActive())
        {
            activeTween.Kill();
        }

        _activeTweens[top] = top
            .DOLocalRotate(_openRotation, _duration)
            .SetEase(_openEase);
    }

    private void Close(Transform top)
    {
        if (_closeAudio != null)
        {
            AudioController.Instance.PlayOneShotSFXSound(_closeAudio);
        }

        if (_activeTweens.TryGetValue(top, out Tween activeTween) && activeTween.IsActive())
        {
            activeTween.Kill();
        }

        Vector3 closedRotation = _initialRotations.GetValueOrDefault(top, Vector3.zero);

        _activeTweens[top] = top
            .DOLocalRotate(closedRotation, _duration)
            .SetEase(_closeEase);
    }

    private void OnDestroy()
    {
        foreach (var tween in _activeTweens.Values)
        {
            if (tween.IsActive()) tween.Kill();
        }
        _activeTweens.Clear();
    }
}