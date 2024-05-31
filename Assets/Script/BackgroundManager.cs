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
    float background_startPos_x;
    float background_yDistance = 25.5f;
    float background_xDistance = 24f;
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
        background_startPos_x = Camera.main.transform.position.x;
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
            for (float x = background_startPos_x; x <= background_startPos_x + background_xDistance + 1f; x += background_xDistance)
                for (int i = 0; i < 5; i++)
                {
                    Instantiate(backgrounds[i], new Vector3(x, y, -i), Quaternion.identity);
                }

        currentBackground_y -= background_yDistance;    //��� ���� ��ġ �̵�
    }
    public void Make_background()   // ��� �߰� ���� �Լ�
    {
        background_startPos_x = Camera.main.transform.position.x;
        //��� ���� y��ġ �̵� �� 2�� ����
        currentBackground_y -= background_yDistance;
        for (float x = background_startPos_x; x <= background_startPos_x + background_xDistance + 1f; x += background_xDistance)
            for (int i = 0; i < 5; i++)
                Instantiate(backgrounds[i], new Vector3(x, currentBackground_y, -i), Quaternion.identity);
    }
}
