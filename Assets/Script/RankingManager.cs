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

    //��ŷUI
    public TextMeshProUGUI[] playerRank;
    public TextMeshProUGUI[] rank_name;
    public TextMeshProUGUI[] rank_score;

    public TMP_InputField nameInputField; // �̸� �Է� �ʵ�
    public GameObject registerButtonObj;    //��ŷ ��� ��ư ������Ʈ(�̸� �Է�â)
    public GameObject playerInputFieldObj;  //�̸� �Է� �ʵ� ������Ʈ
    public Button registerButton;   //��ŷ ��� ��ư(���� ���â)

    private Dictionary<string, int> ranking = new Dictionary<string, int>(); // �̸��� ������ ������ ��ųʸ�
    private string filePath;    //�ְ����� ���� ���
    private bool isRigister = false;

    private void Awake()
    {
        if(Instance == null)    Instance = this;
        else   Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "ranking.txt"); //�ְ����� ���� ��� ����
        nameInputField.characterLimit = 3;  //�̸� ���� ����
        LoadScores();   //���� �ε�
    }

    private void Update()
    {
        //���� ���� �� ��ŷ ���
        if (GameManager.Instance.gameEnd)
        {
            UpdateRanking();
            if (IsRank() && !isRigister)    //��ũ�ȿ� ��� ��� ����
            {
                registerButtonObj.SetActive(true);
            }
        }
    }
    private void LoadScores()
    {
        // ranking ��ųʸ��� �ʱ�ȭ
        ranking.Clear();

        // ranking.txt ������ �����ϴ��� Ȯ��
        if (File.Exists(filePath))
        {
            // ������ �о �� ���� ó��
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // "�̸�:����" ������ �����͸� ':'�� �и�
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
        // ������ �������� �������� ����
        var sortedRanking = new List<KeyValuePair<string, int>>(ranking);
        sortedRanking.Sort((x, y) => y.Value.CompareTo(x.Value));

        int rank = 0;
        //��ŷ ���â�� ���
        foreach (var entry in sortedRanking)
        {
            rank_name[rank].text = entry.Key;
            rank_score[rank].text = string.Format("{0:N0}", entry.Value);
            //�÷��̾� ��ŷ ����
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

            if (rank > 9) break;    //10����� ���
        }
    }

    private void SaveScores()
    {
        //���� �������� ����
        var sortedRanking = new List<KeyValuePair<string, int>>(ranking);
        sortedRanking.Sort((x, y) => y.Value.CompareTo(x.Value));

        //��ŷ ���Ͽ� ���� ��ŷ ���� ����
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            int count = 0;
            foreach (var entry in sortedRanking)
            {
                writer.WriteLine($"{entry.Key}:{entry.Value}"); //�̸�:���� �������� ����
                count++;

                if (count > 9) break;   //10������� ����
            }
        }
    }

    public void RegisterRanking()
    {
        string playerName = nameInputField.text.ToUpper();
        //�̸��� �ߺ��� �ƴϸ� ���
        if (!ranking.ContainsKey(playerName) && nameInputField.text != "Duplicate" && !string.IsNullOrEmpty(playerName))
        {
            isRigister = true;
            playerInputFieldObj.SetActive(false);
            registerButtonObj.SetActive(false);


            // ���ο� �÷��̾� ������ ��ŷ�� �߰�
            ranking[playerName] = GameManager.Instance.score;

            // ��ŷ ������Ʈ
            UpdateRanking();

            // ��ŷ ���Ͽ� ����
            SaveScores();
        }
        else if (!string.IsNullOrEmpty(nameInputField.text))    //�ߺ��̸� ���� �޼��� ���
        {
            nameInputField.text = "Duplicate";
        }
    }

    private bool IsRank()   //��ŷ�ȿ� ������� Ȯ��
    {
        //��ϵ� ������ 10�� �����̸� ���
        if (ranking.Count < 9) return true;
        else
        {
            foreach (var entry in ranking)
            {
                //��ŷ�ȿ� ������� true
                if (GameManager.Instance.score >= entry.Value) return true;
            }
            return false;
        }
    }

    public void ActiveRegisterField()
    {
        //�̸� �Է��ʵ� Ȱ��ȭ
        playerInputFieldObj.SetActive(true);
    }
}
