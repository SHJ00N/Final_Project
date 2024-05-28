using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    public static GameStartManager Instance;

    public GameObject startSet;
    public TextMeshProUGUI countDownText;
    public Image blinderImage;

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); };
        startSet.SetActive(true);
    }
}
