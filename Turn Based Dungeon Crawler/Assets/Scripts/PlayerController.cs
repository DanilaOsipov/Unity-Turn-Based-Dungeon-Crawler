using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private DungeonMap dungeonMap;
    private IMapObject mapObject;

    private void Start()
    {
        mapObject = GetComponent<PlayerMovement>();

        dungeonMap.SpawnRandomly(transform, mapObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            dungeonMap.Move(transform, transform.forward, mapObject);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            dungeonMap.Move(transform, transform.right, mapObject);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            dungeonMap.Move(transform, -transform.forward, mapObject);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            dungeonMap.Move(transform, -transform.right, mapObject);
        }
    }
}
