using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameEndSet;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI result_score;
    public TextMeshProUGUI comboText;

    public int score = 0;
    public bool gameEnd = false;
    public int direction = 1;
    public int combo = 0;
    public bool isGameStart = false;

    private bool gameEndWindowActive = false;
    private bool isPaused = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        scoreText.text = string.Format("{0:N0}", score);    //����
        comboText.text = string.Format("{0}", combo);   //�޺�
        if (!gameEnd)
        {
            if (Input.GetKeyDown(KeyCode.Escape)&& isGameStart)
            {
                Time.timeScale = (!isPaused)? 0f : 1f;
                isPaused = !isPaused;
            }
        }
        //���� ��� â
        if (gameEnd)
        {
            result_score.text = string.Format("{0:N0}", score);
            if(!gameEndWindowActive)
                Invoke("gameEndActive", 2f);
        }
    }

    private void gameEndActive()
    {
        gameEndWindowActive = true;
        gameEndSet.SetActive(true);
    }
}
