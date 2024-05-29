using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject apple;    //���� ������
    public GameObject leaf; //���� �ı� �� �����Ǵ� ������Ʈ

    private void Awake()
    {
        if(PlatformManager.Instance.appleActive)
            apple.SetActive(true);  //���� ������ �÷��� ���� Ȱ��ȭ
    }
    // Update is called once per frame
    void Update()
    {
        //���� ��ġ �ʱ�ȭ
        if (GameManager.Instance.direction == 1)    
        {
            if (transform.position.x < Camera.main.transform.position.x -10)
                transform.position = transform.position + new Vector3( 20, 0, 0);
        }
        else
        {
            if (transform.position.x > Camera.main.transform.position.x + 10)
                transform.position = transform.position + new Vector3(-20, 0, 0);
        }

        //ī�޶� ������ ����� �ı�
        if (transform.position.y > Camera.main.transform.position.y + 9f)
            Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Breaker"))        //����ڰ� ���� �ı�
        {
            Destroy(gameObject);    //���� ������Ʈ �ı�
            Destroy(collision.gameObject);  //���� ������Ʈ �ı�

            //�ܿ��� ����
            Instantiate(leaf, transform.position, Quaternion.identity);
            Instantiate(leaf, transform.position, Quaternion.identity);
            Instantiate(leaf, transform.position, Quaternion.identity);
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (!enemy.enemyHitPlayer)
            {
                Destroy(gameObject);   //���� ������Ʈ �ı�

                //�ܿ��� ����
                Instantiate(leaf, transform.position, Quaternion.identity);
                Instantiate(leaf, transform.position, Quaternion.identity);
                Instantiate(leaf, transform.position, Quaternion.identity);
            }
        }
    }
}
