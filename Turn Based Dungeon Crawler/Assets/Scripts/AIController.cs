using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int minEnemiesCount = 10;
    [SerializeField] private int maxEnemiesCount = 15;
    [SerializeField] private DungeonMap dungeonMap;

    void Start()
    {
        int count = Random.Range(minEnemiesCount, maxEnemiesCount);

        for (int i = 0; i < count; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.name = "Enemy " + i;
            IMapObject mapObject = enemy.GetComponent<EnemyMovement>();

            dungeonMap.SpawnRandomly(enemy.transform, mapObject);
        }
    }

    void Update()
    {
        
    }
}
