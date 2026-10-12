using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestEvents", menuName = "Scriptable Objects/ChestEvents")]
public class ChestEvents : ScriptableObject
{
    public event Action<Chest> OnChestInteract;

    public void InvokeChestInteract(Chest chest) => OnChestInteract?.Invoke(chest);
}