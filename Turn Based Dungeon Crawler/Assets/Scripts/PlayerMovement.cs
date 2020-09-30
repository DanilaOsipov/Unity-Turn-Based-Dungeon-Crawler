using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 6.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    private MapChar MapChar => MapChar.Player;

    public void MoveRight(Action callback)
    {
        Move(transform.right, callback);
    }

    public void MoveLeft(Action callback)
    {
        Move(-transform.right, callback);
    }

    public void MoveForward(Action callback)
    {
        Move(transform.forward, callback);
    }

    public void MoveBack(Action callback)
    {
        Move(-transform.forward, callback);
    }

    public void RotateLeft(Action callback)
    {
        StartCoroutine(RotateInternal(transform.rotation * Quaternion.Euler(0, -90, 0), callback));
    }

    public void RotateRight(Action callback)
    {
        StartCoroutine(RotateInternal(transform.rotation * Quaternion.Euler(0, 90, 0), callback));
    }

    private IEnumerator RotateInternal(Quaternion targetRotation, Action callback)
    {
        for (float t = 0; t < 1; t += rotationSpeed * Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

            yield return null;
        }

        transform.rotation = targetRotation;

        callback();
    }

    private void Move(Vector3 dir, Action callback)
    {
        if (Pathfinding.Instance.CanMove(transform, dir, out Vector3 movement))
        {
            Pathfinding.Instance.UpdateObjectOnMap(transform, MapChar, movement);

            Vector3 start = transform.localPosition;
            Vector3 direction = movement - start;
            movement = start + direction;
            movement *= DungeonGenerator.MovementOffset;
            movement.y = transform.position.y;

            StartCoroutine(MoveInternal(movement, callback));
        }
        else callback();
    }

    private IEnumerator MoveInternal(Vector3 to, Action callback)
    {
        for (float t = 0; t < 1; t += movementSpeed * Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, to, t);

            yield return null;
        }

        if (!BattleSystem.BattleInProcess) callback();
        else Messenger.Broadcast(GameEvent.ENEMY_TURN);
    }
}
