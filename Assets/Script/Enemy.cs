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
    public float rayPos_y = 0.45f;

    private bool isGrounded = false;
    private bool isFalling;
    public bool enemyHitEnable = true;
    public bool enemyHitPlayer = true;
    private int countHitPlatform = 0;
    private bool isDie = false;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        playerCollider2D = GameObject.Find("Player").GetComponent<Collider2D>();
        enemy_speed = UnityEngine.Random.Range(0.1f, 2f); //�� �̵� �ӵ� ������ �Ҵ�
        enemy_direction = UnityEngine.Random.Range(0, 2) * 2 - 1;   //1:������, -1:����
    }
    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                Destroy(gameObject);
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

        // �� Fall ���� üũ
        if (isGrounded) CheckIsGrounded();
        else OnCollPlatform();
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
            _collider2D.enabled = false;
        }
    }
    private void OnCollPlatform()   //���� �� �÷��� �浹 Ȱ��ȭ �Լ�
    {
        //���� Ȯ���� ���� Ray
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - rayPos_y), Vector2.down, 1f, LayerMask.GetMask("Platform"));  //�浹ü Ȯ��
        if (rayHit.collider != null) // �� �ؿ� �浹ü�� ������
        {
            if (Mathf.Abs((transform.position.y - rayPos_y) - rayHit.transform.position.y) < 0.1f && isFalling) //�� �ؿ� �÷����� �ְ� Fall����
            {
                Debug.Log("�� �浹 ��");
                isFalling = false;
                _collider2D.enabled = true;
            }
        }
    }

    public IEnumerator OnEnemyActive() //�� Ȱ�� Ȱ��ȭ
    {
        yield return new WaitForSeconds(enemyActiveTime);
        enemy_speed = preEnemy_speed;   //���� �ӵ� ���Ҵ�
        //������Ʈ ���� ����ȭ
        if (transform.localScale.x < 0)  
            transform.localScale = new Vector3(-1 * transform.localScale.x, -1 * transform.localScale.y, transform.localScale.z);
        rayPos_y = 0.45f;
        Physics2D.IgnoreCollision(_collider2D, playerCollider2D, false);
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
             else
              {
                 if (countHitPlatform > 2)
                 {
                    _collider2D.enabled = false;
                    _rigidbody2D.simulated = false;
                    _animator.SetTrigger("Die");
                    isDie = true;
                 }
                 countHitPlatform++;
                 Destroy(collision.gameObject);
             }
        }


        if (collision.collider.CompareTag("Enemy") && enemyHitEnable) //���� �� �浹
        {
            OffEnemyActive();   //Ȱ�� ��Ȱ��ȭ
            //���� �Ÿ� ����
            Vector2 moveDir = new Vector2((transform.position.x - collision.transform.position.x) * 3f, 0f);
            _rigidbody2D.AddForce(moveDir * 3f, ForceMode2D.Impulse);
            //���� �ð� �� ����ȭ
            StartCoroutine(OnEnemyActive());
        }
    }   
}
