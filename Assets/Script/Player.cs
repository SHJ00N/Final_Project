using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject breaker;  //���� �ı� ������Ʈ
    public Transform breakPos;  //�ı� ��ġ
    public Transform rayPos;    //fall������ �� ������ �Ÿ��� ����ϱ� ���� ray�� �׸��� ��ġ

    private Collider2D _collider2D;
    private Animator _animator; //�÷��̾� �ִϸ��̼� ��Ʈ�ѷ�
    private Rigidbody2D _rigid;

    private bool isGrounded = false;    //���� ��ġ ����
    private bool hitPlatform;   //�÷��� �浹 ����
    public bool playerHitEnable = true;  //������ �浹 ����

    private float playerHitActive = 1f;

    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        //�����̽� �Է� �� ���� �ı�
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !GameManager.Instance.gameEnd)
        {
            isGrounded = false;
            hitPlatform = false;
            _animator.SetInteger("Fall", 1);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), true) ;  //���ǰ��� �浹 ��Ȱ��ȭ
            Instantiate(breaker, breakPos.position,Quaternion.Euler(0,0,90));
            PlatformManager.Instance.MakePlatform();    //���� ī�޶� �ؿ� ���� ���� �߰� ����
            EnemySpawnManager.Instance.SpawnEnemy(); //�� ����
        }
        if(!isGrounded) //�������� ������ ��
        {
            OutFallAnimation(); //���� �� �浹 Ȱ��ȭ �Լ�
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
                enemy.OffEnemyActive(); //�� ������� Ȱ�� ����
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));    //�浹 ��Ȱ��ȭ
                //���� �����Ÿ� ����
                Vector2 movedir = new Vector2(-(transform.position.x - collision.transform.position.x) * 3f, Mathf.Abs(transform.position.y - collision.transform.position.y));
                collision.rigidbody.AddForce(movedir * 3f, ForceMode2D.Impulse);

                //�÷��̾�
                _rigid.AddForce(Vector2.up * 200f, ForceMode2D.Force);  //�����Ÿ� ����
                //���� ����
                if (transform.localScale.x < 0) //����
                {
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    PlatformManager.Instance.platformDirection = -1;
                    BackgroundManager.Instance.background_direction = -1;
                }
                else   //������
                {
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    PlatformManager.Instance.platformDirection = 1;
                    BackgroundManager.Instance.background_direction = 1;
                }
                //���� �ð� �� �� �� �÷��̾� �浹 ����ȭ
                StartCoroutine(enemy.OnEnemyActive()); 
                StartCoroutine(OnPlayerHit(collision.collider));
            }
            else if(enemy.enemyHitEnable)
            {
                //��� ������Ʈ �̵� ����
                enemy.enemy_speed = 0f;
                PlatformManager.Instance.platform_speed = 0f;
                for (int i = 0; i < 5; i++)
                BackgroundManager.Instance.background_speed[i] = 0f;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));
                _animator.SetTrigger("Die");
                GameManager.Instance.gameEnd = true;
            }
            playerHitEnable = true;
        }
    }
    private IEnumerator OnPlayerHit(Collider2D collider2D)  //�÷��̾�� �� �浹 Ȱ��ȭ
    {
        yield return new WaitForSeconds(playerHitActive);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"),false);
    }
    private void OutFallAnimation()
    {
        //������ ��ġ�� ������ �������� Ray�� �׸�
        Debug.DrawRay(rayPos.position, Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rayPos.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));  //�浹ü Ȯ��
        if (rayHit.collider != null)
        {
            Debug.Log("�浹ü ����");
            //������ ��ġ�� ����� ���� ���� �浹 Ȱ��ȭ �� �ִϸ��̼� ����
            if (Mathf.Abs(rayPos.position.y - rayHit.transform.position.y) < 0.05f && !hitPlatform)
            {
                Debug.Log("�浹 ��");
                hitPlatform = true;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), false);
                _animator.SetInteger("Fall", 0);
            }
        }
    }
}