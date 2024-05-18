using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour
{
    private int direction;  //��� �̵� ����
    private int background_number;  //���� ��� ��ȣ(0~4)
    // Start is called before the first frame update
    void Awake()
    {
        background_number = BackgroundManager.Instance.currentBackgroundNum;
    }

    // Update is called once per frame
    void Update()
    {
        direction = BackgroundManager.Instance.background_direction;    //��� �̵����� ������Ʈ
        if (!GameManager.Instance.gameEnd)
            transform.Translate(Vector3.right * direction * BackgroundManager.Instance.background_speed[background_number] * Time.deltaTime); //��� �̵�
        //��� ��ġ �ʱ�ȭ
        if (direction == -1)
        {
            if (transform.position.x <  -24)
                transform.position = transform.position + new Vector3(+48, 0, 0);
        }
        else
        {
            if (transform.position.x > 24)
                transform.position = transform.position + new Vector3(-48, 0, 0);
        }

        //ī�޶� ������ ����� �ı�
        if (transform.position.y - 13f > Camera.main.transform.position.y + 9f)
        {
            Destroy(gameObject);
        }
    }
}
