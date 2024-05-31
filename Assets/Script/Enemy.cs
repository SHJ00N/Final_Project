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

    private int enemy_direction;    //�� �̵� ����
    public float enemy_speed;  //�� �̵� �ӵ�
    public float preEnemy_speed;

    public float enemyActiveTime = 3f; //���� ���� ���ð�

    public float enemyHeight = 0.1f;    //�� ž
    public float enemyWidth = 0.45f;    //�� �� �� ����
    public float rayPos_y = 0.45f;  //�� ��ġ ���� ray�� y ��ġ

    private bool isGrounded = false;
    private bool isFalling;
    public bool enemyHitEnable = true;  //�� �浹 ���� ����
    public bool enemyHitPlayer = true;  //�� �� �÷��̾� �浹 ���� ����
    private int countHitPlatform = 0;   //�� �� �÷��̾� �浹 �� �÷����� �浹�� Ƚ��
    private bool isDie = false; //��� ����

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        playerCollider2D = GameObject.Find("Player").GetComponent<Collider2D>();    //�÷��̾��� collider
        switch (GameManager.Instance.score / 50000) {
            //�� �̵� �ӵ� ������ �Ҵ�
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
        enemy_direction = UnityEngine.Random.Range(0, 2) * 2 - 1;   //1:������, -1:����
    }
    void Update()
    {
        CheckIsGrounded();  //fall ���� Ȯ��
        
        //��� �ִϸ��̼��� ������ ������Ʈ �ı�
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                Destroy(gameObject);
        //���� ī�޶� ������ ������ �ı�
        if ((transform.position.y > Camera.main.transform.position.y + 10f) || (transform.position.y < Camera.main.transform.position.y - 12f))
            Destroy(gameObject);

        if(!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * enemy_direction * enemy_speed *Time.deltaTime); //�� �̵�

        //�� ��ġ �ʱ�ȭ
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

    private void CheckIsGrounded()  //���� Fall ���� Ȯ�� �Լ�
    {
        // �� �� �÷��� Ȯ���� ���� Ray
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, 1f, LayerMask.GetMask("Platform"));  //�浹ü Ȯ��
        if (rayHit.collider == null)    //�� �ؿ� �÷����� ������ Fall ���·� ����
        {
            Debug.Log("�� �浹 ����");
            isGrounded = false;
            isFalling = true;
            gameObject.layer = LayerMask.NameToLayer("FallingEnemy");   //���̾� ����ũ ����
        } else
        {
            if (Mathf.Abs((transform.position.y - rayPos_y) - rayHit.transform.position.y) < 0.5f && isFalling) //�� �ؿ� �÷����� �ְ� Fall����
            {
                Debug.Log("�� �浹 ��");
                isFalling = false;
                gameObject.layer = LayerMask.NameToLayer("Enemy");  //���̾� ����ũ ���� ����
            }
        }
    }
    public IEnumerator OnEnemyActive(Collider2D collisionCollider) //�� Ȱ�� Ȱ��ȭ
    {
        yield return new WaitForSeconds(enemyActiveTime);
        enemy_speed = preEnemy_speed;   //���� �ӵ� ���Ҵ�
        //������Ʈ ���� ����ȭ
        if (transform.localScale.x < 0)  
            transform.localScale = new Vector3(-1 * transform.localScale.x, -1 * transform.localScale.y, transform.localScale.z);
        rayPos_y = 0.45f;
        //collider Ȱ��ȭ
        Physics2D.IgnoreCollision(_collider2D, playerCollider2D, false);
        if(collisionCollider != null)
            Physics2D.IgnoreCollision(_collider2D, collisionCollider, false);
        enemyHitEnable = true;  //�浹 �ѱ�
    }

    public void OffEnemyActive()    //�� Ȱ�� ��Ȱ��ȭ
    {
        enemyHitEnable = false;
        preEnemy_speed = enemy_speed;   //���� �ӵ� ����
        enemy_speed = 0f;   //������ ����
        Physics2D.IgnoreCollision(_collider2D, playerCollider2D);
        //������Ʈ ���� ������
        if (transform.localScale.x > 0)
            transform.localScale = new Vector3(-1 * transform.localScale.x, -1 * transform.localScale.y, transform.localScale.z);
        rayPos_y = 0.1f;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform") && !isDie && !isGrounded)
        {
            Debug.Log("�� �÷��� �浹!");
             if (enemyHitPlayer)
             {
                 isGrounded = true;
             }
             else //�÷��̾ fall ���� �� �� �浹
              {
                //�÷����� 3�� �̻� �浹�ϸ� �ı�
                 if (countHitPlatform > 2)
                 {
                    //�浹 ��Ȱ��ȭ
                    _collider2D.enabled = false;
                    _rigidbody2D.simulated = false;
                    _animator.SetTrigger("Die");    //��� �ִϸ��̼� ����
                    isDie = true;
                 }
                 countHitPlatform++;
                 Destroy(collision.gameObject); //�÷��� �ı�
             }
        }


        if (collision.collider.CompareTag("Enemy") && enemyHitEnable) //���� �� �浹
        {
            OffEnemyActive();   //Ȱ�� ��Ȱ��ȭ
            //���� �Ÿ� ����
            Vector2 moveDir = new Vector2((transform.position.x - collision.transform.position.x) * 2f, 0f);
            Physics2D.IgnoreCollision(_collider2D, collision.collider);
            _rigidbody2D.AddForce(moveDir * 3f, ForceMode2D.Impulse);
            //���� �ð� �� ����ȭ
            StartCoroutine(OnEnemyActive(collision.collider));
        }
    }   
}
