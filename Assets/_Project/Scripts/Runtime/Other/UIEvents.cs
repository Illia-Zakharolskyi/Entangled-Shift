using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UIEvents", menuName = "Scriptable Objects/UIEvents")]
public class UIEvents : ScriptableObject
{
    public event Action OnPause;
    public event Action OnUnPause;
    public event Action OnInventoryOpen;
    public event Action OnInventoryClose;
    public event Action OnChestOpen;
    public event Action OnChestClose;

    public void InvokePause() => OnPause?.Invoke();
    public void InvokeUnPause() => OnUnPause?.Invoke();
    public void InvokeInventoryOpen() => OnInventoryOpen?.Invoke();
    public void InvokeInventoryClose() => OnInventoryClose?.Invoke();
    public void InvokeChestOpen() => OnChestOpen?.Invoke();
    public void InvokeChestClose() => OnChestClose?.Invoke();
}
