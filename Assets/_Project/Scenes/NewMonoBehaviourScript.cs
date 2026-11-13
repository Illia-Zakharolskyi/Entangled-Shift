using UnityEngine;
using System.Diagnostics;

public class LaunchRust : MonoBehaviour
{
    public void PlayRust()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "steam://run/252490",
            UseShellExecute = true
        });
    }
}