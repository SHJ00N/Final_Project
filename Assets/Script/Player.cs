using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject breaker;  //발판 파괴 오브젝트
    public Transform breakPos;  //파괴 위치
    public Transform rayPos;    //fall상태일 때 땅과의 거리를 계산하기 위한 ray를 그리는 위치

    private Collider2D _collider2D;
    private Animator _animator; //플레이어 애니메이션 컨트롤러
    private Rigidbody2D _rigid;

    private bool isGrounded = false;    //현재 위치 상태
    private bool hitPlatform;   //플랫폼 충돌 상태
    public bool playerHitEnable = true;  //적과의 충돌 상태
    private float player_speed = 5f;
    private float preComboCountTime;
    private float ComboTime = 2f;   //콤보 유지 시간
    private int score = 500;    //획득하는 점수

    private float playerHitActive = 1f;

    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        preComboCountTime = -ComboTime; //초기 이전 콤보 시작 시간
    }

    void Update()
    {
        if (!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * GameManager.Instance.direction * player_speed * Time.deltaTime);
        //스페이스 입력 시 발판 파괴
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !GameManager.Instance.gameEnd)
        {
            float currnetComboCountTime = Time.time;
            isGrounded = false;
            hitPlatform = false;

            _animator.SetInteger("Fall", 1);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), true) ;  //발판과의 충돌 비활성화
            Instantiate(breaker, breakPos.position,Quaternion.Euler(0,0,90));

            PlatformManager.Instance.MakePlatform();    //메인 카메라 밑에 예비 발판 추가 생성
            EnemySpawnManager.Instance.SpawnEnemy(); //적 스폰

            //점수 및 콤보 증가
            if (currnetComboCountTime - preComboCountTime <= ComboTime)     // 콤보 유지 시간 안에 스페이스바 입력 시 콤보 증가
            {
                if (GameManager.Instance.combo < 99) GameManager.Instance.combo++;  //최대 콤보 99로 제한
            }
            else GameManager.Instance.combo = 1;
            preComboCountTime = currnetComboCountTime;  //이전 콤보 시작 시간 변경
            GameManager.Instance.score = GameManager.Instance.score + score + (score * GameManager.Instance.combo * 1 / 10);    //콤보에 따른 추가 점수 반영
        }
        if(!isGrounded) //떨어지는 상태일 때
        {
            OutFallAnimation(); //착지 및 충돌 활성화 함수
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Platform") && !isGrounded)   //발판과 충돌
        {
            isGrounded = true;  //착지 및 달리는 상태
        }

        if (collision.collider.CompareTag("Enemy") && playerHitEnable)    //적과 충돌
        {
            playerHitEnable = false;    //플레이어 무적상태
            Enemy enemy = collision.collider.GetComponent<Enemy>(); //충돌한 적 스크립트
            //if (transform.position.y - playerHeight >= collision.transform.position.y + enemy.enemyHeight)  //플레이어가 적을 밟았을 때
            if (!hitPlatform)
            {
                enemy.OffEnemyActive(); //적 뒤집고 활동 끄기
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));    //충돌 비활성화
                //적을 일정거리 날림
                Vector2 movedir = new Vector2(-(transform.position.x - collision.transform.position.x) * 3f, Mathf.Abs(transform.position.y - collision.transform.position.y));
                collision.rigidbody.AddForce(movedir * 3f, ForceMode2D.Impulse);

                //플레이어
                _rigid.AddForce(Vector2.up * 200f, ForceMode2D.Force);  //일정거리 점프
                //방향 변경
                if (transform.localScale.x < 0) //왼쪽
                {
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    GameManager.Instance.direction = 1;
                }
                else   //오른쪽
                {
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    GameManager.Instance.direction = -1;
                }
                //일정 시간 후 적 및 플레이어 충돌 정상화
                StartCoroutine(enemy.OnEnemyActive()); 
                StartCoroutine(OnPlayerHit(collision.collider));
            }
            else if(enemy.enemyHitEnable)
            {
                //모든 오브젝트 이동 중지
                enemy.enemy_speed = 0f;
                _animator.SetTrigger("Die");
                GameManager.Instance.gameEnd = true;
            }
            playerHitEnable = true;
        }
    }
    private IEnumerator OnPlayerHit(Collider2D collider2D)  //플레이어와 적 충돌 활성화
    {
        yield return new WaitForSeconds(playerHitActive);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"),false);
    }
    private void OutFallAnimation()
    {
        //착지할 위치의 발판을 가져오는 Ray를 그림
        Debug.DrawRay(rayPos.position, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rayPos.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));  //충돌체 확인
        if (rayHit.collider != null)
        {
            Debug.Log("충돌체 감지");
            //착지할 위치와 가까워 지면 발판 충돌 활성화 및 애니메이션 변경
            if (Mathf.Abs(rayPos.position.y - rayHit.transform.position.y) < 0.05f && !hitPlatform)
            {
                Debug.Log("충돌 온");
                hitPlatform = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), false);
                _animator.SetInteger("Fall", 0);
            }
        }
    }
}
