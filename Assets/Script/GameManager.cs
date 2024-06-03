using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameEndSet;
    public GameObject pauseSet;
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
        scoreText.text = string.Format("{0:N0}", score);    //점수
        comboText.text = string.Format("{0}", combo);   //콤보
        if (!gameEnd)
        {
            if (Input.GetKeyDown(KeyCode.Escape)&& isGameStart)
            {
                Time.timeScale = (!isPaused)? 0f : 1f;
                pauseSet.SetActive((!isPaused)? true : false);
                isPaused = !isPaused;
            }
        }
        //게임 결과 창
        if (gameEnd)
        {
            AudioManager.Instance.PlayBgm(false);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.End);

            result_score.text = string.Format("{0:N0}", score);
            if(!gameEndWindowActive)
                Invoke("gameEndActive", 2f);
        }
    }

    //콤보 효과
    public IEnumerator ComboEffect()
    {
        for (float i = 1.5f; i > 1; i -= 0.1f)
        {
            yield return new WaitForSeconds(0.025f);
            comboText.rectTransform.localScale = new Vector2(i, i*1.5f);
        }
    }

    private void gameEndActive()
    {
        gameEndWindowActive = true;
        gameEndSet.SetActive(true);
    }

    public void ButtonDownContinue()
    {
        Time.timeScale = 1f;
        pauseSet.SetActive(false);
        isPaused = false;
    }
}
