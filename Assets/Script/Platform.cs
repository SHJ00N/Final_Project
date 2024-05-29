using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject apple;    //점수 아이템
    public GameObject leaf; //발판 파괴 후 생성되는 오브젝트

    private void Awake()
    {
        if(PlatformManager.Instance.appleActive)
            apple.SetActive(true);  //점수 아이템 플랫폼 위에 활성화
    }
    // Update is called once per frame
    void Update()
    {
        //발판 위치 초기화
        if (GameManager.Instance.direction == 1)    
        {
            if (transform.position.x < Camera.main.transform.position.x -10)
                transform.position = transform.position + new Vector3( 20, 0, 0);
        }
        else
        {
            if (transform.position.x > Camera.main.transform.position.x + 10)
                transform.position = transform.position + new Vector3(-20, 0, 0);
        }

        //카메라 영역을 벗어나면 파괴
        if (transform.position.y > Camera.main.transform.position.y + 9f)
            Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Breaker"))        //사용자가 발판 파괴
        {
            Destroy(gameObject);    //발판 오브젝트 파괴
            Destroy(collision.gameObject);  //공격 오브젝트 파괴

            //잔여물 생성
            Instantiate(leaf, transform.position, Quaternion.identity);
            Instantiate(leaf, transform.position, Quaternion.identity);
            Instantiate(leaf, transform.position, Quaternion.identity);
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (!enemy.enemyHitPlayer)
            {
                Destroy(gameObject);   //발판 오브젝트 파괴

                //잔여물 생성
                Instantiate(leaf, transform.position, Quaternion.identity);
                Instantiate(leaf, transform.position, Quaternion.identity);
                Instantiate(leaf, transform.position, Quaternion.identity);
            }
        }
    }
}
