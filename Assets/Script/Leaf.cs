using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    Rigidbody2D _rigid;
    Collider2D _collider;

    private int leaf_direction; //날아갈 방향(왼쪽, 오른쪽)
    private int leaf_force = 5; //물리적 힘 크기

    //날아갈 방향
    private float moveDir_x;    
    private float moveDir_y;

    private int coll_count = 0; //충돌 횟수
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _rigid = GetComponent<Rigidbody2D>();

        transform.localScale = Vector3.one * (Random.Range(1,4)*0.5f + 0.5f) ;  //랜덤 크기 설정
        //랜덤 방향
        leaf_direction = Random.Range(0, 2) * 2 - 1;
        moveDir_x = Random.Range(0f, 0.5f) * leaf_direction;
        moveDir_y = Random.Range(0.5f, 1.5f);

        _rigid.AddForce(new Vector2(moveDir_x, moveDir_y)*leaf_force, ForceMode2D.Impulse); //물리적 힘 가함
    }

    // Update is called once per frame
    void Update()
    {
        //카메라 영역을 벗어나면 오브젝트 파괴
        if(transform.position.y < Camera.main.transform.position.y - 8f)
            Destroy(gameObject);

            if (transform.position.x > Camera.main.transform.position.x + 5.2f)
            {
                transform.position = new Vector3(Camera.main.transform.position.x - 5f, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < Camera.main.transform.position.x - 5.2f)
            {
                transform.position = new Vector3(Camera.main.transform.position.x + 5f, transform.position.y, transform.position.z);
            }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            coll_count++;   //충돌 횟수 증가

            //방향 크기에 따른 오브젝트 파괴
            if (coll_count == 3)
            {
                if(moveDir_y > 0.3f)
                    _collider.enabled = false;  //충돌 비활성화
                else
                    Destroy(gameObject);
            }
            else
            {
                //방향 크기 감소 후 물리적 힘 가함
                moveDir_x *= 0.75f;
                moveDir_y *= 0.75f;
                _rigid.AddForce(new Vector2(moveDir_x, moveDir_y) * leaf_force, ForceMode2D.Impulse);
            }
        }
    }
}
