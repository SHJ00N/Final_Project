using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenScript : MonoBehaviour
{
    public static bool isFullScreen = true;

    private void Awake()
    {
        Screen.SetResolution(720, 1280, isFullScreen);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            ChangeScreenMode();
    }

    void ChangeScreenMode()
    {
        Debug.Log("해상도 변경");
        isFullScreen = !isFullScreen;
        Screen.SetResolution(720, 1280, isFullScreen);
    }
}
