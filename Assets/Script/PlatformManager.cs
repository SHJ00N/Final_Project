using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance; //발판 생성을 담당하는 싱글톤
    public GameObject[] platform;   //발판 오브젝트

    public bool appleActive;
    //발판 생성 위치 정보
    float platform_x_distance = 0.85f;
    float platform_y_distance = 2.55f;
    float platform_x_startPos = -5f;
    float platform_x_endPos = 14.57f;
    float platform_y_startPos = 0f;
    float platform_y_endPos = -10.2f;

    public int platformDirection = -1;  //발판 이동 방향
    public float platform_speed = 3f;   //발판 이동 속도
    private void Awake()
    {
        if (Instance == null)   //싱글톤
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        InitPlatform(); //게임 시작 시 발판 생성
    }
    private void InitPlatform() //초기 발판 생성 함수
    {
        for(float y = platform_y_startPos; y >= platform_y_endPos; y-= platform_y_distance)
        {
            for(float x = platform_x_startPos; x <= platform_x_endPos; x += platform_x_distance)
            {
                //점수 아이템 랜덤 활성화
                int appleProbability = Random.Range(0, 100);
                if (appleProbability < 5)
                    appleActive = true;
                //3종류의 발판을 랜덤하게 생성
                int randomValue = Random.Range(0, 3);
                Instantiate(platform[randomValue],new Vector2(x, y), Quaternion.identity);
                //점수아이템 설정 기본 상태로 변경
                if (appleActive)
                    appleActive = false;
            }
        }
    }
    public void MakePlatform()  //발판 추가 함수
    {
        // 발판 생성 y위치 이동 후 생성
        platform_y_endPos -= platform_y_distance;
        for (float x = platform_x_startPos; x <= platform_x_endPos; x += platform_x_distance)
        {
            //점수 아이템 랜덤 활성화
            int appleProbability = Random.Range(0, 100);
            if (appleProbability < 5)
                appleActive = true;
            //3종류의 발판을 랜덤하게 생성
            int randomValue = Random.Range(0, 3);
            Instantiate(platform[randomValue], new Vector2(x, platform_y_endPos), Quaternion.identity);
            //점수아이템 설정 기본 상태로 변경
            if (appleActive)
                appleActive = false;
        }
    }
}
