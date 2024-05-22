using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameEndSet;
    public GameObject registerButtonObj;
    public GameObject playerInputFieldObj;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI[] rank_name;
    public TextMeshProUGUI[] rank_score;
    public TextMeshProUGUI result_score;
    public TMP_InputField nameInputField; // 이름 입력 필드
    public Button registerButton;

    public int score = 0;
    public bool gameEnd = false;
    public int direction = 1;

    private Dictionary<string, int> ranking = new Dictionary<string, int>(); // 이름과 점수를 저장할 딕셔너리
    private string filePath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        filePath = Path.Combine(Application.dataPath, "Resources/ranking.txt");

        LoadScores();

        nameInputField.characterLimit = 3;
    }
    // Update is called once per frame
    void Update()
    {
        scoreText.text = string.Format("{0:N0}", score);

        if (gameEnd)
        {
            UpdateRanking();
            result_score.text = string.Format("{0:N0}", score);
            if (IsRank())
            {
                registerButtonObj.SetActive(true);
            }
            Invoke("gameEndActive", 2f);
        }
    }

    private void gameEndActive()
    {
        gameEndSet.SetActive(true);
    }

    private void LoadScores()
    {
        // ranking 딕셔너리를 초기화
        ranking.Clear();

        // ranking.txt 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // 파일을 읽어서 각 줄을 처리
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // "이름:점수" 형식의 데이터를 ':'로 분리
                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string name = parts[0];
                    if (int.TryParse(parts[1], out int score))
                    {
                        ranking[name] = score;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
        }
    }

    private void UpdateRanking()
    {
        // 점수를 기준으로 내림차순 정렬
        var sortedRanking = new List<KeyValuePair<string, int>>(ranking);
        sortedRanking.Sort((x, y) => y.Value.CompareTo(x.Value));

        int rank = 0;
        foreach(var entry in sortedRanking)
        {
            rank_name[rank].text = entry.Key;
            rank_score[rank].text = string.Format("{0:N0}", entry.Value);
            rank++;

            if (rank > 9) break;
        }
    }

    public void RegisterRanking()
    {
        playerInputFieldObj.SetActive(false);
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            // 새로운 플레이어 점수를 랭킹에 추가
            ranking[playerName] = score;

            // 랭킹 파일에 저장
            SaveScores();

            // 랭킹 업데이트
            UpdateRanking();
        }
    }

    private void SaveScores()
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            int count = 0;
            foreach (var entry in ranking)
            {
                writer.WriteLine($"{entry.Key}:{entry.Value}");
                count++;

                if(count >9) break;
            }
        }
    }

    private bool IsRank()
    {
        if (ranking.Count < 9) return true;
        else
        {
            foreach (var entry in ranking)
            {
                if (score >= entry.Value) return true;
            }
            return false;
        }
    }
    
    public void ActiveRegisterField()
    {
        playerInputFieldObj.SetActive(true);
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }
}
