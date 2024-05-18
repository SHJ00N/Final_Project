using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject leaf; //���� �ı� �� �����Ǵ� ������Ʈ
    private int direction;  //�̵� ����

    // Update is called once per frame
    void Update()
    {
        direction = PlatformManager.Instance.platformDirection; //���� �̵� ���� ������Ʈ
        if(!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * direction * PlatformManager.Instance.platform_speed * Time.deltaTime);   //���� �̵�

        //���� ��ġ �ʱ�ȭ
        if (direction == -1)    
        {
            if (transform.position.x <  -10)
                transform.position = transform.position + new Vector3( 20, 0, 0);
        }
        else
        {
            if (transform.position.x >10)
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
    }
}