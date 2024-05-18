using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Collider2D _collider2D;
    private Rigidbody2D _rigidbody2D;

    private int enemy_direction;    //적 이동 방향
    public float enemy_speed;  //적 이동 속도
    public float preEnemy_speed;

    public float enemyActiveTime = 3f; //상태 복구 대기시간

    public float enemyHeight = 0.1f;    //적 탑
    public float enemyWidth = 0.45f;    //적 폭 및 바텀
    public float rayPos_y = 0.45f;

    private bool isGrounded = false;
    private bool isFalling;
    public bool enemyHitEnable = true;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        enemy_speed = Random.Range(0.1f, 2f); //적 이동 속도 랜덤값 할당
        enemy_direction = Random.Range(0, 2) * 2 - 1;   //1:오른쪽, -1:왼쪽
    }
    void Update()
    {
        if ((transform.position.y > Camera.main.transform.position.y + 10f))
            Destroy(gameObject);
        if(!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * enemy_direction * enemy_speed *Time.deltaTime); //적 이동
        //적 위치 초기화
        if(Mathf.Abs(transform.position.x) > Camera.main.transform.position.x + 6f)
        {
            if (enemy_direction == 1)
            {
                transform.position = new Vector3(Camera.main.transform.position.x - 5.7f, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(Camera.main.transform.position.x + 5.7f, transform.position.y, transform.position.z);
            }
        }

        // 적 Fall 상태 체크
        if (isGrounded) CheckIsGrounded();
        else OnCollPlatform();
    }

    private void CheckIsGrounded()  //적이 Fall 상태 확인 함수
    {
        // 발 밑 플랫폼 확인을 위한 Ray
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, 1f, LayerMask.GetMask("Platform"));  //충돌체 확인
        if (rayHit.collider == null)    //발 밑에 플랫폼이 없으면 Fall 상태로 변경
        {
            Debug.Log("적 충돌 오프");
            isGrounded = false;
            isFalling = true;
            _collider2D.enabled = false;
        }
    }
    private void OnCollPlatform()   //착지 시 플랫폼 충돌 활성화 함수
    {
        //착지 확인을 위한 Ray
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, 1f, LayerMask.GetMask("Platform"));  //충돌체 확인
        if (rayHit.collider != null) // 발 밑에 충돌체가 있으면
        {
            if (Mathf.Abs((transform.position.y - rayPos_y) - rayHit.transform.position.y) < 0.1f && isFalling) //발 밑에 플랫폼이 있고 Fall상태
            {
                Debug.Log("적 충돌 온");
                isFalling = false;
                _collider2D.enabled = true;
            }
        }
    }

    public IEnumerator OnEnemyActive() //적 활동 활성화
    {
        yield return new WaitForSeconds(enemyActiveTime);
        enemy_speed = preEnemy_speed;   //이전 속도 재할당
        //오브젝트 방향 정상화
        if (transform.localScale.x < 0)  
            transform.localScale = new Vector3(-1 * transform.localScale.x, -1 * transform.localScale.y, transform.localScale.z);
        rayPos_y = 0.45f;
        enemyHitEnable = true;  //충돌 켜기
    }

    public void OffEnemyActive()    //적 활동 비활성화
    {
        enemyHitEnable = false;
        preEnemy_speed = enemy_speed;   //현재 속도 저장
        enemy_speed = 0f;   //움직임 정지
        //오브젝트 방향 뒤집기
        if (transform.localScale.x > 0)
            transform.localScale = new Vector3(-1 * transform.localScale.x, -1 * transform.localScale.y, transform.localScale.z);
        rayPos_y = 0.1f;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            //착지
            isGrounded = true;
        }

        if (collision.collider.CompareTag("Enemy") && enemyHitEnable) //적과 적 충돌
        {
            OffEnemyActive();   //활동 비활성화
            //일정 거리 날라감
            Vector2 moveDir = new Vector2((transform.position.x - collision.transform.position.x) * 3f, 0f);
            _rigidbody2D.AddForce(moveDir * 3f, ForceMode2D.Impulse);
            //일정 시간 후 정상화
            StartCoroutine(OnEnemyActive());
        }
    }
    
}
