using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour
{
    private int direction;  //배경 이동 방향
    private int background_number;  //현재 배경 번호(0~4)
    // Start is called before the first frame update
    void Awake()
    {
        background_number = BackgroundManager.Instance.currentBackgroundNum;
    }

    // Update is called once per frame
    void Update()
    {
        direction = BackgroundManager.Instance.background_direction;    //배경 이동방향 업데이트
        if (!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * direction * BackgroundManager.Instance.background_speed[background_number] * Time.deltaTime); //배경 이동
        //배경 위치 초기화
        if (direction == -1)
        {
            if (transform.position.x <  -24)
                transform.position = transform.position + new Vector3(+48, 0, 0);
        }
        else
        {
            if (transform.position.x > 24)
                transform.position = transform.position + new Vector3(-48, 0, 0);
        }

        //카메라 영역을 벗어나면 파괴
        if (transform.position.y - 13f > Camera.main.transform.position.y + 9f)
        {
            Destroy(gameObject);
        }
    }
}
