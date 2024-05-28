using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour
{
    [SerializeField] int direction, speed;
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            direction = GameManager.Instance.direction;
            speed = 0;
        }
        else direction = 1;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left*speed*direction*Time.deltaTime);
        //배경 위치 초기화
        if (direction == 1)
        {
            if (transform.position.x <  Camera.main.transform.position.x - 24)
                transform.position = transform.position + new Vector3(+48, 0, 0);
        }
        else
        {
            if (transform.position.x > Camera.main.transform.position.x + 24)
                transform.position = transform.position + new Vector3(-48, 0, 0);
        }

        //카메라 영역을 벗어나면 파괴
        if (transform.position.y - 13f > Camera.main.transform.position.y + 9f)
        {
            Destroy(gameObject);
        }
    }
}
