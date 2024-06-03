using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    private bool hitPlayer = false; //�ߺ� ���� ������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hitPlayer)       //�÷��̾�� �浹
        {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Item);
            hitPlayer = true;
            Destroy(gameObject);    //������Ʈ �ı�
            GameManager.Instance.score += 1000; //���� ����
            hitPlayer = false;
        }

        if(collision.CompareTag("Enemy") || collision.CompareTag("FallingEnemy"))
            Destroy(gameObject);
    }
}
