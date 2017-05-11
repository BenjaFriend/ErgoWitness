using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple containter class that will control whether the netflow objects
/// should be paused or not
/// </summary>
public class NetflowPauseController : MonoBehaviour
{

    private static bool isPaused = true;

    public static bool IsPaused { get { return isPaused; } set { isPaused = value; } }

    public void TogglePaused()
    {
        isPaused = !isPaused;
    }

}
