using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float rotationSpeed = 6.0f;

    private EnemyAnimation enemyAnimation;

    public GameObject target;
    private bool canMove;
    private MapChar mapChar = MapChar.Enemy;

    private void Start()
    {
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    public void Chase()
    {
        var path = Pathfinding.Instance.FindPath(transform, target.transform);

        if (path == null || path.Count < 2)
            return;

        Vector3 start = path[0].Position;
        Vector3 end = path[1].Position;
        Vector3 direction = end - start;

        Vector3 endPos = start + direction;
        endPos *= DungeonGenerator.MovementOffset;
        endPos.y = transform.position.y;

        float angle = -(Vector3.SignedAngle(direction, transform.forward, Vector3.up));

        if (angle != 0)
        {
            if (path.Count == 2)
            {
                Rotate(transform.rotation * Quaternion.Euler(0, angle, 0));
            }
            else
            {
                RotateAndMove(transform.rotation * Quaternion.Euler(0, angle, 0), endPos, end);
            }
        }
        else
        {
            if (path.Count > 2)
            {
                Move(endPos, end);
            }
        }
    }

    private void RotateAndMove(Quaternion targetRotation, Vector3 endPos, Vector3 mapPos)
    {
        StartCoroutine(RotateAndMoveInternal(targetRotation, endPos, mapPos));
    }

    private void Rotate(Quaternion targetRotation)
    {
        StartCoroutine(RotateInternal(targetRotation));
    }

    private IEnumerator RotateInternal(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;

        for (float t = 0; t < 1; t += rotationSpeed * Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.rotation = targetRotation;

        Messenger.Broadcast(GameEvent.ENEMY_TURN);
    }

    private IEnumerator RotateAndMoveInternal(Quaternion targetRotation, Vector3 endPos, Vector3 mapPos)
    {
        Quaternion startRotation = transform.rotation;

        for (float t = 0; t < 1; t += rotationSpeed * Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.rotation = targetRotation;

        Move(endPos, mapPos);
    }

    private void Move(Vector3 endPos, Vector3 mapPos)
    {
        Pathfinding.Instance.UpdateObjectOnMap(transform, mapChar, mapPos);

        StartCoroutine(MoveInternal(endPos));
    }

    private IEnumerator MoveInternal(Vector3 to)
    {
        enemyAnimation.StartWalking(() => { });

        float startTime = Time.time;
        float length = Vector3.Distance(transform.localPosition, to);
        Vector3 from = transform.localPosition;

        float t = 0;

        while (t < 1)
        {
            float distCovered = (Time.time - startTime) * speed;
            t = distCovered / length;

            transform.localPosition = Vector3.Lerp(from, to, t);

            yield return null;
        }

        enemyAnimation.StopWalking(() => { Messenger.Broadcast(GameEvent.PLAYER_TURN); });
    }
}
