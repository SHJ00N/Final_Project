using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBgScroll : MonoBehaviour
{
    [SerializeField] float speed;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.position.x < -6.39f)
            transform.position = transform.position + new Vector3(12.78f, 0f, 0f);
    }
}
