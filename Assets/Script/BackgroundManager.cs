using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    public static BackgroundManager Instance;   //��� ������ ����ϴ� �̱���
    public GameObject[] backgrounds;    //��� ������Ʈ

    //��� ���� ��ġ
    float currentBackground_y;
    float background_yDistance = 25.5f;

    public int background_direction = -1;   //��� �̵� ����
    public float[] background_speed = {1,3,5,7,9}; //��溰 �̵� �ӵ�
    public int currentBackgroundNum = 0;    //���� ���� �� ��� ��ȣ
    private void Awake()
    {
        if (Instance == null)   //�̱���
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        //���� ī�޶� ��ġ�� �������� ��� ����
        currentBackground_y = Camera.main.transform.position.y+1f;  
        Init_background();
    }

    // Update is called once per frame
    void Update()
    {
        //ī�޶� ������ ���� ���� ��� �̸� ����
        if (currentBackground_y >Camera.main.transform.position.y)
            Make_background();
    }
    void Init_background()  //���� ������ �� ��� ���� �Լ�
    {
        //4���� ��� ����
        for (float y = currentBackground_y; y >= -background_yDistance; y -= background_yDistance)
            for (float x = 0; x <= 24; x += 24)
                for (int i = 0; i < 5; i++)
                {
                    Instantiate(backgrounds[i], new Vector3(x, y, -i), Quaternion.identity);
                    currentBackgroundNum++;
                    if(currentBackgroundNum > 4)
                        currentBackgroundNum = 0;
                }

        currentBackground_y -= background_yDistance;    //��� ���� ��ġ �̵�
    }
    public void Make_background()   // ��� �߰� ���� �Լ�
    {
        //��� ���� y��ġ �̵� �� 2�� ����
        currentBackground_y -= background_yDistance;
        for (float x = 0; x <= 24; x += 24)
            for (int i = 0; i < 5; i++)
                Instantiate(backgrounds[i], new Vector3(x, currentBackground_y, -i), Quaternion.identity);
    }
}
