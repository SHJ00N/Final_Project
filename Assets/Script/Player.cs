using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject shield;
    public GameObject breaker;  //발판 파괴 오브젝트
    public Transform breakPos;  //파괴 위치
    public Transform rayPos;    //fall상태일 때 땅과의 거리를 계산하기 위한 ray를 그리는 위치

    private Collider2D _collider2D;
    private Animator _animator; //플레이어 애니메이션 컨트롤러
    private Rigidbody2D _rigid;

    private float prePlayerPosition_y;
    private bool isGrounded = false;    //현재 위치 상태
    private bool hitPlatform;   //플랫폼 충돌 상태
    public bool playerHitEnable = true;  //적과의 충돌 상태
    private float player_speed = 5f;
    private float preComboCountTime;
    private float ComboTime = 2f;   //콤보 유지 시간
    private int score = 500;    //획득하는 점수

    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        preComboCountTime = -ComboTime; //초기 이전 콤보 시작 시간
        prePlayerPosition_y = transform.position.y;
    }

    void Update()
    {
        CheckIsGrounded(); //Fall 상태 확인
        if (!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * GameManager.Instance.direction * player_speed * Time.deltaTime);
        //스페이스 입력 시 발판 파괴
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !GameManager.Instance.gameEnd)
        {
            float currnetComboCountTime = Time.time;
            Instantiate(breaker, breakPos.position,Quaternion.identity);

            //점수 및 콤보 증가
            if (currnetComboCountTime - preComboCountTime <= ComboTime)     // 콤보 유지 시간 안에 스페이스바 입력 시 콤보 증가
            {
                if (GameManager.Instance.combo < 99) GameManager.Instance.combo++;  //최대 콤보 99로 제한
                StartCoroutine(GameManager.Instance.ComboEffect()); //콤보 효과 연출
            }
            else GameManager.Instance.combo = 1;
            preComboCountTime = currnetComboCountTime;  //이전 콤보 시작 시간 변경
            GameManager.Instance.score = GameManager.Instance.score + score + (score * GameManager.Instance.combo * 1 / 10);    //콤보에 따른 추가 점수 반영

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Pierce);
        }
        //플레이어의 y좌표가 일정 이상 내려가면 적 과 플랫폼 생성
        if(Mathf.Abs(prePlayerPosition_y - transform.position.y) > PlatformManager.Instance.platform_y_distance)
        {
            PlatformManager.Instance.MakePlatform();
            EnemySpawnManager.Instance.SpawnEnemy();
            prePlayerPosition_y = transform.position.y; //이전 위치 업데이트
        }
    }
    private void CheckIsGrounded()
    {
        //착지할 위치의 발판을 가져오는 Ray를 그림
        Debug.DrawRay(rayPos.position, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rayPos.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));  //충돌체 확인
        if (rayHit.collider != null)
        {
            Debug.Log("충돌체 감지");
            //착지할 위치와 가까워 지면 발판 충돌 활성화 및 애니메이션 변경
            if (Mathf.Abs(rayPos.position.y - rayHit.transform.position.y) < 0.5f && !hitPlatform)
            {
                Debug.Log("충돌 온");
                hitPlatform = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), false);
                _animator.SetInteger("Fall", 0);
            }
        } else
        {
            isGrounded = false;
            hitPlatform = false;
            _animator.SetInteger("Fall", 1);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"));
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
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.Step);
                
                enemy.OffEnemyActive(); //적 뒤집고 활동 끄기
                enemy.enemyHitPlayer = false;
                //적을 일정거리 날림
                Vector2 movedir = new Vector2(-(transform.position.x - collision.transform.position.x) * 3f, Mathf.Abs(transform.position.y - collision.transform.position.y));
                collision.rigidbody.AddForce(movedir * 2f, ForceMode2D.Impulse);

                //플레이어
                _rigid.AddForce(Vector2.up * 50f, ForceMode2D.Force);  //일정거리 점프
                //플레이어 와 적 충돌 마스크 비활성화
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FallingEnemy"));
                shield.SetActive(true); //무적 이펙트 활성화
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

                Invoke("OnPlayerHit2Enemy", 2f);    //일정 시간 후 무적 상태 종료
            }
            else if(enemy.enemyHitEnable)
            {
                enemy.enemy_speed = 0f; //적 이동 중지
                _animator.SetTrigger("Die");    //사망 애니메이션 연출
                GameManager.Instance.gameEnd = true;    //게임 끝
                AudioManager.Instance.PlayBgm(false);
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.End);
            }
            playerHitEnable = true;
        }
    }

    private void OnPlayerHit2Enemy()
    {
        //무적 이펙트 비활성화
        shield.SetActive(false);
        //플레이어 와 적 충돌 마스크 활성화
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"),false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FallingEnemy"),false);
    }
}
