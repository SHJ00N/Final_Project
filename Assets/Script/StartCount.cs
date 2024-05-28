using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartCount : MonoBehaviour
{
    public GameObject startSet;
    public TextMeshProUGUI countDownText;

    private void Awake()
    {
        startSet.SetActive(true);
        StartCoroutine(OnGameStartCount());
    }

    IEnumerator OnGameStartCount()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(1f);
        countDownText.fontSize = 300;
        for(int i = 3; i > 0; i--)
        {
            countDownText.text = (i).ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        countDownText.fontSize = 250;
        countDownText.text = "Start!";

        yield return new WaitForSecondsRealtime(1f);
        startSet.SetActive(false);
        Time.timeScale = 1;
    }
}
