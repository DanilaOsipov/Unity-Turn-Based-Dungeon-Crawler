using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6.0f;

    public GameObject target;
    public int startRoomId;
    private bool canMove;
    private MapChar mapChar = MapChar.Enemy;

    private void Start()
    {
        Messenger<int>.AddListener(GameEvent.PLAYER_ENTERED_THE_ROOM, OnPlayerEnteredTheRoom);
    }

    private void OnPlayerEnteredTheRoom(int roomId)
    {
        if (startRoomId == roomId)
        {
            canMove = true;
        }
    }

    private void Update()
    {
        if (canMove)
        {
            var path = Pathfinding.Instance.FindPath(transform, target.transform);

            if (path == null || path.Count <= 2)
                return;

            Vector3 end = path[1].Position;

            Pathfinding.Instance.UpdateObjectOnMap(transform, mapChar, end);

            Vector3 start = path[0].Position;
            Vector3 direction = end - start;
            end = start + direction;
            end *= DungeonGenerator.MovementOffset;
            end.y = transform.position.y;

            Move(end);
        }
    }

    private void Move(Vector3 to)
    {
        StartCoroutine(MoveInternal(to));
    }

    private IEnumerator MoveInternal(Vector3 to)
    {
        canMove = false;

        for (float t = 0; t < 1; t += speed * Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, to, t);

            yield return null;
        }

        canMove = true;
    }
}
