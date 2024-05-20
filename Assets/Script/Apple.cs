using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    private bool hitPlayer = false; //중복 실행 방지용

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hitPlayer)       //플레이어와 충돌
        {
            hitPlayer = true;
            Destroy(gameObject);    //오브젝트 파괴
            GameManager.Instance.score += 1000; //점수 증가
            hitPlayer = false;
        }
    }
}
