using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UIEvents", menuName = "Scriptable Objects/UIEvents")]
public class UIEvents : ScriptableObject
{
    public event Action OnPause;
    public event Action OnUnPause;

    public void InvokePause() => OnPause?.Invoke();
    public void InvokeUnPause() => OnUnPause?.Invoke();
}
