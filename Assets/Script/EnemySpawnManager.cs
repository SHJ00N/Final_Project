using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;   //적 스폰 매니저 싱글톤
    public GameObject[] enemy;  //적 오브젝트

    float spawnPos_y = -1.55f;
    float spawnPos_x;
    float spawnPos_y_distance = 2.55f;
    float spawnPos_y_end = -9.2f;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        spawnPos_x = Camera.main.transform.position.x - 6f;
    }
    // Start is called before the first frame update
    void Start()
    {
        for (; spawnPos_y >= spawnPos_y_end; spawnPos_y -= spawnPos_y_distance)
        {
            int randomValue = Random.Range(0, 2);
            if (randomValue == 1)
            {
                int randomEnemy = Random.Range(0, 3);
                Instantiate(enemy[randomEnemy], new Vector2(spawnPos_x, spawnPos_y), Quaternion.identity);
            }
        }
    }

    public void SpawnEnemy()
    {
        spawnPos_x = Camera.main.transform.position.x - 6f;
        int randomValue = Random.Range(0, 2);
        if (randomValue == 1)
        {
            int randomEnemy = Random.Range(0, 3);
            Instantiate(enemy[randomEnemy], new Vector2(spawnPos_x, spawnPos_y), Quaternion.identity);
        }
        spawnPos_y -= spawnPos_y_distance;
    }
}
