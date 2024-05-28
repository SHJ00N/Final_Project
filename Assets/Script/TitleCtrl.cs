using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleCtrl : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
