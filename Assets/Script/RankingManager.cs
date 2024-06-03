using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;

    //랭킹UI
    public TextMeshProUGUI[] playerRank;
    public TextMeshProUGUI[] rank_name;
    public TextMeshProUGUI[] rank_score;

    public TMP_InputField nameInputField; // 이름 입력 필드
    public GameObject registerButtonObj;    //랭킹 등록 버튼 오브젝트(이름 입력창)
    public GameObject playerInputFieldObj;  //이름 입력 필드 오브젝트
    public Button registerButton;   //랭킹 등록 버튼(게임 결과창)

    private Dictionary<string, int> ranking = new Dictionary<string, int>(); // 이름과 점수를 저장할 딕셔너리
    private string filePath;    //최고점수 파일 경로
    private bool isRigister = false;

    private void Awake()
    {
        if(Instance == null)    Instance = this;
        else   Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "ranking.txt"); //최고점수 파일 경로 지정
        nameInputField.characterLimit = 3;  //이름 제한 길이
        LoadScores();   //점수 로드
    }

    private void Update()
    {
        //게임 오버 시 랭킹 출력
        if (GameManager.Instance.gameEnd)
        {
            UpdateRanking();
            if (IsRank() && !isRigister)    //랭크안에 들면 등록 가능
            {
                registerButtonObj.SetActive(true);
            }
        }
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
            File.Create(filePath).Close();
        }
    }

    private void UpdateRanking()
    {
        // 점수를 기준으로 내림차순 정렬
        var sortedRanking = new List<KeyValuePair<string, int>>(ranking);
        sortedRanking.Sort((x, y) => y.Value.CompareTo(x.Value));

        int rank = 0;
        //랭킹 결과창에 출력
        foreach (var entry in sortedRanking)
        {
            rank_name[rank].text = entry.Key;
            rank_score[rank].text = string.Format("{0:N0}", entry.Value);
            //플레이어 랭킹 강조
            if (entry.Key == nameInputField.text.ToUpper() && isRigister)
            {
                playerRank[rank].color = new Color(0.9f, 0.9f, 0.4f);
                rank_name[rank].color = new Color(0.9f, 0.9f, 0.4f);
                rank_score[rank].color = new Color(0.9f, 0.9f, 0.4f);
            }
            else
            {
                playerRank[rank].color = Color.black;
                rank_name[rank].color = Color.black;
                rank_score[rank].color = Color.black;
            }
            rank++;

            if (rank > 9) break;    //10등까지 출력
        }
    }

    private void SaveScores()
    {
        //점수 내림차순 정렬
        var sortedRanking = new List<KeyValuePair<string, int>>(ranking);
        sortedRanking.Sort((x, y) => y.Value.CompareTo(x.Value));

        //랭킹 파일에 현재 랭킹 정보 저장
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            int count = 0;
            foreach (var entry in sortedRanking)
            {
                writer.WriteLine($"{entry.Key}:{entry.Value}"); //이름:점수 형식으로 저장
                count++;

                if (count > 9) break;   //10등까지만 저장
            }
        }
    }

    public void RegisterRanking()
    {
        string playerName = nameInputField.text.ToUpper();
        //이름이 중복이 아니면 등록
        if (!ranking.ContainsKey(playerName) && nameInputField.text != "Duplicate" && !string.IsNullOrEmpty(playerName))
        {
            isRigister = true;
            playerInputFieldObj.SetActive(false);
            registerButtonObj.SetActive(false);


            // 새로운 플레이어 점수를 랭킹에 추가
            ranking[playerName] = GameManager.Instance.score;

            // 랭킹 업데이트
            UpdateRanking();

            // 랭킹 파일에 저장
            SaveScores();
        }
        else if (!string.IsNullOrEmpty(nameInputField.text))    //중복이면 오류 메세지 출력
        {
            nameInputField.text = "Duplicate";
        }
    }

    private bool IsRank()   //랭킹안에 들었는지 확인
    {
        //등록된 점수가 10명 이하이면 등록
        if (ranking.Count < 9) return true;
        else
        {
            foreach (var entry in ranking)
            {
                //랭킹안에 들었으면 true
                if (GameManager.Instance.score >= entry.Value) return true;
            }
            return false;
        }
    }

    public void ActiveRegisterField()
    {
        //이름 입력필드 활성화
        playerInputFieldObj.SetActive(true);
    }
}
