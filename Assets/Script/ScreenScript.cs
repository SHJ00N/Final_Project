using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScript : MonoBehaviour
{
    public static bool isFullScreen = true;

    private void Awake()
    {
        Screen.SetResolution(720, 1280, true);
    }
}
