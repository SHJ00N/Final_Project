using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject leaf; //발판 파괴 후 생성되는 오브젝트
    private int direction;  //이동 방향

    // Update is called once per frame
    void Update()
    {
        direction = PlatformManager.Instance.platformDirection; //발판 이동 방향 업데이트
        if(!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * direction * PlatformManager.Instance.platform_speed * Time.deltaTime);   //발판 이동

        //발판 위치 초기화
        if (direction == -1)    
        {
            if (transform.position.x <  -10)
                transform.position = transform.position + new Vector3( 20, 0, 0);
        }
        else
        {
            if (transform.position.x >10)
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
    }
}
