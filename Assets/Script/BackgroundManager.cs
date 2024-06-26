using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    public static BackgroundManager Instance;   //배경 생성을 담당하는 싱글톤
    public GameObject[] backgrounds;    //배경 오브젝트

    //배경 생성 위치
    float currentBackground_y;
    float background_startPos_x;
    float background_yDistance = 25.5f;
    float background_xDistance = 24f;
    private void Awake()
    {
        if (Instance == null)   //싱글톤
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        //현재 카메라 위치를 기준으로 배경 생성
        background_startPos_x = Camera.main.transform.position.x;
        currentBackground_y = Camera.main.transform.position.y+1f;  
        Init_background();
    }

    // Update is called once per frame
    void Update()
    {
        //카메라 영역에 들어올 예비 배경 미리 생성
        if (currentBackground_y >Camera.main.transform.position.y)
            Make_background();
    }
    void Init_background()  //게임 시작할 때 배경 생성 함수
    {
        //4장의 배경 생성
        for (float y = currentBackground_y; y >= -background_yDistance; y -= background_yDistance)
            for (float x = background_startPos_x; x <= background_startPos_x + background_xDistance + 1f; x += background_xDistance)
                for (int i = 0; i < 5; i++)
                {
                    Instantiate(backgrounds[i], new Vector3(x, y, -i), Quaternion.identity);
                }

        currentBackground_y -= background_yDistance;    //배경 생성 위치 이동
    }
    public void Make_background()   // 배경 추가 생성 함수
    {
        background_startPos_x = Camera.main.transform.position.x;
        //배경 생성 y위치 이동 후 2장 생성
        currentBackground_y -= background_yDistance;
        for (float x = background_startPos_x; x <= background_startPos_x + background_xDistance + 1f; x += background_xDistance)
            for (int i = 0; i < 5; i++)
                Instantiate(backgrounds[i], new Vector3(x, currentBackground_y, -i), Quaternion.identity);
    }
}
