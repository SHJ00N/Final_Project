using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance; //���� ������ ����ϴ� �̱���
    public GameObject[] platform;   //���� ������Ʈ

    public bool appleActive;
    //���� ���� ��ġ ����
    float platform_x_distance = 0.85f;
    float platform_y_distance = 2.55f;
    float platform_x_startPos = -5f;
    float platform_x_endPos = 14.57f;
    float platform_y_startPos = 0f;
    float platform_y_endPos = -10.2f;

    public int platformDirection = -1;  //���� �̵� ����
    public float platform_speed = 3f;   //���� �̵� �ӵ�
    private void Awake()
    {
        if (Instance == null)   //�̱���
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        InitPlatform(); //���� ���� �� ���� ����
    }
    private void InitPlatform() //�ʱ� ���� ���� �Լ�
    {
        for(float y = platform_y_startPos; y >= platform_y_endPos; y-= platform_y_distance)
        {
            for(float x = platform_x_startPos; x <= platform_x_endPos; x += platform_x_distance)
            {
                //���� ������ ���� Ȱ��ȭ
                int appleProbability = Random.Range(0, 100);
                if (appleProbability < 5)
                    appleActive = true;
                //3������ ������ �����ϰ� ����
                int randomValue = Random.Range(0, 3);
                Instantiate(platform[randomValue],new Vector2(x, y), Quaternion.identity);
                //���������� ���� �⺻ ���·� ����
                if (appleActive)
                    appleActive = false;
            }
        }
    }
    public void MakePlatform()  //���� �߰� �Լ�
    {
        // ���� ���� y��ġ �̵� �� ����
        platform_y_endPos -= platform_y_distance;
        for (float x = platform_x_startPos; x <= platform_x_endPos; x += platform_x_distance)
        {
            //���� ������ ���� Ȱ��ȭ
            int appleProbability = Random.Range(0, 100);
            if (appleProbability < 5)
                appleActive = true;
            //3������ ������ �����ϰ� ����
            int randomValue = Random.Range(0, 3);
            Instantiate(platform[randomValue], new Vector2(x, platform_y_endPos), Quaternion.identity);
            //���������� ���� �⺻ ���·� ����
            if (appleActive)
                appleActive = false;
        }
    }
}
