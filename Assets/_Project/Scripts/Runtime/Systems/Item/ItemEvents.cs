using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEvents", menuName = "Scriptable Objects/ItemEvents")]
public class ItemEvents : ScriptableObject
{
    public event Action<WorldItem> OnInteractablePick;

    public void InvokeInteractablePick(WorldItem item) => OnInteractablePick?.Invoke(item);
}
