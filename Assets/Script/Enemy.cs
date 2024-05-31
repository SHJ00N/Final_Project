using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Collider2D _collider2D;
    private Collider2D playerCollider2D;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;

    private int enemy_direction;    //적 이동 방향
    public float enemy_speed;  //적 이동 속도
    public float preEnemy_speed;

    public float enemyActiveTime = 3f; //상태 복구 대기시간

    public float enemyHeight = 0.1f;    //적 탑
    public float enemyWidth = 0.45f;    //적 폭 및 바텀
    public float rayPos_y = 0.45f;  //적 위치 기준 ray의 y 위치

    private bool isGrounded = false;
    private bool isFalling;
    public bool enemyHitEnable = true;  //적 충돌 가능 여부
    public bool enemyHitPlayer = true;  //적 과 플레이어 충돌 가능 여부
    private int countHitPlatform = 0;   //적 과 플레이어 충돌 후 플랫폼과 충돌한 횟수
    private bool isDie = false; //사망 여부

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        playerCollider2D = GameObject.Find("Player").GetComponent<Collider2D>();    //플레이어의 collider
        switch (GameManager.Instance.score / 50000) {
            //적 이동 속도 랜덤값 할당
            case 0:
                enemy_speed = UnityEngine.Random.Range(0.1f, 1f);
                break;
            case 1:
                enemy_speed = UnityEngine.Random.Range(1f, 1.5f);
                break;
            case 2:
                enemy_speed = UnityEngine.Random.Range(1.5f, 2f);
                break;
            case 3:
                enemy_speed = UnityEngine.Random.Range(2f, 2.5f);
                break;
            default:
                enemy_speed = 2.5f;
                break;
        }
        enemy_direction = UnityEngine.Random.Range(0, 2) * 2 - 1;   //1:오른쪽, -1:왼쪽
    }
    void Update()
    {
        CheckIsGrounded();  //fall 상태 확인
        
        //사망 애니메이션이 끝나면 오브젝트 파괴
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                Destroy(gameObject);
        //메인 카메라 밖으로 나가면 파괴
        if ((transform.position.y > Camera.main.transform.position.y + 10f) || (transform.position.y < Camera.main.transform.position.y - 12f))
            Destroy(gameObject);

        if(!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * enemy_direction * enemy_speed *Time.deltaTime); //적 이동

        //적 위치 초기화
            if (GameManager.Instance.direction == 1)
            {
                if(transform.position.x < Camera.main.transform.position.x - 6f)
                    transform.position = new Vector3(Camera.main.transform.position.x + 5.5f, transform.position.y, transform.position.z);
            }
            else
            {
                if (transform.position.x > Camera.main.transform.position.x + 6f)
                    transform.position = new Vector3(Camera.main.transform.position.x - 5.5f, transform.position.y, transform.position.z);
            }
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
            gameObject.layer = LayerMask.NameToLayer("FallingEnemy");   //레이어 마스크 변경
        } else
        {
            if (Mathf.Abs((transform.position.y - rayPos_y) - rayHit.transform.position.y) < 0.5f && isFalling) //발 밑에 플랫폼이 있고 Fall상태
            {
                Debug.Log("적 충돌 온");
                isFalling = false;
                gameObject.layer = LayerMask.NameToLayer("Enemy");  //레이어 마스크 원상 복구
            }
        }
    }
    public IEnumerator OnEnemyActive(Collider2D collisionCollider) //적 활동 활성화
    {
        yield return new WaitForSeconds(enemyActiveTime);
        enemy_speed = preEnemy_speed;   //이전 속도 재할당
        //오브젝트 방향 정상화
        if (transform.localScale.x < 0)  
            transform.localScale = new Vector3(-1 * transform.localScale.x, -1 * transform.localScale.y, transform.localScale.z);
        rayPos_y = 0.45f;
        //collider 활성화
        Physics2D.IgnoreCollision(_collider2D, playerCollider2D, false);
        if(collisionCollider != null)
            Physics2D.IgnoreCollision(_collider2D, collisionCollider, false);
        enemyHitEnable = true;  //충돌 켜기
    }

    public void OffEnemyActive()    //적 활동 비활성화
    {
        enemyHitEnable = false;
        preEnemy_speed = enemy_speed;   //현재 속도 저장
        enemy_speed = 0f;   //움직임 정지
        Physics2D.IgnoreCollision(_collider2D, playerCollider2D);
        //오브젝트 방향 뒤집기
        if (transform.localScale.x > 0)
            transform.localScale = new Vector3(-1 * transform.localScale.x, -1 * transform.localScale.y, transform.localScale.z);
        rayPos_y = 0.1f;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") && !isDie && !isGrounded)
        {
            Debug.Log("적 플랫폼 충돌!");
             if (enemyHitPlayer)
             {
                 isGrounded = true;
             }
             else //플레이어가 fall 상태 일 때 충돌
              {
                //플랫폼과 3번 이상 충돌하면 파괴
                 if (countHitPlatform > 2)
                 {
                    //충돌 비활성화
                    _collider2D.enabled = false;
                    _rigidbody2D.simulated = false;
                    _animator.SetTrigger("Die");    //사망 애니메이션 연출
                    isDie = true;
                 }
                 countHitPlatform++;
                 Destroy(collision.gameObject); //플랫폼 파괴
             }
        }


        if (collision.collider.CompareTag("Enemy") && enemyHitEnable) //적과 적 충돌
        {
            OffEnemyActive();   //활동 비활성화
            //일정 거리 날라감
            Vector2 moveDir = new Vector2((transform.position.x - collision.transform.position.x) * 2f, 0f);
            Physics2D.IgnoreCollision(_collider2D, collision.collider);
            _rigidbody2D.AddForce(moveDir * 3f, ForceMode2D.Impulse);
            //일정 시간 후 정상화
            StartCoroutine(OnEnemyActive(collision.collider));
        }
    }   
}
