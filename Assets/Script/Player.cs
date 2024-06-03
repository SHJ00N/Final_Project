using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject shield;
    public GameObject breaker;  //���� �ı� ������Ʈ
    public Transform breakPos;  //�ı� ��ġ
    public Transform rayPos;    //fall������ �� ������ �Ÿ��� ����ϱ� ���� ray�� �׸��� ��ġ

    private Collider2D _collider2D;
    private Animator _animator; //�÷��̾� �ִϸ��̼� ��Ʈ�ѷ�
    private Rigidbody2D _rigid;

    private float prePlayerPosition_y;
    private bool isGrounded = false;    //���� ��ġ ����
    private bool hitPlatform;   //�÷��� �浹 ����
    public bool playerHitEnable = true;  //������ �浹 ����
    private float player_speed = 5f;
    private float preComboCountTime;
    private float ComboTime = 2f;   //�޺� ���� �ð�
    private int score = 500;    //ȹ���ϴ� ����

    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        preComboCountTime = -ComboTime; //�ʱ� ���� �޺� ���� �ð�
        prePlayerPosition_y = transform.position.y;
    }

    void Update()
    {
        CheckIsGrounded(); //Fall ���� Ȯ��
        if (!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * GameManager.Instance.direction * player_speed * Time.deltaTime);
        //�����̽� �Է� �� ���� �ı�
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !GameManager.Instance.gameEnd)
        {
            float currnetComboCountTime = Time.time;
            Instantiate(breaker, breakPos.position,Quaternion.identity);

            //���� �� �޺� ����
            if (currnetComboCountTime - preComboCountTime <= ComboTime)     // �޺� ���� �ð� �ȿ� �����̽��� �Է� �� �޺� ����
            {
                if (GameManager.Instance.combo < 99) GameManager.Instance.combo++;  //�ִ� �޺� 99�� ����
                StartCoroutine(GameManager.Instance.ComboEffect()); //�޺� ȿ�� ����
            }
            else GameManager.Instance.combo = 1;
            preComboCountTime = currnetComboCountTime;  //���� �޺� ���� �ð� ����
            GameManager.Instance.score = GameManager.Instance.score + score + (score * GameManager.Instance.combo * 1 / 10);    //�޺��� ���� �߰� ���� �ݿ�

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Pierce);
        }
        //�÷��̾��� y��ǥ�� ���� �̻� �������� �� �� �÷��� ����
        if(Mathf.Abs(prePlayerPosition_y - transform.position.y) > PlatformManager.Instance.platform_y_distance)
        {
            PlatformManager.Instance.MakePlatform();
            EnemySpawnManager.Instance.SpawnEnemy();
            prePlayerPosition_y = transform.position.y; //���� ��ġ ������Ʈ
        }
    }
    private void CheckIsGrounded()
    {
        //������ ��ġ�� ������ �������� Ray�� �׸�
        Debug.DrawRay(rayPos.position, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rayPos.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));  //�浹ü Ȯ��
        if (rayHit.collider != null)
        {
            Debug.Log("�浹ü ����");
            //������ ��ġ�� ����� ���� ���� �浹 Ȱ��ȭ �� �ִϸ��̼� ����
            if (Mathf.Abs(rayPos.position.y - rayHit.transform.position.y) < 0.5f && !hitPlatform)
            {
                Debug.Log("�浹 ��");
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
        if(collision.collider.CompareTag("Platform") && !isGrounded)   //���ǰ� �浹
        {
            isGrounded = true;  //���� �� �޸��� ����
        }

        if (collision.collider.CompareTag("Enemy") && playerHitEnable)    //���� �浹
        {
            playerHitEnable = false;    //�÷��̾� ��������
            Enemy enemy = collision.collider.GetComponent<Enemy>(); //�浹�� �� ��ũ��Ʈ
            //if (transform.position.y - playerHeight >= collision.transform.position.y + enemy.enemyHeight)  //�÷��̾ ���� ����� ��
            if (!hitPlatform)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.Step);
                
                enemy.OffEnemyActive(); //�� ������ Ȱ�� ����
                enemy.enemyHitPlayer = false;
                //���� �����Ÿ� ����
                Vector2 movedir = new Vector2(-(transform.position.x - collision.transform.position.x) * 3f, Mathf.Abs(transform.position.y - collision.transform.position.y));
                collision.rigidbody.AddForce(movedir * 2f, ForceMode2D.Impulse);

                //�÷��̾�
                _rigid.AddForce(Vector2.up * 50f, ForceMode2D.Force);  //�����Ÿ� ����
                //�÷��̾� �� �� �浹 ����ũ ��Ȱ��ȭ
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FallingEnemy"));
                shield.SetActive(true); //���� ����Ʈ Ȱ��ȭ
                //���� ����
                if (transform.localScale.x < 0) //����
                {
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    GameManager.Instance.direction = 1;
                }
                else   //������
                {
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    GameManager.Instance.direction = -1;
                }

                Invoke("OnPlayerHit2Enemy", 2f);    //���� �ð� �� ���� ���� ����
            }
            else if(enemy.enemyHitEnable)
            {
                enemy.enemy_speed = 0f; //�� �̵� ����
                _animator.SetTrigger("Die");    //��� �ִϸ��̼� ����
                GameManager.Instance.gameEnd = true;    //���� ��
                AudioManager.Instance.PlayBgm(false);
                AudioManager.Instance.PlaySfx(AudioManager.Sfx.End);
            }
            playerHitEnable = true;
        }
    }

    private void OnPlayerHit2Enemy()
    {
        //���� ����Ʈ ��Ȱ��ȭ
        shield.SetActive(false);
        //�÷��̾� �� �� �浹 ����ũ Ȱ��ȭ
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"),false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("FallingEnemy"),false);
    }
}
