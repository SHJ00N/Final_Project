using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    Rigidbody2D _rigid;
    Collider2D _collider;

    private int leaf_direction; //���ư� ����(����, ������)
    private int leaf_force = 5; //������ �� ũ��

    //���ư� ����
    private float moveDir_x;    
    private float moveDir_y;

    private int coll_count = 0; //�浹 Ƚ��
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _rigid = GetComponent<Rigidbody2D>();

        transform.localScale = Vector3.one * (Random.Range(1,4)*0.5f + 0.5f) ;  //���� ũ�� ����
        //���� ����
        leaf_direction = Random.Range(0, 2) * 2 - 1;
        moveDir_x = Random.Range(0f, 0.5f) * leaf_direction;
        moveDir_y = Random.Range(0.5f, 1.5f);

        _rigid.AddForce(new Vector2(moveDir_x, moveDir_y)*leaf_force, ForceMode2D.Impulse); //������ �� ����
    }

    // Update is called once per frame
    void Update()
    {
        //ī�޶� ������ ����� ������Ʈ �ı�
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
            coll_count++;   //�浹 Ƚ�� ����

            //���� ũ�⿡ ���� ������Ʈ �ı�
            if (coll_count == 3)
            {
                if(moveDir_y > 0.3f)
                    _collider.enabled = false;  //�浹 ��Ȱ��ȭ
                else
                    Destroy(gameObject);
            }
            else
            {
                //���� ũ�� ���� �� ������ �� ����
                moveDir_x *= 0.75f;
                moveDir_y *= 0.75f;
                _rigid.AddForce(new Vector2(moveDir_x, moveDir_y) * leaf_force, ForceMode2D.Impulse);
            }
        }
    }
}
